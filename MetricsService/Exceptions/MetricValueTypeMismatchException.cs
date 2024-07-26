using MetricsService.Attributes;
using System;

namespace MetricsService.Exceptions;

/// <summary>
/// Metric Value Type Mismatch Exception.
/// </summary>
/// <param name="enumType">Enum Type that is decorated with <see cref="MeterDefinitionAttribute"/>.</param>
/// <param name="attributeMetricValueType">Metric Value Type as defined by <see cref="MeterDefinitionAttribute"/>.</param>
/// <param name="serviceMetricValueType">Metric Value Type the <see cref="MetricsService{TMetricEnumType}"/> was created with.</param>
public class MetricValueTypeMismatchException(Type enumType, Type attributeMetricValueType, Type serviceMetricValueType) : Exception($"Type '{enumType}' specifies a MetricValueType of '{attributeMetricValueType}' but this service was created with a MetricValueType of '{serviceMetricValueType}'.")
{
    /// <summary>
    /// Enum Type that is decorated with <see cref="MeterDefinitionAttribute"/>.
    /// </summary>
    public Type EnumType { get; set; } = enumType;

    /// <summary>
    /// Metric Value Type as defined by <see cref="MeterDefinitionAttribute"/>.
    /// </summary>
    public Type AttributeMetricValueType { get; set; } = attributeMetricValueType;

    /// <summary>
    /// Metric Value Type the <see cref="MetricsService{TMetricEnumType}"/> was created with.
    /// </summary>
    public Type ServiceMetricValueType { get; set; } = serviceMetricValueType;
}
