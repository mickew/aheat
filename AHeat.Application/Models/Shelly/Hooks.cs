namespace AHeat.Application.Models.Shelly;
public record WebHooks(
     IReadOnlyList<Hook> Hooks,
     int Rev
);

public record Hook(
    int Id,
    int Cid,
    bool Enable,
    string Event,
    string Name,
    IReadOnlyList<string> Urls,
    object Condition,
    int RepeatPeriod
);

internal record CreateWebHook(
    int Id,
    string Method,
    CreateWebHookParams Params
);

internal record CreateWebHookParams(
    int Cid,
    bool Enable,
    string Event,
    string Name,
    IReadOnlyList<string> Urls
);
internal record UpdateWebHook(
    int Id,
    string Method,
    UpdateWebHookParams Params
);

internal record UpdateWebHookParams(
    int Id,
    int Cid,
    bool Enable,
    string Name,
    IReadOnlyList<string> Urls
);
