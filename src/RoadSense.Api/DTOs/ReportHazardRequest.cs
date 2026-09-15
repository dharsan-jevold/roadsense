using RoadSense.Domain.Entities;

namespace RoadSense.Api.DTOs;

public record ReportHazardRequest(
    HazardType Type,
    double Latitude,
    double Longitude,
    int Severity);