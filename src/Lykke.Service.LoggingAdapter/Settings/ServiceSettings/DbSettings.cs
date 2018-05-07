﻿using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LoggingAdapter.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
