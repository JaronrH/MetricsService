using System;

namespace MetricsService.Attributes;

/// <summary>
/// Metric Tag to always be added to the Meter and/or Metric it is defined with.  Can have multiple Attributes.
/// </summary>
/// <remarks>If used with <see cref="MeterDefinitionAttribute"/>, it will apply to the Meter creation while using it with the <see cref="MetricDefinitionAttribute"/> will add that tag to the Instrument creation.</remarks>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MetricTagAttribute(string name, object? value) : Attribute
{
    /// <summary>
    /// Tag Name
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Tag Value
    /// </summary>
    public object? Value { get; set; } = value;
}