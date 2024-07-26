using System;
using MetricsService.Attributes;

namespace MetricsService.Exceptions;

/// <summary>
/// Exception thrown when an <paramref name="enumType"/> is used in the <see cref="MetricsService{TMetricEnumType}"/> that isn't decorated with <see cref="MeterDefinitionAttribute"/>.
/// </summary>
/// <param name="enumType">Enum Type that is missing the <see cref="MeterDefinitionAttribute"/>.</param>
public class MeterAttributeMissingException(Type enumType): Exception($"Type '{enumType}' is not decorated with '{typeof(MeterDefinitionAttribute)}'.")
{
    /// <summary>
    /// Enum Type that is missing the <see cref="MeterDefinitionAttribute"/>.
    /// </summary>
    public Type EnumType { get; set; } = enumType;
}