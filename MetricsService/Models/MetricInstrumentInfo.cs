using System;
using System.Collections.Generic;
using MetricsService.Interfaces;

namespace MetricsService.Models;

/// <summary>
/// Metric Instrument Information
/// </summary>
/// <typeparam name="TMetricEnumType">Enum where the values are the keys defining metrics to be created/handled.</typeparam>
public class MetricInstrumentInfo<TMetricEnumType> : IMetricInstrumentInfo<TMetricEnumType> 
    where TMetricEnumType : struct, Enum
{
    /// <summary>
    /// Metric Instrument Information
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enum where the values are the keys defining metrics to be created/handled.</typeparam>
    /// <param name="metric">Metric Identifier</param>
    /// <param name="instrumentType">Instrument Type</param>
    /// <param name="metricNameSuffix">Metric Name Suffix</param>
    /// <param name="metricName">Metric Name</param>
    public MetricInstrumentInfo(TMetricEnumType metric,
        InstrumentType instrumentType,
        string metricNameSuffix,
        string metricName = "")
    {
        Metric = metric;
        InstrumentType = instrumentType;
        MetricNameSuffix = metricNameSuffix;
        MetricName = metricName;
    }

    /// <summary>
    /// Metric Instrument Information
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enum where the values are the keys defining metrics to be created/handled.</typeparam>
    public MetricInstrumentInfo() {}

    /// <summary>
    /// Metric Identifier
    /// </summary>
    public TMetricEnumType Metric { get; set; }

    /// <summary>
    /// Instrument Type
    /// </summary>
    public InstrumentType InstrumentType { get; set; }

    /// <summary>
    /// Metric Name Suffix
    /// </summary>
    public string MetricNameSuffix { get; set; } = null!;

    /// <summary>
    /// Full Metric Name
    /// </summary>
    public string MetricName { get; set; } = null!;

    /// <summary>
    /// Optional Unit
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Optional Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional Required Tag Key's (case-sensitive)
    /// </summary>
    public string[] RequiredTagNames { get; set; } = [];

    /// <summary>
    /// Static Tags to be added during the creation of the Instrument.
    /// </summary>
    public IReadOnlyCollection<KeyValuePair<string, object?>> Tags { get; set; } = [];

    #region Overrides of Object

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"Metric {InstrumentType.GetDisplayName()} Instrument Info for '{Metric.GetDisplayName()}'.";

    #endregion
}