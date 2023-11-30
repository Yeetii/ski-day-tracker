namespace backend;

public static class SkiDaysCalculator
{
    public static SkiDays CalculateSkiDays(IEnumerable<Activity> activities)
    {
        var alpineSkiDays = activities.Where(activity => activity.Type == ActivityType.AlpineSkiing)
            .Select(activity => activity.Date).Distinct();
        var backcountrySkiDays = activities.Where(activity => activity.Type == ActivityType.BackcountrySkiing)
            .Select(activity => activity.Date).Distinct();
        var nordicSkiDays = activities.Where(activity => activity.Type == ActivityType.NordicSkiing)
            .Select(activity => activity.Date).Distinct();
        var snowboardingDays = activities.Where(activity => activity.Type == ActivityType.Snowboarding)
            .Select(activity => activity.Date).Distinct();
        
        return new SkiDays
        {
            AlpineSkiDays = alpineSkiDays.Count(),
            BackcountrySkiDays = backcountrySkiDays.Count(),
            NordicSkiDays = nordicSkiDays.Count(),
            SnowboardDays = snowboardingDays.Count(),
        };
    }
}