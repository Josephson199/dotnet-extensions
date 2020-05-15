using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AutoFixture.Idioms;
using EMG.Extensions.DependencyInjection.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tests
{
    public class ServiceCollectionDiscoveryExtensionsTests
    {
        [Test, CustomAutoData]
        public void AddServiceDiscovery_registers_IDiscoveryService(ServiceCollection services)
        {
            services.AddServiceDiscovery();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<IDiscoveryService>();
        }

        [Test, CustomAutoData]
        public void AddServiceDiscovery_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetMethod(nameof(ServiceCollectionDiscoveryExtensions.AddServiceDiscovery)));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_configures_options_with_delegate(ServiceCollection services, Action<ServiceModelDiscoveryOptions> configureOptions)
        {
            services.ConfigureServiceDiscovery(configureOptions);

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<ServiceModelDiscoveryOptions>>();

            Mock.Get(configureOptions).Verify(p => p(options.Value));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_configures_options_with_configuration(ServiceCollection services, ConfigurationBuilder configurationBuilder, string sectionName, Uri probeEndpoint)
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                [$"{sectionName}:ProbeEndpoint"] = probeEndpoint.ToString()
            });

            var configuration = configurationBuilder.Build();
            
            services.ConfigureServiceDiscovery(configuration.GetSection(sectionName));

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<ServiceModelDiscoveryOptions>>();

            Assert.That(options.Value.ProbeEndpoint, Is.EqualTo(probeEndpoint));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.ConfigureServiceDiscovery)));
        }

        [Test, CustomAutoData]
        public void DiscoverService_registers_service_using_discovery(ServiceCollection services, Func<NetTcpBinding> bindingFactory, ServiceLifetime lifetime, ITestService testService, IDiscoveryService discoveryService)
        {
            services.DiscoverService<ITestService>(bindingFactory, lifetime);

            services.AddSingleton<IDiscoveryService>(discoveryService);

            Mock.Get(discoveryService).Setup(p => p.Discover<ITestService>(It.IsAny<Binding>())).Returns(testService);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            Assert.That(service, Is.SameAs(testService));

            Mock.Get(bindingFactory).Verify(p => p(), Times.Once);
        }

        [Test, CustomAutoData]
        public void DiscoverBasicHttpService_registers_service_for_BasicHttp(ServiceCollection services, Action<BasicHttpBinding> customizeBinding, ServiceLifetime lifetime, ITestService testService, IDiscoveryService discoveryService)
        {
            services.DiscoverBasicHttpService<ITestService>(customizeBinding, lifetime);

            services.AddSingleton<IDiscoveryService>(discoveryService);

            Mock.Get(discoveryService).Setup(p => p.Discover<ITestService>(It.IsAny<Binding>())).Returns(testService);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            Assert.That(service, Is.SameAs(testService));

            Mock.Get(customizeBinding).Verify(p => p(It.IsAny<BasicHttpBinding>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void DiscoverNetTcpService_registers_service_for_NetTcp(ServiceCollection services, Action<NetTcpBinding> customizeBinding, ServiceLifetime lifetime, ITestService testService, IDiscoveryService discoveryService)
        {
            services.DiscoverNetTcpService<ITestService>(customizeBinding, lifetime);

            services.AddSingleton<IDiscoveryService>(discoveryService);

            Mock.Get(discoveryService).Setup(p => p.Discover<ITestService>(It.IsAny<Binding>())).Returns(testService);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            Assert.That(service, Is.SameAs(testService));

            Mock.Get(customizeBinding).Verify(p => p(It.IsAny<NetTcpBinding>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void DiscoverNamedPipeService_registers_service_for_NamedPipe(ServiceCollection services, Action<NetNamedPipeBinding> customizeBinding, ServiceLifetime lifetime, ITestService testService, IDiscoveryService discoveryService)
        {
            services.DiscoverNamedPipeService<ITestService>(customizeBinding, lifetime);

            services.AddSingleton<IDiscoveryService>(discoveryService);

            Mock.Get(discoveryService).Setup(p => p.Discover<ITestService>(It.IsAny<Binding>())).Returns(testService);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            Assert.That(service, Is.SameAs(testService));

            Mock.Get(customizeBinding).Verify(p => p(It.IsAny<NetNamedPipeBinding>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void DiscoverWSHttpService_registers_service_for_WSHttp(ServiceCollection services, Action<WSHttpBinding> customizeBinding, ServiceLifetime lifetime, ITestService testService, IDiscoveryService discoveryService)
        {
            services.DiscoverWSHttpService<ITestService>(customizeBinding, lifetime);

            services.AddSingleton<IDiscoveryService>(discoveryService);

            Mock.Get(discoveryService).Setup(p => p.Discover<ITestService>(It.IsAny<Binding>())).Returns(testService);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            Assert.That(service, Is.SameAs(testService));

            Mock.Get(customizeBinding).Verify(p => p(It.IsAny<WSHttpBinding>()), Times.Once());
        }
    }
}