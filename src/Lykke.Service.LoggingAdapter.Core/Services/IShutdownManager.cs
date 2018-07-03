using System.Threading.Tasks;

namespace Lykke.Service.LoggingAdapter.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
