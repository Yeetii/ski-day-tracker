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

    [Fact]
    public void Should_sum_up_vertical_meters_for_backcountry_skiing()
    {
        var activities = new List<Activity>
        {
            new() { Type = ActivityType.BackcountrySkiing, Date = new DateOnly(2023, 07, 01), ElevationGain = 100 },
            new() { Type = ActivityType.BackcountrySkiing, Date = new DateOnly(2023, 07, 01), ElevationGain = 200 },
        };

        var skiDays = SkiDaysCalculator.CalculateSkiDays(activities);

        skiDays.BackcountrySkiDays.Should().Be(1);
        skiDays.BackcountrySkiElevationGain.Should().Be(300);
    }

    [Fact]
    public void Different_activity_types_on_same_day_should_only_be_one_ski_day()
    {
        var activities = new List<Activity>
        {
            new() { Type = ActivityType.AlpineSkiing, Date = new DateOnly(2023, 07, 01) },
            new() { Type = ActivityType.BackcountrySkiing, Date = new DateOnly(2023, 07, 01) },
        };

        var skiDays = SkiDaysCalculator.CalculateSkiDays(activities);

        skiDays.TotalSkiDays.Should().Be(1);
    }
}