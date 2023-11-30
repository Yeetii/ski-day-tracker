namespace backend;

public interface ICosmosClient
{
    Task<Athlete> GetAthleteAsync(string id);
    Task<IEnumerable<Athlete>> GetAthletesAsync();
    Task<Athlete> UpsertAthleteAsync(Athlete athlete);
}
