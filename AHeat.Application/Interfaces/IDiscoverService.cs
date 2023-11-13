using AHeat.Web.Shared;

namespace AHeat.Application.Interfaces
{
    public interface IDiscoverService
    {
        Task<DicoverInfo> Discover(string url);
    }
}
