using RoadSense.Domain.Entities;
using Xunit;

namespace RoadSense.UnitTests;

public class UnitTest1
{
    [Fact]
    public void Hazard_ShouldStoreReportedValues()
    {
        // Arrange
        var hazard = new Hazard(
            HazardType.Pothole,
            9.9252,
            78.1198,
            3);

        // Assert
        Assert.Equal(
            HazardType.Pothole,
            hazard.Type);

        Assert.Equal(
            9.9252,
            hazard.Latitude);

        Assert.Equal(
            78.1198,
            hazard.Longitude);

        Assert.Equal(
            3,
            hazard.Severity);

        Assert.Equal(
            1,
            hazard.ReportCount);

        Assert.NotEqual(
            Guid.Empty,
            hazard.Id);
    }

    [Fact]
    public void Hazard_ShouldCalculateHigherConfidenceWithMoreReports()
    {
        // Arrange
        var hazard = new Hazard(
            HazardType.Pothole,
            9.9252,
            78.1198,
            3);

        var now = DateTime.UtcNow;

        // Act
        var initialConfidence =
            hazard.GetConfidenceScore(now);

        hazard.AddReport(3);
        hazard.AddReport(4);
        hazard.AddReport(4);

        var higherConfidence =
            hazard.GetConfidenceScore(now);

        // Assert
        Assert.True(
            higherConfidence > initialConfidence);

        Assert.True(
            higherConfidence <= 95);
    }

    [Fact]
    public void Hazard_ShouldCalculateConfidenceLevel()
    {
        // Arrange
        var hazard = new Hazard(
            HazardType.Pothole,
            9.9252,
            78.1198,
            3);

        var now = DateTime.UtcNow;

        // One fresh report = 40% confidence.
        Assert.Equal(
            ConfidenceLevel.Low,
            hazard.GetConfidenceLevel(now));

        // Add three more reports.
        // Total = 4 reports = 70% confidence.
        hazard.AddReport(3);
        hazard.AddReport(3);
        hazard.AddReport(3);

        Assert.Equal(
            ConfidenceLevel.Medium,
            hazard.GetConfidenceLevel(now));

        // Add four more reports.
        // Total = 8 reports = 85% confidence.
        hazard.AddReport(3);
        hazard.AddReport(3);
        hazard.AddReport(3);
        hazard.AddReport(3);

        Assert.Equal(
            ConfidenceLevel.High,
            hazard.GetConfidenceLevel(now));
    }
}