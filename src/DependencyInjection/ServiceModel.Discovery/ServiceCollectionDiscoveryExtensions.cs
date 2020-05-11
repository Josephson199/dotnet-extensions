﻿// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EMG.Extensions.DependencyInjection.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionDiscoveryExtensions
    {
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, IConfigurationSection configurationSection, Action<ServiceModelDiscoveryOptions> configureOptions = null)
        {
            services.TryAddSingleton<IDiscoveryService, ServiceModelDiscoveryService>();

            services.Configure<ServiceModelDiscoveryOptions>(configurationSection);

            if (configureOptions != null)
            {
                services.Configure<ServiceModelDiscoveryOptions>(configureOptions);
            }

            return services;
        }

        public static IServiceCollection DiscoverService<TService>(this IServiceCollection services, Func<Binding> bindingFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Transient) where TService : class
        {
            services.Add(ServiceDescriptor.Describe(typeof(TService), ResolveService, serviceLifetime));

            return services;

            object ResolveService(IServiceProvider serviceProvider)
            {
                var discoveryService = serviceProvider.GetRequiredService<IDiscoveryService>();

                var binding = bindingFactory();

                var service = discoveryService.Discover<TService>(binding);

                return service;
            }
        }

        public static IServiceCollection DiscoverBasicHttpService<TService>(this IServiceCollection services, Action<BasicHttpBinding> customizeBinding = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TService : class
        {
            return services.DiscoverService<TService>(CreateBinding, serviceLifetime);

            BasicHttpBinding CreateBinding()
            {
                var binding = new BasicHttpBinding();

                customizeBinding?.Invoke(binding);

                return binding;
            }
        }

        public static IServiceCollection DiscoverWSHttpService<TService>(this IServiceCollection services, Action<WSHttpBinding> customizeBinding = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TService : class
        {
            return services.DiscoverService<TService>(CreateBinding, serviceLifetime);

            WSHttpBinding CreateBinding()
            {
                var binding = new WSHttpBinding();

                customizeBinding?.Invoke(binding);

                return binding;
            }
        }

        public static IServiceCollection DiscoverNetTcpService<TService>(this IServiceCollection services, Action<NetTcpBinding> customizeBinding = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TService : class
        {
            return services.DiscoverService<TService>(CreateBinding, serviceLifetime);

            NetTcpBinding CreateBinding()
            {
                var binding = new NetTcpBinding();

                customizeBinding?.Invoke(binding);

                return binding;
            }
        }

        public static IServiceCollection DiscoverNamedPipeService<TService>(this IServiceCollection services, Action<NetNamedPipeBinding> customizeBinding = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TService : class
        {
            return services.DiscoverService<TService>(CreateBinding, serviceLifetime);

            NetNamedPipeBinding CreateBinding()
            {
                var binding = new NetNamedPipeBinding();

                customizeBinding?.Invoke(binding);

                return binding;
            }
        }
    }
}
