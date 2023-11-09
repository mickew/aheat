namespace AHeat.Application.Models.Shelly;
internal record ChannelStatus(
     int Id,
     string Source,
     bool Output,
     DeviceTemperature Temperature
 );

public record DeviceTemperature(
    double TC,
    double TF
);

internal record RelayStatus(
    bool Ison,
    bool HasTimer,
    int TimerStarted,
    int TimerDuration,
    int TimerRemaining,
    bool Overpower,
    string Source
);
