using System;
using System.Reflection;
using MetricsService;
using MetricsService.Attributes;
using MetricsService.Exceptions;
using MetricsService.Interfaces;
using OpenTelemetry.Metrics;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Service/Services for Metrics from <typeparamref name="TMetricEnumType"/> using information in the decorating <see cref="MeterDefinitionAttribute"/>.
    /// </summary>
    /// <remarks>If the MetricValueType is Long, will also register the service <see cref="IMetricsService{TMetricEnumType}"/> in addition to the appropriate <see cref="IMetricsService{TMetricEnumType, Type}"/>.</remarks>
    /// <typeparam name="TMetricEnumType">Enum Type that defines the Metrics.</typeparam>
    /// <param name="services">Service Provider</param>
    /// <param name="registerMeterInOpenTelemetry">If true, will also add Metrics Meter to OpenTelemetry.</param>
    /// <returns>Service Provider</returns>
    public static IServiceCollection AddMetricsService<TMetricEnumType>(this IServiceCollection services, bool registerMeterInOpenTelemetry) 
        where TMetricEnumType : struct, Enum
    {
        // Add Services for Metrics
        var type = typeof(TMetricEnumType);
        var attr = type.GetCustomAttribute<MeterDefinitionAttribute>() ?? throw new MeterAttributeMissingException(type);
        if (attr.MetricValueType == typeof(long))
        {
            services.AddSingleton<IMetricsService<TMetricEnumType>, MetricsService<TMetricEnumType>>();
            services.AddSingleton<IMetricsService<TMetricEnumType, long>>(s => s.GetRequiredService<IMetricsService<TMetricEnumType>>());
        }
        else
        {
            var interfaceType = typeof(IMetricsService<,>).MakeGenericType(type, attr.MetricValueType);
            var implementationType = typeof(MetricsService<,>).MakeGenericType(type, attr.MetricValueType);
            services.AddSingleton(interfaceType, implementationType);
        }

        // Register Metrics Meter?
        if (registerMeterInOpenTelemetry)
            services
                .AddOpenTelemetry()
                .WithMetrics(b => b.AddMeter<TMetricEnumType>());

        return services;
    }
}