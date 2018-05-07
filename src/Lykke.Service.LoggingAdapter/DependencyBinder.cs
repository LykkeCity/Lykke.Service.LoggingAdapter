using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Log;
using Microsoft.Extensions.Configuration;

namespace Lykke.Service.LoggingAdapter
{
    public class DependencyBinder
    {
        public ILog Log { get; private set; }


        public ContainerBuilder Bind(IConfigurationRoot configuration, ContainerBuilder builder = null,
            bool fromTests = false)
        {
            throw new NotImplementedException();
        }
    }
}
