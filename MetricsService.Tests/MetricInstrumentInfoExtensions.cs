using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using FluentAssertions;
using MetricsService.Interfaces;

namespace MetricsService.Tests;

public static class MetricInstrumentInfoExtensions
{
    public static MetricInstrumentAssertions<TMetricEnumType> Should<TMetricEnumType>(this IMetricInstrumentInfo<TMetricEnumType> instance) 
        where TMetricEnumType : struct, Enum 
        => new(instance);
}

public class MetricInstrumentAssertions<TMetricEnumType>(IMetricInstrumentInfo<TMetricEnumType> instance) :
    ReferenceTypeAssertions<IMetricInstrumentInfo<TMetricEnumType>, MetricInstrumentAssertions<TMetricEnumType>>(instance)
    where TMetricEnumType : struct, Enum
{
    protected override string Identifier => $"IMetricInstrumentInfo<{typeof(TMetricEnumType).Name}>";

    /// <summary>
    /// Make sure two <see cref="IMetricInstrumentInfo{TMetricEnumType}"/> objects have the same values.
    /// </summary>
    /// <param name="expected">Expected Value to Compare to.</param>
    [CustomAssertion]
    public AndConstraint<MetricInstrumentAssertions<TMetricEnumType>> Be(IMetricInstrumentInfo<TMetricEnumType>? expected)
    {
        using (new AssertionScope())
        {
            if (expected == null)
                Subject.Should().BeNull("Expected value is null.");
            else
            {
                Subject
                    .Should().NotBeNull("Expected value is not null.");
                Subject.Metric
                    .Should().Be(expected.Metric);
                Subject.InstrumentType
                    .Should().Be(expected.InstrumentType);
                Subject.MetricNameSuffix
                    .Should().Be(expected.MetricNameSuffix);
                Subject.MetricName
                    .Should().Be(expected.MetricName);
                Subject.Unit
                    .Should().Be(expected.Unit);
                Subject.Description
                    .Should().Be(expected.Description);
                Subject.RequiredTagNames
                    .Should().HaveCount(expected.RequiredTagNames.Length)
                    .And.Contain(expected.RequiredTagNames);
                if (expected.Tags.Any())
                    Subject.Tags
                        .Should().HaveCount(expected.Tags.Count)
                        .And.Contain(expected.Tags);
                else
                    Subject.Tags
                        .Should().BeEmpty();
            }
        }
        return new AndConstraint<MetricInstrumentAssertions<TMetricEnumType>>(this);
    }
}