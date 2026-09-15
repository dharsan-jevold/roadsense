using RoadSense.Domain.Entities;

namespace RoadSense.Application.Repositories;

public interface IHazardRepository
{
    Task<Hazard> AddAsync(Hazard hazard);

    Task<Hazard?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Hazard>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusMeters);

    Task<Hazard?> FindNearbyByTypeAsync(
        HazardType type,
        double latitude,
        double longitude,
        double radiusMeters);

    Task<Hazard> UpdateAsync(Hazard hazard);
}