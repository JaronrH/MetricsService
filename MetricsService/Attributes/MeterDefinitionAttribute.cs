using System;
using System.ComponentModel;

namespace MetricsService.Attributes;

/// <summary>
/// Add Meter Information to Enum.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class MeterDefinitionAttribute : DisplayNameAttribute
{
    /// <summary>
    /// Meter Definition
    /// </summary>
    /// <param name="meterName">Meter Name</param>
    /// <param name="version">Optional Meter Version</param>
    /// <param name="metricValueType">Optional Type used for defining the Meter Service through DI Scanning (defaults to <see cref="long"/> when null).</param>
    public MeterDefinitionAttribute(string meterName, string? version = null, Type? metricValueType = null) : base(meterName)
    {
        Version = version;
        MetricValueType = metricValueType ?? typeof(long);
    }

    /// <summary>
    /// Meter Definition
    /// </summary>
    /// <param name="meterName"></param>
    /// <param name="metricValueType"></param>
    public MeterDefinitionAttribute(string meterName, Type metricValueType) : base(meterName)
    {
        MetricValueType = metricValueType;
    }

    /// <summary>
    /// Meter Version
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// If defined, this will auto-register this Meter in DI using this type as the implementing type.
    /// </summary>
    public Type MetricValueType { get; set; }

    /// <summary>
    /// Required Tag Names for all Metrics (case-sensitive)
    /// </summary>
    /// <remarks>Only applies to additional tags that can be sent in and not Tags defined in <see cref="MetricTag"/> attributes!</remarks>
    public string[] RequiredTagNames { get; set; } = [];
}