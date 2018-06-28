namespace Lykke.Service.LoggingAdapter.Core.Services
{
    public interface IHealthNotifierSender
    {
        void SendNotification(string appName, string appVersion, string envInfo, string healthMessage, object context = null);
    }
}
