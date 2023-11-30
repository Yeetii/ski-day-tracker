namespace backend;

public enum ActivityType
{
    AlpineSkiing,
    BackcountrySkiing,
    NordicSkiing,
    Snowboarding,
    Other
}

public class Activity {
    public ActivityType Type { get; set; }
    public DateOnly Date { get; set; }
}

