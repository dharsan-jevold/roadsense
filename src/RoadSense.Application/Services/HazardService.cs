using RoadSense.Application.Repositories;
using RoadSense.Domain.Entities;

namespace RoadSense.Application.Services;

public class HazardService : IHazardService
{
    private readonly IHazardRepository _hazardRepository;

    public HazardService(IHazardRepository hazardRepository)
    {
        _hazardRepository = hazardRepository;
    }

    public async Task<Hazard> CreateHazardAsync(
    HazardType type,
    double latitude,
    double longitude,
    int severity)
{
    const double clusteringRadiusMeters = 50;

    var existingHazard =
        await _hazardRepository.FindNearbyByTypeAsync(
            type,
            latitude,
            longitude,
            clusteringRadiusMeters);

    if (existingHazard is not null)
    {
        existingHazard.AddReport(severity);

        return await _hazardRepository.UpdateAsync(
            existingHazard);
    }

    var hazard = new Hazard(
        type,
        latitude,
        longitude,
        severity);

    return await _hazardRepository.AddAsync(hazard);
}    public async Task<IReadOnlyList<Hazard>> GetNearbyHazardsAsync(
    double latitude,
    double longitude,
    double radiusMeters)
{
    return await _hazardRepository.GetNearbyAsync(
        latitude,
        longitude,
        radiusMeters);
}

    public async Task<Hazard?> GetHazardAsync(Guid id)
    {
        return await _hazardRepository.GetByIdAsync(id);
    }
}