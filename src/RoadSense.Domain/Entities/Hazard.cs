namespace RoadSense.Domain.Entities;

public class Hazard
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public HazardType Type { get; private set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public int Severity { get; private set; }

    public DateTime ReportedAtUtc { get; private set; } = DateTime.UtcNow;

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
    }
}