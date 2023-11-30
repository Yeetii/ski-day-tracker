namespace backend;

public class Athlete
{
    public string Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public SkiDays SkiDays { get; set; }

    public Athlete()
    {
        Id = "";
        Firstname = "";
        Lastname = "";
        City = "";
        State = "";
        Country = "";
        SkiDays = new SkiDays();
    }

    public Athlete(StravaAthlete stravaAthlete, SkiDays skiDays)
    {
        Id = stravaAthlete.Id.ToString();
        Firstname = stravaAthlete.Firstname;
        Lastname = stravaAthlete.Lastname;
        City = stravaAthlete.City;
        State = stravaAthlete.State;
        Country = stravaAthlete.Country;
        SkiDays = skiDays;
    }
}