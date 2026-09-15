using RoadSense.Domain.Entities;

namespace RoadSense.Api.DTOs;

public record HazardResponse(
    Guid Id,
    HazardType Type,
    double Latitude,
    double Longitude,
    int Severity,
    int ReportCount,
    double ConfidenceScore,
    ConfidenceLevel ConfidenceLevel,
    DateTime ReportedAtUtc,
    DateTime LastReportedAtUtc);