namespace AHeat.Web.Shared;
public class ClimateDeviceDto
{
    public ClimateDeviceDto(): this(default(int), string.Empty, string.Empty) { }

    public ClimateDeviceDto(int id, string deviceId, string name)
    {
        ID = id;
        DeviceId = deviceId;
        Name = name;
    }
    public int ID { get; set; }
    public string DeviceId { get; set; }
    public string Name { get; set; }
}
