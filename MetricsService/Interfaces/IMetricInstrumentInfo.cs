using System;
using System.Collections.Generic;
using MetricsService.Models;

namespace MetricsService.Interfaces;

public interface IMetricInstrumentInfo<out TMetricEnumType> 
    where TMetricEnumType : struct, Enum
{
    /// <summary>
    /// Metric Identifier
    /// </summary>
    TMetricEnumType Metric { get; }

    /// <summary>
    /// Instrument Type
    /// </summary>
    InstrumentType InstrumentType { get; }

    /// <summary>
    /// Metric Name Suffix
    /// </summary>
    string MetricNameSuffix { get; }

    /// <summary>
    /// Full Metric Name
    /// </summary>
    string MetricName { get; }

    /// <summary>
    /// Optional Unit
    /// </summary>
    string? Unit { get; }

    /// <summary>
    /// Optional Description
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Optional Required Tag Key's (case-sensitive)
    /// </summary>
    string[] RequiredTagNames { get; }

    /// <summary>
    /// Static Tags to be added during the creation of the Instrument.
    /// </summary>
    IReadOnlyCollection<KeyValuePair<string,object?>> Tags { get; }
}