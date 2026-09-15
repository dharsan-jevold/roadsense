using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using RoadSense.Application.Repositories;
using RoadSense.Domain.Entities;
using RoadSense.Infrastructure.Data;

namespace RoadSense.Infrastructure.Repositories;

public class HazardRepository : IHazardRepository
{
    private readonly RoadSenseDbContext _dbContext;

    public HazardRepository(RoadSenseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Hazard> AddAsync(Hazard hazard)
    {
        _dbContext.Hazards.Add(hazard);

        await _dbContext.SaveChangesAsync();

        return hazard;
    }

    public async Task<Hazard?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Hazards
            .FirstOrDefaultAsync(h => h.Id == id);
    }
    public async Task<Hazard?> FindNearbyByTypeAsync(
    HazardType type,
    double latitude,
    double longitude,
    double radiusMeters)
{
    var vehicleLocation = new Point(longitude, latitude)
    {
        SRID = 4326
    };

    return await _dbContext.Hazards
        .Where(h =>
            h.Type == type &&
            h.Location.IsWithinDistance(
                vehicleLocation,
                radiusMeters))
        .OrderByDescending(h => h.LastReportedAtUtc)
        .FirstOrDefaultAsync();
}

public async Task<Hazard> UpdateAsync(Hazard hazard)
{
    await _dbContext.SaveChangesAsync();

    return hazard;
}

    public async Task<IReadOnlyList<Hazard>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusMeters)
    {
        var vehicleLocation = new Point(longitude, latitude)
        {
            SRID = 4326
        };

        return await _dbContext.Hazards
            .Where(h =>
                h.Location.IsWithinDistance(
                    vehicleLocation,
                    radiusMeters))
            .ToListAsync();
    }
}