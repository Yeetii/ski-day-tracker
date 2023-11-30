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

        public GetSkiDays(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetSkiDays>();
        }

        [Function("GetSkiDays")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
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

            var seasonStart = new DateTime(2023, 07, 01);

            var model = await GetStravaModel(token, seasonStart);

            if (model is null)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var activities = model.Select(activity => new Activity
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
            });

            var skiDays = CalculateSkiDays(activities);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var json = JsonSerializer.Serialize(skiDays);
            response.WriteString(json);
            return response;
        }

        private async static Task<IEnumerable<StravaModel>?> GetStravaModel(string token, DateTime? after = null)
        {
            var client = new HttpClient();
            var activities = new List<StravaModel>();
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
                var pageActivities = JsonSerializer.Deserialize<List<StravaModel>>(body);

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
    }

    public class SkiDays 
    {
        public int AlpineSkiDays { get; set; }
        public int BackcountrySkiDays { get; set; }
        public int NordicSkiDays { get; set; }
        public int SnowboardDays { get; set; }
    }
}
