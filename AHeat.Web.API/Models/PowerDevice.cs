using AHeat.Web.Shared.Models;

namespace AHeat.Web.API.Models;

public class PowerDevice
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public DeviceTypes DeviceType {  get; set; }
    public string HostName { get; set; } = null!;
    public string DeviceName { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
    public string DeviceMac { get; set; } = null!;
    public string DeviceModel { get; set; } = null!;
    public int DeviceGen { get; set; }
}
