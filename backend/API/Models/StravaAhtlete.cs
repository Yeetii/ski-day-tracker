using System.Text.Json.Serialization;

namespace backend;
public record Bike(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("primary")] bool Primary,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("resource_state")] int ResourceState,
        [property: JsonPropertyName("distance")] int Distance
    );

    public record StravaAthlete(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("resource_state")] int ResourceState,
        [property: JsonPropertyName("firstname")] string Firstname,
        [property: JsonPropertyName("lastname")] string Lastname,
        [property: JsonPropertyName("city")] string City,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("country")] string Country,
        [property: JsonPropertyName("sex")] string Sex,
        [property: JsonPropertyName("premium")] bool Premium,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
        [property: JsonPropertyName("badge_type_id")] int BadgeTypeId,
        [property: JsonPropertyName("profile_medium")] string ProfileMedium,
        [property: JsonPropertyName("profile")] string Profile,
        [property: JsonPropertyName("friend")] object Friend,
        [property: JsonPropertyName("follower")] object Follower,
        [property: JsonPropertyName("follower_count")] int FollowerCount,
        [property: JsonPropertyName("friend_count")] int FriendCount,
        [property: JsonPropertyName("mutual_friend_count")] int MutualFriendCount,
        [property: JsonPropertyName("athlete_type")] int AthleteType,
        [property: JsonPropertyName("date_preference")] string DatePreference,
        [property: JsonPropertyName("measurement_preference")] string MeasurementPreference,
        [property: JsonPropertyName("clubs")] IReadOnlyList<object> Clubs,
        [property: JsonPropertyName("ftp")] object Ftp,
        [property: JsonPropertyName("weight")] double Weight,
        [property: JsonPropertyName("bikes")] IReadOnlyList<Bike> Bikes,
        [property: JsonPropertyName("shoes")] IReadOnlyList<Shoe> Shoes
    );

    public record Shoe(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("primary")] bool Primary,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("resource_state")] int ResourceState,
        [property: JsonPropertyName("distance")] int Distance
    );

