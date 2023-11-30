using backend;

namespace Tests;

public class SkiDaysCalculatorTests
{
    [Fact]
    public void Multiple_activities_in_one_day_should_only_count_once()
    {
        var activities = new List<Activity>
        {
            new() { Type = ActivityType.AlpineSkiing, Date = new DateOnly(2023, 07, 01) },
            new() { Type = ActivityType.AlpineSkiing, Date = new DateOnly(2023, 07, 01) },
        };

        var skiDays = SkiDaysCalculator.CalculateSkiDays(activities);

        skiDays.AlpineSkiDays.Should().Be(1);
    }
}