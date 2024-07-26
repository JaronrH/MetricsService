# Metrics Service

This library is designed to help speed up the process of creating OpenTelemetry Metrics and services with Microsoft's Dependency Injection. 

## Features

 - Uses decorated Enums to define Metrics.
 - Supports the creation of Counter, Up/Down, and Histogram metrics (you will need to define your own boundaries).
 - Can create an *IDisposable* to handle Up/Down counters that will increase the count on creation and decrease it when *Dispose()* is called.
 - Can register *IMetricsService<EnumType, MetricValueType>* services by simply calling *AddMetricsService< EnumType >()*.  Can automatically register the metrics in OpenTelemetry by passing true as the parameter.
 - Can have mandatory tags defined as well as create Metrics and Meters with tags already specified.
 - Overridable functions in Service for custom implementations.

## Example

Metric Enumeration:

    /// <summary>
    /// Test Metric
    /// </summary>
    [MeterDefinition("My.Meter.Namespace", "beta-1.0", RequiredTagNames = ["Tag1", "Tag2"])]
    [MetricTag("MyMetricTag", "MyMetricTagValue")]
    public enum MyMetricLong
    {
        [MetricDefinition(InstrumentType.Counter, "CounterMetric", "Test Counter Metric", Unit = "MadeUpUnit", RequiredTagNames = ["CounterName"])]
        CounterMetric,
     
        [MetricDefinition(InstrumentType.UpDownCounter, "UpDownMetric", "Test Up/Down Metric")]
        [MetricTag("CounterName", "UpDownMetric")]
        UpDownMetric,
     
        [MetricDefinition(InstrumentType.Histogram, "HistogramMetric", "Test Histogram Metric")]
        HistogramMetric,
     
        [MetricDefinition(InstrumentType.Counter, "EverythingCounterMetric", "Test Everything Counter Metric", RequiredTagNames = ["CounterName"])]
        [MetricDefinition(InstrumentType.UpDownCounter, "EverythingUpDownMetric", "Test Everything Up/Down Metric")]
        [MetricDefinition(InstrumentType.Histogram, "EverythingHistogramMetric", "Test Everything Histogram Metric")]
        [MetricTag("EverythingMetric", true)]
        EverythingMetric
    }
To add the metrics to DI:

    services.AddMetricsService<MyMetricLong>(true)

This will:

 - Add a new Meter called "My.Meter.Namespace" as Version 'beta-1.0' and tag 'MyMetricTag='MyMetricTagValue'.  All Metrics will require tags named 'Tag1' and 'Tag2' (or a MissingRequiredTagsException will be thrown).
 - Add services in DI called IMetricsService< MyMetricLong > and IMetricsService< MyMetricLong, long >.

> Note: IMetricsService< MyMetricLong > is only created when the default MetricDataType is used.  The MeterDefinitionAttribute can create Metrics that use any of the supported metric types (int, long, decimal, double, float, or byte).

 - Create the following Metrics:
 
| Name | Description | Created With Tags| Required Tags | 
|--|--|--|--|
| my.meter.namespace.counterMetric | Test Counter Metric |"CounterName"="UpDownMetric"| "Tag1", "Tag2", "CounterName"|
| my.meter.namespace.upDownMetric | Test Up/Down Metric ||"Tag1", "Tag2" |
| my.meter.namespace.histogramMetric | Test Histogram Metric ||"Tag1", "Tag2" |
| my.meter.namespace.everythingCounterMetric | Test Everything Counter Metric |"EverythingMetric"=true|"Tag1", "Tag2", "CounterName" |
| my.meter.namespace.everythingUpDownMetric | Test Everything Up/Down Metric |"EverythingMetric"=true|"Tag1", "Tag2" |
| my.meter.namespace.everythingHistogramMetric |Test Everything Histogram Metric  |"EverythingMetric"=true |"Tag1", "Tag2"|

> Note: Name, be default, combines the Meter name with the Metric Name Suffix using dot-notation and camelCase. 

## Overrides
MetricsService<,> can be used as a base class and have various parts of the process customized.  
|Name|Description  |
|--|--|
| InitializeMetricInstruments | Called by Constructor to create Instruments.  By default, this loops through all the Enum values, loads the Attributes, and calls AddMetricInstrumentName() and CreateMetricInstrument() for each. |
|AddMetricInstrumentName|Add Metric Instrument's Name to the provided MetricInstrumentInfo and return the updated data.|
|CreateMetricInstrument|Takes the Instrument Data, Creates the Instrument(s) and adds them to the Instruments ConcurrentDictionary.|
|AssertAmount|Used to validate the amount sent to the Inc/Dec calls (Instrument information is provided).|