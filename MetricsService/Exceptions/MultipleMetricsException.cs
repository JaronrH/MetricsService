using System;
using System.Collections.Generic;
using MetricsService.Models;

namespace MetricsService.Exceptions;

/// <summary>
/// Multiple Instruments Found for Metric.
/// </summary>
/// <param name="metric">Metric</param>
/// <param name="instrumentTypesFound">Instrument Types Found for Metric</param>
public class MultipleMetricsException(object metric, IEnumerable<InstrumentType> instrumentTypesFound) : ArgumentException($"Multiple Instruments found for Metric '{metric}'.")
{
    /// <summary>
    /// Metric
    /// </summary>
    public object Metric { get; set; } = metric;

    /// <summary>
    /// Instrument Types Found for Metric
    /// </summary>

    public IEnumerable<InstrumentType> InstrumentTypesFound { get; set; } = instrumentTypesFound;
}