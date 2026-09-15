using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace RoadSense.Domain.Entities;

public class Hazard
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public HazardType Type { get; private set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    [JsonIgnore]
    public Point Location { get; private set; }

    public int Severity { get; private set; }

    public int ReportCount { get; private set; } = 1;

    public DateTime ReportedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime LastReportedAtUtc { get; private set; } = DateTime.UtcNow;

    public Hazard(
        HazardType type,
        double latitude,
        double longitude,
        int severity)
    {
        Type = type;
        Latitude = latitude;
        Longitude = longitude;
        Severity = severity;

        Location = new Point(longitude, latitude)
        {
            SRID = 4326
        };
    }
    public double GetConfidenceScore(DateTime utcNow)
{
    // More independent reports increase confidence,
    // but with diminishing returns.
    var reportConfidence =
        Math.Min(
            95.0,
            40.0 + (15.0 * Math.Log2(ReportCount)));

    // Fresh reports should be trusted more than old reports.
    var age = utcNow - LastReportedAtUtc;

    var freshnessMultiplier = age.TotalHours switch
    {
        <= 1 => 1.00,
        <= 6 => 0.90,
        <= 24 => 0.75,
        <= 72 => 0.50,
        _ => 0.25
    };

    return Math.Round(
        reportConfidence * freshnessMultiplier,
        1);
}
    public ConfidenceLevel GetConfidenceLevel(DateTime utcNow)
{
    var score = GetConfidenceScore(utcNow);

    return score switch
    {
        >= 75 => ConfidenceLevel.High,
        >= 50 => ConfidenceLevel.Medium,
        _ => ConfidenceLevel.Low
    };
}

    public void AddReport(int severity)
    {
        ReportCount++;

        if (severity > Severity)
        {
            Severity = severity;
        }

        LastReportedAtUtc = DateTime.UtcNow;
    }
}