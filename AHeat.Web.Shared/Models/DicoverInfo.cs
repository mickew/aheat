using AHeat.Web.Shared.Models;

namespace AHeat.Web.Shared;

public record DicoverInfo(string DeviceHostName, DeviceTypes DeviceType, string DeviceName, string DeviceId, string DeviceMac, string DeviceModel, int DeviceGen);

