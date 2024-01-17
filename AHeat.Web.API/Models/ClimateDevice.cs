namespace AHeat.Web.API.Models;

public class ClimateDevice
{
    public int ID { get; set; }
    public string DeviceId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<ClimateDeviceReading> Readings { get; set; } = null!;
}
