using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using MetricsService.Attributes;
using MetricsService.Exceptions;
using MetricsService.Extensions;
using MetricsService.Interfaces;
using MetricsService.Models;
using Microsoft.Extensions.Logging;

namespace MetricsService;

/// <summary>
/// Metrics Class that uses an Enum, decorated with <see cref="MeterDefinitionAttribute"/> and values decorated by <see cref="MetricDefinitionAttribute"/>, to define the metrics.
/// </summary>
/// <typeparam name="TMetricEnumType">Enum where the values are the keys defining metrics to be created/handled.</typeparam>
public class MetricsService<TMetricEnumType>(IMeterFactory meterFactory, ILogger<MetricsService<TMetricEnumType, long>> log) : MetricsService<TMetricEnumType, long>(meterFactory, log), IMetricsService<TMetricEnumType>
    where TMetricEnumType : struct, Enum
{
    #region Overrides of MetricsService<TMetricEnumType,long>

    /// <summary>
    /// Assert that the provided amount is valid and adjust/set default as needed.
    /// </summary>
    /// <example>If null, set value to 1. Otherwise, make sure value is greater than or equal to 0</example>
    /// <param name="amount">Amount</param>
    /// <param name="metricInstrumentInfo">Metric Instrument Information this amount is for.</param>
    /// <param name="makeDecAmount">If true, value should be made negative for use in DecCounter()</param>
    /// <returns>Actual Amount to use.</returns>
    protected override long AssertAmount(long? amount, IMetricInstrumentInfo<TMetricEnumType> metricInstrumentInfo, bool makeDecAmount = false)
    {
        amount ??= 1;
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        return makeDecAmount
            ? -1L * amount.Value
            : amount.Value;
    }

    #endregion
}

/// <summary>
/// Metrics Class that uses an Enum, decorated with <see cref="MeterDefinitionAttribute"/> and values decorated by <see cref="MetricDefinitionAttribute"/>, to define the metrics.
/// </summary>
/// <typeparam name="TMetricEnumType">Enum where the values are the keys defining metrics to be created/handled.</typeparam>
/// <typeparam name="TMetricValueType">Instrument Type to use.</typeparam>
public class MetricsService<TMetricEnumType, TMetricValueType> : IMetricsService<TMetricEnumType, TMetricValueType> 
    where TMetricEnumType : struct, Enum 
    where TMetricValueType : struct
{
    public MetricsService(IMeterFactory meterFactory, ILogger<MetricsService<TMetricEnumType, TMetricValueType>> log)
    {
        Log = log;
        try
        {
            var attr = typeof(TMetricEnumType).GetCustomAttribute<MeterDefinitionAttribute>() ?? throw new MeterAttributeMissingException(typeof(TMetricEnumType));
            if (attr.MetricValueType != null && attr.MetricValueType != typeof(TMetricValueType))
                throw new MetricValueTypeMismatchException(typeof(TMetricEnumType), attr.MetricValueType, typeof(TMetricValueType));
            var tags = typeof(TMetricEnumType)
                .GetCustomAttributes<MetricTagAttribute>()
                .Select(i => new KeyValuePair<string, object?>(i.Name, i.Value))
                .ToArray();
            Meter = meterFactory.Create(attr.DisplayName, attr.Version, tags);
            // ReSharper disable once VirtualMemberCallInConstructor
            InitializeMetricInstruments();
        }
        catch (Exception e)
        {
            Log.LogError(e, $"Unable to initialize Metric Service for '{typeof(TMetricEnumType)}'.");
            throw;
        }
    }

    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Log { get; private set; }

    /// <summary>
    /// Initialize Metric Instruments in the Instruments field.
    /// Base implementation loops through all the values in <see cref="TMetricEnumType"/> and calls CreateMetricInstrument() for each Enum value and Each <see cref="MetricDefinitionAttribute"/> on that value.
    /// </summary>
    /// <remarks>Called by Constructor.</remarks>
    protected virtual void InitializeMetricInstruments()
    {
        var attr = typeof(TMetricEnumType).GetCustomAttribute<MeterDefinitionAttribute>()!;
        var type = typeof(TMetricEnumType);
        foreach (var metricEnum in Enum.GetValues(typeof(TMetricEnumType)).Cast<TMetricEnumType>())
        foreach (var metricDefinition in type
                     .GetMember(metricEnum.ToString())
                     .First()
                     .GetCustomAttributes(typeof(MetricDefinitionAttribute))
                     .Cast<MetricDefinitionAttribute>()
                )
            CreateMetricInstrument(AddMetricInstrumentName(
                    new MetricInstrumentInfo<TMetricEnumType>(metricEnum, metricDefinition.Type, metricDefinition.MetricNameSuffix)
                    {
                        Unit = metricDefinition.Unit,
                        Description = metricDefinition.Description,
                        RequiredTagNames = attr.RequiredTagNames.Concat(metricDefinition.RequiredTagNames).ToArray(),
                        Tags = type
                            .GetMember(metricEnum.ToString())
                            .First()
                            .GetCustomAttributes<MetricTagAttribute>()
                            .Select(i => new KeyValuePair<string, object?>(i.Name, i.Value))
                            .ToArray()
                    }
                )
            );
    }

    /// <summary>
    /// Creates a new Instrument and stores it in Instruments.
    /// </summary>
    /// <param name="metricInstrumentInfo">Metric Instrument Info</param>
    protected virtual void CreateMetricInstrument(MetricInstrumentInfo<TMetricEnumType> metricInstrumentInfo)
    {
        if (metricInstrumentInfo == null) throw new ArgumentNullException(nameof(metricInstrumentInfo));
        if (string.IsNullOrEmpty(metricInstrumentInfo.MetricName))
            throw new ArgumentNullException(nameof(metricInstrumentInfo.MetricName), "Metric Name cannot be null/empty.");
        Instruments.TryAdd(metricInstrumentInfo, metricInstrumentInfo.InstrumentType switch
        {
            InstrumentType.Counter => Meter.CreateCounter<TMetricValueType>(metricInstrumentInfo.MetricName, metricInstrumentInfo.Unit, metricInstrumentInfo.Description, metricInstrumentInfo.Tags),
            InstrumentType.UpDownCounter => Meter.CreateUpDownCounter<TMetricValueType>(metricInstrumentInfo.MetricName, metricInstrumentInfo.Unit, metricInstrumentInfo.Description, metricInstrumentInfo.Tags),
            InstrumentType.Histogram => Meter.CreateHistogram<TMetricValueType>(metricInstrumentInfo.MetricName, metricInstrumentInfo.Unit, metricInstrumentInfo.Description, metricInstrumentInfo.Tags),
            _ => throw new ArgumentOutOfRangeException()
        });
    }

    /// <summary>
    /// Add Metric Instrument's Name to the provided <paramref name="metricInstrumentInfo"/> and return the updated info.
    /// </summary>
    /// <param name="metricInstrumentInfo">Metric Instrument Info</param>
    /// <returns><see cref="MetricInstrumentInfo{TMetricEnumType}"/> with the MetricName property set.</returns>
    protected virtual MetricInstrumentInfo<TMetricEnumType> AddMetricInstrumentName(MetricInstrumentInfo<TMetricEnumType> metricInstrumentInfo)
    {
        metricInstrumentInfo.MetricName = $"{Meter.Name} {metricInstrumentInfo.MetricNameSuffix}".ToDotNotation(true);
        return metricInstrumentInfo;
    }

    /// <summary>
    /// Internal List of Instruments by Name
    /// </summary>
    protected readonly ConcurrentDictionary<IMetricInstrumentInfo<TMetricEnumType>, Instrument> Instruments = new();

    /// <summary>
    /// Assert that the provided amount is valid and adjust/set default as needed.
    /// </summary>
    /// <example>If null, set value to 1. Otherwise, make sure value is greater than or equal to 0</example>
    /// <param name="amount">Amount</param>
    /// <param name="metricInstrumentInfo">Metric Instrument Information this amount is for.</param>
    /// <param name="makeDecAmount">If true, value should be made negative for use in DecCounter()</param>
    /// <returns>Actual Amount to use.</returns>
    protected virtual TMetricValueType AssertAmount(TMetricValueType? amount, IMetricInstrumentInfo<TMetricEnumType> metricInstrumentInfo, bool makeDecAmount)
    {
        if (typeof(TMetricValueType) == typeof(int))
        {
            if (amount == null) return (TMetricValueType)(object)(makeDecAmount ? -1 : 1);
            if ((int)(object)(amount) <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            return makeDecAmount
                ? (TMetricValueType)(object)(-1 * (int)(object)(amount))
                : amount.Value;
        }
        if (typeof(TMetricValueType) == typeof(long))
        {
            if (amount == null) return (TMetricValueType)(object)(makeDecAmount ? -1L : 1L);
            if ((long)(object)(amount) <= 0L) throw new ArgumentOutOfRangeException(nameof(amount));
            return makeDecAmount
                ? (TMetricValueType)(object)(-1 * (long)(object)(amount))
                : amount.Value;
        }
        if (typeof(TMetricValueType) == typeof(decimal))
        {
            if (amount == null) return (TMetricValueType)(object)(makeDecAmount ? -1m : 1m);
            if ((decimal)(object)(amount) <= 0m) throw new ArgumentOutOfRangeException(nameof(amount));
            return makeDecAmount
                ? (TMetricValueType)(object)(-1m * (decimal)(object)(amount))
                : amount.Value;
        }
        if (typeof(TMetricValueType) == typeof(double))
        {
            if (amount == null) return (TMetricValueType)(object)(makeDecAmount ? -1d : 1d);
            if ((double)(object)(amount) <= 0d) throw new ArgumentOutOfRangeException(nameof(amount));
            return makeDecAmount
                ? (TMetricValueType)(object)(-1d * (double)(object)(amount))
                : amount.Value;
        }
        if (typeof(TMetricValueType) == typeof(float))
        {
            if (amount == null) return (TMetricValueType)(object)(makeDecAmount ? -1f : 1f);
            if ((float)(object)(amount) <= 0f) throw new ArgumentOutOfRangeException(nameof(amount));
            return makeDecAmount
                ? (TMetricValueType)(object)(-1f * (float)(object)(amount))
                : amount.Value;
        }
        if (typeof(TMetricValueType) == typeof(byte))
        {
            if (amount == null) return (TMetricValueType)(object)(makeDecAmount ? -1 : 1);
            if ((byte)(object)(amount) <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            return makeDecAmount
                ? (TMetricValueType)(object)(-1 * (byte)(object)(amount))
                : amount.Value;
        }
        throw new NotImplementedException($"Default Implementation does not support '{typeof(TMetricValueType)}'.");
    }

    /// <summary>
    /// Meter Instance
    /// </summary>
    public Meter Meter { get; protected set; }

    /// <summary>
    /// Instrument Definitions
    /// </summary>
    IReadOnlyCollection<IMetricInstrumentInfo<TMetricEnumType>> IMetricsService<TMetricEnumType, TMetricValueType>.Instruments => Instruments.Keys.ToList().AsReadOnly();

    /// <summary>
    /// Increment Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="amount">Amount to Increment Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public IMetricsService<TMetricEnumType, TMetricValueType> IncCounter(TMetricEnumType metricType, TMetricValueType? amount = null, params KeyValuePair<string, object?>[] tags)
    {
        var instrumentTypes = Instruments
            .Where(i => i.Key.Metric.Equals(metricType))
            .Select(i => i.Key.InstrumentType)
            .ToArray();
        return instrumentTypes.Length switch
        {
            > 1 => throw new MultipleMetricsException(metricType, instrumentTypes),
            0 => throw new ArgumentException($"No Instruments found for Metric '{metricType}'."),
            _ => IncCounter(metricType, instrumentTypes[0], amount, tags)
        };
    }

    /// <summary>
    /// Increment Counter by an Amount.
    /// </summary>
    /// <param name="metricType">Metric Type to Increment.</param>
    /// <param name="instrumentType">Instrument Type</param>
    /// <param name="amount">Amount to Increment Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public IMetricsService<TMetricEnumType, TMetricValueType> IncCounter(TMetricEnumType metricType, InstrumentType instrumentType, TMetricValueType? amount = null, params KeyValuePair<string,object?>[] tags)
    {
        // Find Instrument
        var keyQuery = Instruments
            .Keys
            .Where(i => i.Metric.Equals(metricType) && i.InstrumentType == instrumentType)
            .ToArray();
        if (!keyQuery.Any() || !Instruments.TryGetValue(keyQuery.First(), out var instrument))
            throw new KeyNotFoundException($"There is no metric named '{metricType.GetDisplayName()}' with an instrument type of '{instrumentType}' defined.");
        var key = keyQuery.First();

        // Assert Amount's Value
        amount = AssertAmount(amount, key, false);
        if (!amount.HasValue) throw new ArgumentNullException(nameof(amount));

        // Required Tags?
        if (key.RequiredTagNames.Any())
        {
            var missingTagNames = key.RequiredTagNames.Where(i => tags.All(t => t.Key != i)).ToArray();
            if (missingTagNames.Any())
                throw new MissingRequiredTagsException(missingTagNames);
        }

        // Update Instrument
        switch (instrumentType)
        {
            case InstrumentType.Counter:
                ((Counter<TMetricValueType>)instrument).Add(amount.Value, tags);
                break;
            case InstrumentType.UpDownCounter:
                ((UpDownCounter<TMetricValueType>)instrument).Add(amount.Value, tags);
                break;
            case InstrumentType.Histogram:
                ((Histogram<TMetricValueType>)instrument).Record(amount.Value, tags);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Done!
        return this;
    }

    /// <summary>
    /// Decrement Counter by an Amount.  If there are multiple, compatible Instruments assigned to this Metric, an exception will be thrown.
    /// </summary>
    /// <param name="metricType">Metric Type to Decrement.</param>
    /// <param name="amount">Amount to Decrement Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public IMetricsService<TMetricEnumType, TMetricValueType> DecCounter(TMetricEnumType metricType, TMetricValueType? amount = null, params KeyValuePair<string, object?>[] tags)
    {
        var validInstrumentTypes = new[] {InstrumentType.UpDownCounter};
        var instrumentTypes = Instruments
            .Where(i => i.Key.Metric.Equals(metricType) && validInstrumentTypes.Contains(i.Key.InstrumentType))
            .Select(i => i.Key.InstrumentType)
            .ToArray();
        return instrumentTypes.Length switch
        {
            > 1 => throw new MultipleMetricsException(metricType, instrumentTypes),
            0 => throw new ArgumentException($"No Compatible Instruments found for Metric '{metricType}'."),
            _ => DecCounter(metricType, instrumentTypes[0], amount, tags)
        };
    }

    /// <summary>
    /// Decrement Counter by an Amount.
    /// </summary>
    /// <param name="metricType">Metric Type to Decrement.</param>
    /// <param name="instrumentType">Instrument Type</param>
    /// <param name="amount">Amount to Decrement Counter by.</param>
    /// <param name="tags">Optional Tags to add to counter.</param>
    /// <returns>Metrics Service</returns>
    public IMetricsService<TMetricEnumType, TMetricValueType> DecCounter(TMetricEnumType metricType, InstrumentType instrumentType, TMetricValueType? amount = null, params KeyValuePair<string, object?>[] tags)
    {
        // Find Instrument
        var keyQuery = Instruments
            .Keys
            .Where(i => i.Metric.Equals(metricType) && i.InstrumentType == instrumentType)
            .ToArray();
        if (!keyQuery.Any() || !Instruments.TryGetValue(keyQuery.First(), out var instrument))
            throw new KeyNotFoundException($"There is no metric named '{metricType.GetDisplayName()}' with an instrument type of '{instrumentType}' defined.");
        var key = keyQuery.First();

        // Assert Amount's Value
        amount = AssertAmount(amount, key, true);
        if (!amount.HasValue) throw new ArgumentNullException(nameof(amount));

        // Required Tags?
        if (key.RequiredTagNames.Any())
        {
            var missingTagNames = key.RequiredTagNames.Where(i => tags.All(t => t.Key != i)).ToArray();
            if (missingTagNames.Any())
                throw new MissingRequiredTagsException(missingTagNames);
        }

        // Update Instrument
        switch (instrumentType)
        {
            case InstrumentType.Counter:
            case InstrumentType.Histogram:
                throw new ArgumentException($"Metric '{metricType.GetDisplayName()}' is not an {InstrumentType.UpDownCounter.GetDisplayName()}.");
            case InstrumentType.UpDownCounter:
                ((UpDownCounter<TMetricValueType>)instrument).Add(amount.Value, tags);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Done!
        return this;
    }

    #region IDisposable

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Meter.Dispose();
    }

    #endregion
}