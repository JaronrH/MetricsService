using System;
using System.Collections.Generic;
using System.Linq;
using MetricsService.Models;

// ReSharper disable once CheckNamespace
namespace MetricsService.Interfaces;

public static class MetricsServiceExtensions
{
    /// <summary>
    /// Increment Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
    /// <typeparam name="TMetricValueType">Metric Value to use.</typeparam>
    /// <param name="service">Metrics Service</param>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public static IMetricsService<TMetricEnumType, TMetricValueType> IncCounter<TMetricEnumType, TMetricValueType>(
        this IMetricsService<TMetricEnumType, TMetricValueType> service, 
        TMetricEnumType metricType, 
        params KeyValuePair<string, object?>[] tags
        ) 
        where TMetricEnumType : struct, Enum
        where TMetricValueType : struct
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        return service.IncCounter(metricType, null, tags);
    }

    /// <summary>
    /// Increment Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
    /// <typeparam name="TMetricValueType">Metric Value to use.</typeparam>
    /// <param name="service">Metrics Service</param>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="instrumentType">Instrument Type</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public static IMetricsService<TMetricEnumType, TMetricValueType> IncCounter<TMetricEnumType, TMetricValueType>(
        this IMetricsService<TMetricEnumType, TMetricValueType> service, 
        TMetricEnumType metricType, InstrumentType instrumentType, 
        params KeyValuePair<string, object?>[] tags
        )
        where TMetricEnumType : struct, Enum
        where TMetricValueType : struct
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        return service.IncCounter(metricType, instrumentType, null, tags);
    }

    /// <summary>
    /// Decrement Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
    /// <typeparam name="TMetricValueType">Metric Value to use.</typeparam>
    /// <param name="service">Metrics Service</param>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public static IMetricsService<TMetricEnumType, TMetricValueType> DecCounter<TMetricEnumType, TMetricValueType>(
        this IMetricsService<TMetricEnumType, TMetricValueType> service, 
        TMetricEnumType metricType, 
        params KeyValuePair<string, object?>[] tags
        ) 
        where TMetricEnumType : struct, Enum
        where TMetricValueType : struct
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        return service.DecCounter(metricType, null, tags);
    }

    /// <summary>
    /// Decrement Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
    /// <typeparam name="TMetricValueType">Metric Value to use.</typeparam>
    /// <param name="service">Metrics Service</param>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="instrumentType">Instrument Type</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public static IMetricsService<TMetricEnumType, TMetricValueType> DecCounter<TMetricEnumType, TMetricValueType>(
        this IMetricsService<TMetricEnumType, TMetricValueType> service, 
        TMetricEnumType metricType,
        InstrumentType instrumentType,
        params KeyValuePair<string, object?>[] tags
        )
        where TMetricEnumType : struct, Enum
        where TMetricValueType : struct
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        return service.DecCounter(metricType, instrumentType, null, tags);
    }

    /// <summary>
    /// Increment an Up/Down Counter then return an <see cref="IDisposable"/> that will decrement the counter when disposed.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
    /// <typeparam name="TMetricValueType">Metric Value to use.</typeparam>
    /// <param name="service">Metrics Service</param>
    /// <param name="metricType">Metric Type that is an Up/Down Counter.</param>
    /// <param name="decTagsFunc">Optional function, that is called when the returned <see cref="IDisposable"/> object is disposed, that will create the tags to attach to the DecCounter call.</param>
    /// <param name="incTags">Tags to use on the IncCounter call.</param>
    /// <returns><see cref="IDisposable"/> that will decrement the counter when disposed.</returns>
    /// <exception cref="ArgumentNullException">Service is Null</exception>
    /// <exception cref="ArgumentException">Invalid Metric Type</exception>
    public static IDisposable CreateDisposableCounter<TMetricEnumType, TMetricValueType>(
        this IMetricsService<TMetricEnumType, TMetricValueType> service,
        TMetricEnumType metricType, 
        Func<IEnumerable<KeyValuePair<string, object?>>>? decTagsFunc = null, 
        params KeyValuePair<string, object?>[] incTags
        ) 
        where TMetricEnumType : struct, Enum
        where TMetricValueType : struct
    {
        if (service == null) throw new ArgumentNullException(nameof(service));

        // Validate Metric Type
        if (!service.Instruments.Any(i => i.InstrumentType == InstrumentType.UpDownCounter && i.Metric.Equals(metricType)))
            throw new ArgumentException($"Metric '{metricType.GetDisplayName()}' does not support instrument type '{InstrumentType.UpDownCounter.GetDisplayName()}'.");

        // Increment Counter Now
        service.IncCounter(metricType, InstrumentType.UpDownCounter, null, incTags);

        // Create Disposable that will handle the decrement
        return OnDispose.Synchronous(() => service.DecCounter(metricType, InstrumentType.UpDownCounter, null, decTagsFunc?.Invoke().ToArray() ?? []));
    }

    /// <summary>
    /// Increment an Up/Down Counter then return an <see cref="IDisposable"/> that will decrement the counter when disposed.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enumeration that defines the Metrics.</typeparam>
    /// <typeparam name="TMetricValueType">Metric Value to use.</typeparam>
    /// <param name="service">Metrics Service</param>
    /// <param name="metricType">Metric Type that is an Up/Down Counter.</param>
    /// <param name="tags">Tags to include on both the IncCounter and DecCounter calls.</param>
    /// <returns><see cref="IDisposable"/> that will decrement the counter when disposed.</returns>
    /// <exception cref="ArgumentNullException">Service is Null</exception>
    /// <exception cref="ArgumentException">Invalid Metric Type</exception>
    public static IDisposable CreateDisposableCounter<TMetricEnumType, TMetricValueType>(
        this IMetricsService<TMetricEnumType, TMetricValueType> service, 
        TMetricEnumType metricType,
        params KeyValuePair<string, object?>[] tags
        ) 
        where TMetricEnumType : struct, Enum
        where TMetricValueType : struct 
        => CreateDisposableCounter(service, metricType, () => tags, tags);
}