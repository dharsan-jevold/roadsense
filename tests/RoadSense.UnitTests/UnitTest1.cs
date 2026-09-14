using RoadSense.Domain.Entities;
using Xunit;

namespace RoadSense.UnitTests;

public class UnitTest1
{
    [Fact]
    public void Hazard_ShouldStoreReportedValues()
    {
        var hazard = new Hazard(
            HazardType.Pothole,
            9.9252,
            78.1198,
            3);

        Assert.Equal(HazardType.Pothole, hazard.Type);
        Assert.Equal(9.9252, hazard.Latitude);
        Assert.Equal(78.1198, hazard.Longitude);
        Assert.Equal(3, hazard.Severity);
        Assert.NotEqual(Guid.Empty, hazard.Id);
    }
}