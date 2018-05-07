using System;
using System.Collections.Generic;
using System.Text;
using Common.Log;

namespace Lykke.Service.LoggingAdapter.Core.Services
{
    public interface ILogFactory
    {
        ILog GetLog(string appName);
    }
}
