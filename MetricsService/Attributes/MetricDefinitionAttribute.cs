using System;
using System.ComponentModel;
using MetricsService.Models;

namespace MetricsService.Attributes;

/// <summary>
/// Attribute, for use 1 or more times on an Enum member, to assign Metric Name, Type, and Description information.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MetricDefinitionAttribute : DescriptionAttribute
{
    /// <summary>
    /// Metric Definition Attribute
    /// </summary>
    /// <param name="type">Instrument Type</param>
    /// <param name="metricNameSuffix">Metric Name Suffix</param>
    /// <param name="description">Metric Description</param>
    public MetricDefinitionAttribute(InstrumentType type, string metricNameSuffix, string description) : base(description)
    {
        Type = type;
        MetricNameSuffix = metricNameSuffix;
    }

    /// <summary>
    /// Instrument Type
    /// </summary>
    public InstrumentType Type { get; set; }

    /// <summary>
    /// Metric Name Suffix
    /// </summary>
    public string MetricNameSuffix { get; set; }

    /// <summary>
    /// Unit to use with Meter.
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Required Tag Names (case-sensitive)
    /// </summary
    /// <remarks>Only applies to additional tags that can be sent in and not Tags defined in <see cref="MetricTag"/> attributes!</remarks>
    public string[] RequiredTagNames { get; set; } = [];
}