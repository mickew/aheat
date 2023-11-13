namespace AHeat.Application.Models.Shelly;
public record DeviceInfo(
    string Name,
    string Id,
    string Mac,
    int Slot,
    string Model,
    int Gen,
    string FwId,
    string Ver,
    string App,
    bool AuthEn,
    object AuthDomain
);

internal record Device(
    string Type,
    string Mac,
    string Hostname
);

internal record DeviceInfoSettings(
    Device Device,
    string Name
);
