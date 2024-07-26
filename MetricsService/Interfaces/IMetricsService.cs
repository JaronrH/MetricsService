using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using MetricsService.Models;

namespace MetricsService.Interfaces;

/// <summary>
/// Metrics Service that uses <see cref="long"/> as the TMetricValueType
/// </summary>
/// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
public interface IMetricsService<TMetricEnumType> : IMetricsService<TMetricEnumType, long> 
    where TMetricEnumType : struct, Enum { }

/// <summary>
/// Metrics Service
/// </summary>
/// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
/// <typeparam name="TMetricValueType">Metric Value to use.</typeparam>
public interface IMetricsService<TMetricEnumType, TMetricValueType> : IDisposable 
    where TMetricEnumType : struct, Enum 
    where TMetricValueType : struct
{

    /// <summary>
    /// Meter Instance
    /// </summary>
    Meter Meter { get; }

    /// <summary>
    /// Instrument Definitions
    /// </summary>
    IReadOnlyCollection<IMetricInstrumentInfo<TMetricEnumType>> Instruments { get; }

    /// <summary>
    /// Increment Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="amount">Amount to Increment Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    IMetricsService<TMetricEnumType, TMetricValueType> IncCounter(TMetricEnumType metricType, TMetricValueType? amount = null, params KeyValuePair<string, object?>[] tags);

    /// <summary>
    /// Increment Counter by an Amount.
    /// </summary>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="instrumentType">Instrument Type</param>
    /// <param name="amount">Amount to Increment Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    IMetricsService<TMetricEnumType, TMetricValueType> IncCounter(TMetricEnumType metricType, InstrumentType instrumentType, TMetricValueType? amount = null, params KeyValuePair<string, object?>[] tags);

    /// <summary>
    /// Decrement Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <param name="metricType">Metric Type to Decrement.</param>
    /// <param name="amount">Amount to Decrement Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    IMetricsService<TMetricEnumType, TMetricValueType> DecCounter(TMetricEnumType metricType, TMetricValueType? amount = null, params KeyValuePair<string, object?>[] tags);

    /// <summary>
    /// Decrement Counter by an Amount.
    /// </summary>
    /// <param name="metricType">Metric Type to Decrement.</param>
    /// <param name="instrumentType">Instrument Type</param>
    /// <param name="amount">Amount to Decrement Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    IMetricsService<TMetricEnumType, TMetricValueType> DecCounter(TMetricEnumType metricType, InstrumentType instrumentType, TMetricValueType? amount = null, params KeyValuePair<string, object?>[] tags);
}