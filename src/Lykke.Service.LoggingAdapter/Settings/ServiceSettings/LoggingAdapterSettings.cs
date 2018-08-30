using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LoggingAdapter.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LoggingAdapterSettings
    {
        public DbSettings Db { get; set; }

        public IEnumerable<LogSettings> Loggers { get; set; }

        [Optional]
        public int LogInitionMaxDegreeOfParallelism { get; set; } = 3;
    }
}
