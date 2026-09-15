using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RoadSense.Domain.Entities;
using Xunit;

namespace RoadSense.IntegrationTests;

public class HazardApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HazardApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static (double Latitude, double Longitude) GetUniqueLocation()
    {
        var value = Random.Shared.NextDouble();

        return (
            20.0 + value,
            70.0 + value);
    }

    [Fact]
    public async Task PostHazard_ShouldCreateHazard_AndGetSameHazard()
    {
        // Arrange
        var location = GetUniqueLocation();

        var request = new
        {
            type = HazardType.Pothole,
            latitude = location.Latitude,
            longitude = location.Longitude,
            severity = 3
        };

        // Act - POST
        var postResponse = await _client.PostAsJsonAsync(
            "/api/hazard",
            request);

        // Assert - POST
        Assert.Equal(
            HttpStatusCode.Created,
            postResponse.StatusCode);

        var postJson = await postResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var hazardId = postJson
            .GetProperty("id")
            .GetGuid();

        Assert.NotEqual(
            Guid.Empty,
            hazardId);

        // A newly created hazard should have one report.
        Assert.Equal(
            1,
            postJson.GetProperty("reportCount").GetInt32());

        // Act - GET
        var getResponse = await _client.GetAsync(
            $"/api/hazard/{hazardId}");

        // Assert - GET
        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var getJson = await getResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            hazardId,
            getJson.GetProperty("id").GetGuid());

        Assert.Equal(
            "Pothole",
            getJson.GetProperty("type").GetString());

        Assert.Equal(
            location.Latitude,
            getJson.GetProperty("latitude").GetDouble());

        Assert.Equal(
            location.Longitude,
            getJson.GetProperty("longitude").GetDouble());

        Assert.Equal(
            3,
            getJson.GetProperty("severity").GetInt32());

        Assert.Equal(
            1,
            getJson.GetProperty("reportCount").GetInt32());
    }

    [Fact]
    public async Task GetNearbyHazards_ShouldReturnOnlyHazardsWithinRadius()
    {
        // Arrange
        var searchLocation = GetUniqueLocation();

        // Approximately 100 meters from the search point.
        var nearbyHazard = new
        {
            type = HazardType.Pothole,
            latitude = searchLocation.Latitude,
            longitude = searchLocation.Longitude + 0.001,
            severity = 3
        };

        // Approximately 5 km away from the search point.
        var farHazard = new
        {
            type = HazardType.Accident,
            latitude = searchLocation.Latitude + 0.045,
            longitude = searchLocation.Longitude,
            severity = 5
        };

        // Act - Create nearby hazard
        var nearbyResponse = await _client.PostAsJsonAsync(
            "/api/hazard",
            nearbyHazard);

        Assert.Equal(
            HttpStatusCode.Created,
            nearbyResponse.StatusCode);

        var nearbyJson = await nearbyResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var nearbyHazardId = nearbyJson
            .GetProperty("id")
            .GetGuid();

        // Act - Create far hazard
        var farResponse = await _client.PostAsJsonAsync(
            "/api/hazard",
            farHazard);

        Assert.Equal(
            HttpStatusCode.Created,
            farResponse.StatusCode);

        var farJson = await farResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var farHazardId = farJson
            .GetProperty("id")
            .GetGuid();

        // Act - Search within 1 km
        var response = await _client.GetAsync(
            "/api/hazard/nearby" +
            $"?latitude={searchLocation.Latitude}" +
            $"&longitude={searchLocation.Longitude}" +
            "&radiusMeters=1000");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var hazards = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            JsonValueKind.Array,
            hazards.ValueKind);

        var returnedIds = hazards
            .EnumerateArray()
            .Select(h => h.GetProperty("id").GetGuid())
            .ToList();

        Assert.Contains(
            nearbyHazardId,
            returnedIds);

        Assert.DoesNotContain(
            farHazardId,
            returnedIds);
    }
    [Fact]
public void Hazard_ShouldCalculateHigherConfidenceWithMoreReports()
{
    var hazard = new Hazard(
        HazardType.Pothole,
        9.9252,
        78.1198,
        3);

    var now = DateTime.UtcNow;

    var initialConfidence =
        hazard.GetConfidenceScore(now);

    hazard.AddReport(3);
    hazard.AddReport(4);
    hazard.AddReport(4);

    var higherConfidence =
        hazard.GetConfidenceScore(now);

    Assert.True(
        higherConfidence > initialConfidence);

    Assert.True(
        higherConfidence <= 95);
}
    [Fact]
public void Hazard_ShouldCalculateConfidenceLevel()
{
    var hazard = new Hazard(
        HazardType.Pothole,
        9.9252,
        78.1198,
        3);

    var now = DateTime.UtcNow;

    // One fresh report currently produces 40% confidence.
    Assert.Equal(
        ConfidenceLevel.Low,
        hazard.GetConfidenceLevel(now));

    // Add reports to increase confidence.
    hazard.AddReport(3);
    hazard.AddReport(3);
    hazard.AddReport(3);

    // Four fresh reports produce 70% confidence.
    Assert.Equal(
        ConfidenceLevel.Medium,
        hazard.GetConfidenceLevel(now));

    // Add more reports to cross the high-confidence threshold.
    hazard.AddReport(3);
    hazard.AddReport(3);
    hazard.AddReport(3);
    hazard.AddReport(3);

    Assert.Equal(
        ConfidenceLevel.High,
        hazard.GetConfidenceLevel(now));
}

    [Fact]
    public async Task PostHazard_ShouldClusterNearbySameTypeHazard()
    {
        // Arrange
        var location = GetUniqueLocation();

        var firstReport = new
        {
            type = HazardType.Debris,
            latitude = location.Latitude,
            longitude = location.Longitude,
            severity = 2
        };

        var secondReport = new
        {
            type = HazardType.Debris,

            // Approximately 20 meters from the first report.
            latitude = location.Latitude + 0.00018,
            longitude = location.Longitude,
            severity = 4
        };

        // Act - First report
        var firstResponse = await _client.PostAsJsonAsync(
            "/api/hazard",
            firstReport);

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        var firstJson = await firstResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var firstHazardId = firstJson
            .GetProperty("id")
            .GetGuid();

        var firstReportCount = firstJson
            .GetProperty("reportCount")
            .GetInt32();

        // A genuinely new hazard should have one report.
        Assert.Equal(
            1,
            firstReportCount);

        // Act - Second report
        var secondResponse = await _client.PostAsJsonAsync(
            "/api/hazard",
            secondReport);

        Assert.Equal(
            HttpStatusCode.Created,
            secondResponse.StatusCode);

        var secondJson = await secondResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var secondHazardId = secondJson
            .GetProperty("id")
            .GetGuid();

        var secondReportCount = secondJson
            .GetProperty("reportCount")
            .GetInt32();

        // Both reports should refer to the same hazard.
        Assert.Equal(
            firstHazardId,
            secondHazardId);

        // The second report should increase the report count by one.
        Assert.Equal(
            firstReportCount + 1,
            secondReportCount);

        // The higher severity should be retained.
        Assert.Equal(
            4,
            secondJson.GetProperty("severity").GetInt32());
    }
}