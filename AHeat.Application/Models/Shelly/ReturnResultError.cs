namespace AHeat.Application.Models.Shelly;
internal record ReturnResultError(
     int Id,
     string Src,
     string Dst,
     Error Error
);

internal record Error(
     int Code,
     string Message
);
