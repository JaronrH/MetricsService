using System;
using System.Reflection;
using MetricsService.Attributes;
using MetricsService.Exceptions;

// ReSharper disable once CheckNamespace
namespace OpenTelemetry.Metrics;

public static class MeterProviderBuilderExtensions
{
    /// <summary>
    /// Add Metrics from <typeparamref name="TMetricEnumType"/> using information in the decorating <see cref="MeterDefinitionAttribute"/>.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enum Type that defines the Metrics.</typeparam>
    /// <param name="builder">Meter Provider Builder</param>
    public static MeterProviderBuilder AddMeter<TMetricEnumType>(this MeterProviderBuilder builder) 
        where TMetricEnumType : struct
    {
        var type = typeof(TMetricEnumType);
        var attr = type.GetCustomAttribute<MeterDefinitionAttribute>();
        if (attr == null)
            throw new MeterAttributeMissingException(type);
        builder.AddMeter(attr.DisplayName);

        return builder; }
}