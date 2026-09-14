using RoadSense.Domain.Entities;

namespace RoadSense.Application.Services;

public interface IHazardService
{
    Hazard CreateHazard(
        HazardType type,
        double latitude,
        double longitude,
        int severity);

    Hazard? GetHazard(Guid id);
}