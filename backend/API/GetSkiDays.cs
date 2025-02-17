using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using static backend.SkiDaysCalculator;

namespace backend
{
    public class GetSkiDays
    {
        private readonly ILogger _logger;
        private readonly ICosmosClient _cosmosClient;
        private readonly DateTime SeasonStart;

        public GetSkiDays(ILoggerFactory loggerFactory, ICosmosClient cosmosClient)
        {
            _logger = loggerFactory.CreateLogger<GetSkiDays>();
            _cosmosClient = cosmosClient;
            SeasonStart = DateTime.Now < new DateTime(DateTime.Now.Year, 07, 01)
                ? new DateTime(DateTime.Now.Year - 1, 07, 01)
                : new DateTime(DateTime.Now.Year, 07, 01);
        }

        [Function("GetSkiDays")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            string? token = req.Query["token"];
            string? authorizationCode = req.Query["authorizationCode"];

            if (token is null)
            {
                if (authorizationCode is not null)
                {
                    token = await TokenExchange.GetToken(authorizationCode);
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            var stravaAthlete = await GetStravaAthlete(token);
            var stravaActivities = await GetStravaModel(token, SeasonStart);

            if (stravaActivities is null || stravaAthlete is null)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            IEnumerable<Activity> activities = ParseActivities(stravaActivities);

            var skiDays = CalculateSkiDays(activities);

            var athlete = new Athlete(stravaAthlete, skiDays);

            await _cosmosClient.UpsertAthleteAsync(athlete);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var json = JsonSerializer.Serialize(skiDays);
            response.WriteString(json);
            return response;
        }

        private static IEnumerable<Activity> ParseActivities(IEnumerable<StravaActivity>? stravaActivities)
        {
            return stravaActivities.Select(activity => new Activity
            {
                Type = activity.Type switch
                {
                    "AlpineSki" => ActivityType.AlpineSkiing,
                    "BackcountrySki" => ActivityType.BackcountrySkiing,
                    "NordicSki" => ActivityType.NordicSkiing,
                    "Snowboard" => ActivityType.Snowboarding,
                    _ => ActivityType.Other,
                },
                Date = DateOnly.FromDateTime(activity.StartDate),
                ElevationGain = activity.TotalElevationGain,
            });
        }

        private async static Task<IEnumerable<StravaActivity>?> GetStravaModel(string token, DateTime? after = null)
        {
            var client = new HttpClient();
            var activities = new List<StravaActivity>();
            int page = 1;
            bool hasMorePages = true;

            while (hasMorePages)
            {
                var requestUri = $"https://www.strava.com/api/v3/athlete/activities?per_page=200&page={page}";

                if (after.HasValue)
                {
                    var epoch = new DateTimeOffset(after.Value).ToUnixTimeSeconds();
                    requestUri += $"&after={epoch}";
                }

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(requestUri),
                    Headers =
                    {
                        { "Authorization", $"Bearer {token}" },
                    },
                };

                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var pageActivities = JsonSerializer.Deserialize<List<StravaActivity>>(body);

                if (pageActivities != null)
                {
                    activities.AddRange(pageActivities);
                    page++;
                }
                if (pageActivities?.Count < 200)
                {
                    hasMorePages = false;
                }
            }

            return activities;
        }

        private async static Task<StravaAthlete?> GetStravaAthlete(string token)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://www.strava.com/api/v3/athlete"),
                Headers =
                {
                    { "Authorization", $"Bearer {token}" },
                },
            };

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var athlete = JsonSerializer.Deserialize<StravaAthlete>(body);
            return athlete;
        }
    }
}
