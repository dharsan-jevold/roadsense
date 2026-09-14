using RoadSense.Domain.Entities;

namespace RoadSense.Application.Services;

public class HazardService : IHazardService
{
    private readonly Dictionary<Guid, Hazard> _hazards = new();

    public Hazard CreateHazard(
        HazardType type,
        double latitude,
        double longitude,
        int severity)
    {
        var hazard = new Hazard(
            type,
            latitude,
            longitude,
            severity);

        _hazards[hazard.Id] = hazard;

        return hazard;
    }

    public Hazard? GetHazard(Guid id)
    {
        _hazards.TryGetValue(id, out var hazard);

        return hazard;
    }
}