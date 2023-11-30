namespace backend;

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

public class CosmosClient : ICosmosClient
{
    private readonly Container _container;

    public CosmosClient(string connectionString, string databaseName, string containerName)
    {
        var cosmosClientOptions = new CosmosClientOptions()
        {
            SerializerOptions = new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };

        var cosmosClient = new Microsoft.Azure.Cosmos.CosmosClient(connectionString, cosmosClientOptions);
        var database = cosmosClient.GetDatabase(databaseName);
        _container = database.GetContainer(containerName);
    }

    public async Task<Athlete> GetAthleteAsync(string id)
    {
        return await _container.ReadItemAsync<Athlete>(id, new PartitionKey(id));
    }

    public async Task<IEnumerable<Athlete>> GetAthletesAsync()
    {
        var query = _container.GetItemQueryIterator<Athlete>("SELECT * FROM c");
        var athletes = new List<Athlete>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            athletes.AddRange(response);
        }

        return athletes;
    }

    public async Task<Athlete> UpsertAthleteAsync(Athlete athlete)
    {
        return await _container.UpsertItemAsync(athlete);
    }
}
