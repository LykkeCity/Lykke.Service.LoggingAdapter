using System;
using System.Collections.Generic;
using System.Text;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LoggingAdapter.Settings
{
    public class LogSettings
    {
        [AzureTableCheck]
        public string ConnString { get; set; }

        public string AppName { get; set; }
    }
}
