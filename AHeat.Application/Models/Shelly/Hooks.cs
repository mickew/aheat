using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    string method,
    CreateWebHookParams Params
);

internal record CreateWebHookParams(
    int Cid,
    bool Enable,
    string Event,
    string Name,
    IReadOnlyList<string> Urls
);
