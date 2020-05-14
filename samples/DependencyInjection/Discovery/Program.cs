﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discovery
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Dictionary<string, string>
            {
                ["Discovery:ProbeEndpoint"] = "net.tcp://localhost:8001/Probe"
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(settings);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddLogging(l => l.AddConsole());

            services.AddServiceDiscovery(() => new NetTcpBinding(SecurityMode.None))
                    .ConfigureServiceDiscovery(configuration.GetSection("Discovery"));

            services.DiscoverNetTcpService<IMyService>(binding => binding.Security.Mode = SecurityMode.None);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<IMyService>();

            var isServiceFound = service != null;

            Console.WriteLine($"Service { (isServiceFound ? "" : "not ")}found");

            if (isServiceFound)
            {
                var response = service.Echo("This is my message");

                Console.WriteLine(response);
            }
        }
    }

    [ServiceContract(Namespace="http://samples.educations.com/", Name="MyService")]
    public interface IMyService
    {
        [OperationContract]
        string Echo(string message);
    }
}
