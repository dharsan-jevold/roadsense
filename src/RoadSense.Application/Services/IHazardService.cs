using RoadSense.Domain.Entities;

namespace RoadSense.Application.Services;

public interface IHazardService
{
    Task<Hazard> CreateHazardAsync(
        HazardType type,
        double latitude,
        double longitude,
        int severity);

    Task<Hazard?> GetHazardAsync(Guid id);

    Task<IReadOnlyList<Hazard>> GetNearbyHazardsAsync(
        double latitude,
        double longitude,
        double radiusMeters);
}