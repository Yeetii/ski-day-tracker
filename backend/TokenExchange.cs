using System.Text.Json;

static class TokenExchange
{
    public static async Task<string> GetToken(string authorizationCode)
    {
        string clientId = "117539";
        string clientSecret = Environment.GetEnvironmentVariable($"StravaClientSecret");


        using HttpClient client = new();
        // Set the token endpoint URL
        string tokenEndpoint = "https://www.strava.com/api/v3/oauth/token";

        // Prepare the request data
        var requestData = new List<KeyValuePair<string, string>>
            {
                new("client_id", clientId),
                new("client_secret", clientSecret),
                new("code", authorizationCode),
                new("grant_type", "authorization_code")
            };

        // Convert the request data to form-urlencoded format
        var content = new FormUrlEncodedContent(requestData);

        // Send the POST request
        HttpResponseMessage response = await client.PostAsync(tokenEndpoint, content);

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
            // Parse and handle the response
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokens = JsonSerializer.Deserialize<Tokens>(responseContent);
            Console.WriteLine($"Access token: {tokens.access_token}");
            return tokens.access_token;
        }
        else
        {
            Console.WriteLine($"Error: {response.ReasonPhrase}");
            return null;
        }
    }
    record Tokens(string access_token);
}
