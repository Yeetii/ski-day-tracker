using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace backend
{
    public class GetLeaderboad
    {
        private readonly ICosmosClient _cosmosClient;

        public GetLeaderboad(ICosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        [Function("GetLeaderboad")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var athletes = await _cosmosClient.GetAthletesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var json = JsonSerializer.Serialize(athletes);
            response.WriteString(json);
            return response;
        }

    }
}
