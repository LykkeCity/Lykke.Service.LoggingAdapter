using JetBrains.Annotations;

namespace Lykke.Service.LoggingAdapter.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LoggingAdapterSettings
    {
        public DbSettings Db { get; set; }
    }
}
