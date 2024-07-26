using System.Diagnostics.Metrics;
using MetricsService.Attributes;
using MetricsService.Interfaces;
using MetricsService.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using FluentAssertions;
using MetricsService.Exceptions;
using OpenTelemetry.Metrics;

namespace MetricsService.Tests;

public class MetricServiceTests
{
    /// <summary>
    /// Create Mocker for <see cref="MetricsService{TMetricEnumType, TMetricValueType}"/>.
    /// </summary>
    /// <typeparam name="TMetricEnumType">Enum where the values are the keys defining metrics to be created/handled.</typeparam>
    /// <typeparam name="TMetricValueType">Instrument Type to use.</typeparam>
    private AutoMocker CreateMocker<TMetricEnumType, TMetricValueType>()
        where TMetricEnumType : struct, Enum 
        where TMetricValueType : struct
    {
        var mocker = new AutoMocker();

        // Logger Mock
        mocker.Use(new Mock<ILogger<MetricsService<TMetricEnumType, TMetricValueType>>>());

        // Meter Factory
        mocker
            .GetMock<IMeterFactory>()
            .Setup(i => i.Create(It.IsAny<MeterOptions>()))
            .Returns((MeterOptions o) => new Meter(o));

        // Return Mocker
        return mocker;
    }

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

    /// <summary>
    /// Test Metric
    /// </summary>
    [MeterDefinition("My.Meter.Namespace", "beta-1.0", typeof(int), RequiredTagNames = ["Tag1", "Tag2"])]
    [MetricTag("MyMetricTag", "MyMetricTagValue")]
    public enum MyMetricInt
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

    /// <summary>
    /// Test Metric
    /// </summary>
    [MeterDefinition("My.Meter.Namespace", "beta-1.0", typeof(double), RequiredTagNames = ["Tag1", "Tag2"])]
    [MetricTag("MyMetricTag", "MyMetricTagValue")]
    public enum MyMetricDouble
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

    /// <summary>
    /// Test Metric
    /// </summary>
    [MeterDefinition("My.Meter.Namespace", "beta-1.0", typeof(decimal), RequiredTagNames = ["Tag1", "Tag2"])]
    [MetricTag("MyMetricTag", "MyMetricTagValue")]
    public enum MyMetricDecimal
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

    /// <summary>
    /// Test Metric
    /// </summary>
    [MeterDefinition("My.Meter.Namespace", "beta-1.0", typeof(float))]
    public enum MyMetricFloat
    {
        [MetricDefinition(InstrumentType.Counter, "CounterMetric", "Test Counter Metric", Unit = "MadeUpUnit")]
        CounterMetric,

        [MetricDefinition(InstrumentType.UpDownCounter, "UpDownMetric", "Test Up/Down Metric")]
        UpDownMetric,

        [MetricDefinition(InstrumentType.Histogram, "HistogramMetric", "Test Histogram Metric")]
        HistogramMetric,

        [MetricDefinition(InstrumentType.Counter, "EverythingCounterMetric", "Test Everything Counter Metric")]
        [MetricDefinition(InstrumentType.UpDownCounter, "EverythingUpDownMetric", "Test Everything Up/Down Metric")]
        [MetricDefinition(InstrumentType.Histogram, "EverythingHistogramMetric", "Test Everything Histogram Metric")]
        EverythingMetric
    }

    /// <summary>
    /// Test Metric
    /// </summary>
    [MeterDefinition("My.Meter.Namespace", "beta-1.0", typeof(byte))]
    public enum MyMetricByte
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

    /// <summary>
    /// Validate that the Logger Mock is in fact working through <see cref="AutoMocker"/>.
    /// </summary>
    [Fact]
    public void TestLoggerMock()
    {
        var mocker = CreateMocker<MyMetricLong, long>();
        var logger = mocker.Get<ILogger<MetricsService<MyMetricLong, long>>>();
        logger.LogError(new Exception("TextException"), "Bad Thing Happened!");
        mocker
            .GetMock<ILogger<MetricsService<MyMetricLong, long>>>()
            .VerifyLog(logger => logger.LogError(new Exception("TextException"), "Bad Thing Happened!"));
    }

    /// <summary>
    /// Validate Default, Imported Configuration of Instruments
    /// </summary>
    [Fact]
    public void AutoConfigurationTests()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricLong, long>();
        using var service = mocker.CreateInstance<MetricsService<MyMetricLong>>() as IMetricsService<MyMetricLong>;
        service.Should().NotBeNull();

        // Assert Meter
        service.Meter.Name
            .Should().Be("My.Meter.Namespace");
        service.Meter.Version
            .Should().Be("beta-1.0");
        service.Meter.Tags
            .Should().NotBeNull()
            .And.HaveCount(1)
            .And.Contain("MyMetricTag", "MyMetricTagValue");

        // Assert Instruments
        service.Instruments
            .Should().HaveCount(6)
            .And.Contain(i => i.Metric == MyMetricLong.CounterMetric)
            .And.Contain(i => i.Metric == MyMetricLong.UpDownMetric)
            .And.Contain(i => i.Metric == MyMetricLong.HistogramMetric)
            .And.Contain(i => i.Metric == MyMetricLong.EverythingMetric && i.InstrumentType == InstrumentType.Counter)
            .And.Contain(i => i.Metric == MyMetricLong.EverythingMetric && i.InstrumentType == InstrumentType.UpDownCounter)
            .And.Contain(i => i.Metric == MyMetricLong.EverythingMetric && i.InstrumentType == InstrumentType.Histogram)
            ;

        // CounterMetric
        service.Instruments.First(i => i.Metric == MyMetricLong.CounterMetric)
            .Should().Be(new MetricInstrumentInfo<MyMetricLong>
            {
                Metric = MyMetricLong.CounterMetric,
                InstrumentType = InstrumentType.Counter,
                MetricNameSuffix = "CounterMetric",
                MetricName = "my.meter.namespace.counterMetric",
                Unit = "MadeUpUnit",
                Description = "Test Counter Metric",
                RequiredTagNames = ["Tag1", "Tag2", "CounterName"]
            });

        // UpDownMetric
        service.Instruments.First(i => i.Metric == MyMetricLong.UpDownMetric)
            .Should().Be(new MetricInstrumentInfo<MyMetricLong>
            {
                Metric = MyMetricLong.UpDownMetric,
                InstrumentType = InstrumentType.UpDownCounter,
                MetricNameSuffix = "UpDownMetric",
                MetricName = "my.meter.namespace.upDownMetric",
                Description = "Test Up/Down Metric",
                RequiredTagNames = ["Tag1", "Tag2"],
                Tags = [new KeyValuePair<string, object?>("CounterName", "UpDownMetric")]
            });

        // HistogramMetric
        service.Instruments.First(i => i.Metric == MyMetricLong.HistogramMetric)
            .Should().Be(new MetricInstrumentInfo<MyMetricLong>
            {
                Metric = MyMetricLong.HistogramMetric,
                InstrumentType = InstrumentType.Histogram,
                MetricNameSuffix = "HistogramMetric",
                MetricName = "my.meter.namespace.histogramMetric",
                Description = "Test Histogram Metric",
                RequiredTagNames = ["Tag1", "Tag2"]
            });

        // EverythingMetric, Counter
        service.Instruments.First(i => i.Metric == MyMetricLong.EverythingMetric && i.InstrumentType == InstrumentType.Counter)
            .Should().Be(new MetricInstrumentInfo<MyMetricLong>
            {
                Metric = MyMetricLong.EverythingMetric,
                InstrumentType = InstrumentType.Counter,
                MetricNameSuffix = "EverythingCounterMetric",
                MetricName = "my.meter.namespace.everythingCounterMetric",
                Description = "Test Everything Counter Metric",
                RequiredTagNames = ["Tag1", "Tag2", "CounterName"],
                Tags = [new KeyValuePair<string, object?>("EverythingMetric", true)]
            });

        // EverythingMetric, UpDownCounter
        service.Instruments.First(i => i.Metric == MyMetricLong.EverythingMetric && i.InstrumentType == InstrumentType.UpDownCounter)
            .Should().Be(new MetricInstrumentInfo<MyMetricLong>
            {
                Metric = MyMetricLong.EverythingMetric,
                InstrumentType = InstrumentType.UpDownCounter,
                MetricNameSuffix = "EverythingUpDownMetric",
                MetricName = "my.meter.namespace.everythingUpDownMetric",
                Description = "Test Everything Up/Down Metric",
                RequiredTagNames = ["Tag1", "Tag2"],
                Tags = [new KeyValuePair<string, object?>("EverythingMetric", true)]
            });

        // EverythingMetric, Histogram
        service.Instruments.First(i => i.Metric == MyMetricLong.EverythingMetric && i.InstrumentType == InstrumentType.Histogram)
            .Should().Be(new MetricInstrumentInfo<MyMetricLong>
            {
                Metric = MyMetricLong.EverythingMetric,
                InstrumentType = InstrumentType.Histogram,
                MetricNameSuffix = "EverythingHistogramMetric",
                MetricName = "my.meter.namespace.everythingHistogramMetric",
                Description = "Test Everything Histogram Metric",
                RequiredTagNames = ["Tag1", "Tag2"],
                Tags = [new KeyValuePair<string, object?>("EverythingMetric", true)]
            });
    }

    /// <summary>
    /// Make sure check for Decorate Enum is working.
    /// </summary>
    [Fact]
    public void MeterAttributeMissingTest()
    {
        // Create Service
        var mocker = CreateMocker<DayOfWeek, int>();
        mocker
            .Invoking(i => i.CreateInstance<MetricsService<DayOfWeek, int>>())
            .Should()
            .Throw<MeterAttributeMissingException>()
            .WithMessage($"Type '{typeof(DayOfWeek)}' is not decorated with '{typeof(MeterDefinitionAttribute)}'.")
            .Where(i => i.EnumType == typeof(DayOfWeek))
            ;
        mocker
            .GetMock<ILogger<MetricsService<DayOfWeek, int>>>()
            .VerifyLog(logger => logger.LogError(It.IsAny<MeterAttributeMissingException>(), $"Unable to initialize Metric Service for '{typeof(DayOfWeek)}'.")); // . 
    }

    /// <summary>
    /// Make Metric Value Type Validation is working.
    /// </summary>
    [Fact]
    public void MetricValueTypeMismatchTest()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricLong, int>();
        mocker
            .Invoking(i => i.CreateInstance<MetricsService<MyMetricLong, int>>())
            .Should()
            .Throw<MetricValueTypeMismatchException>()
            .WithMessage($"Type '{typeof(MyMetricLong)}' specifies a MetricValueType of '{typeof(long)}' but this service was created with a MetricValueType of '{typeof(int)}'.")
            .Where(i => i.AttributeMetricValueType == typeof(long))
            .Where(i => i.EnumType == typeof(MyMetricLong))
            .Where(i => i.ServiceMetricValueType == typeof(int))
            ;
        mocker
            .GetMock<ILogger<MetricsService<MyMetricLong, int>>>()
            .VerifyLog(logger => logger.LogError(It.IsAny<MetricValueTypeMismatchException>(), $"Unable to initialize Metric Service for '{typeof(MyMetricLong)}'."));
    }

    /// <summary>
    /// Make sure all Meter Instrument Types can be Incremented
    /// </summary>
    [Fact]
    public void PerformIncOnAllMeters()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricFloat, float>();
        var service = mocker.CreateInstance<MetricsService<MyMetricFloat, float>>();
        
        // Inc each meter
        foreach (var metric in Enum.GetValues<MyMetricFloat>().Where(i => i != MyMetricFloat.EverythingMetric))
            service
                .Invoking(i => i.IncCounter(metric))
                .Should().NotThrow();

        // Invalid Instrument
        service
            .Invoking(i => i.IncCounter(MyMetricFloat.UpDownMetric, InstrumentType.Counter))
            .Should().Throw<KeyNotFoundException>().WithMessage("There is no metric named 'UpDownMetric' with an instrument type of 'Counter' defined.");
    }

    /// <summary>
    /// Make sure all Meter Instrument Types can be Decremented when appropriate
    /// </summary>
    [Fact]
    public void PerformDecOnAllMeters()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricFloat, float>();
        var service = mocker.CreateInstance<MetricsService<MyMetricFloat, float>>();

        // Dec UpDown Meter
        service
            .Invoking(i => i.DecCounter(MyMetricFloat.UpDownMetric))
            .Should().NotThrow();

        // These should fail to decrement
        foreach (var metric in Enum.GetValues<MyMetricFloat>().Where(i => i != MyMetricFloat.EverythingMetric && i != MyMetricFloat.UpDownMetric))
            service
                .Invoking(i => i.DecCounter(metric))
                .Should().Throw<ArgumentException>().WithMessage($"No Compatible Instruments found for Metric '{metric}'.");

        // Invalid Instrument
        service
            .Invoking(i => i.DecCounter(MyMetricFloat.UpDownMetric, InstrumentType.Counter))
            .Should().Throw<KeyNotFoundException>().WithMessage("There is no metric named 'UpDownMetric' with an instrument type of 'Counter' defined.");
    }

    /// <summary>
    /// Should not allow IncCounter without specifying an Instrument Type when there are multiples registered!
    /// </summary>
    [Fact]
    public void MultipleInstrumentsExceptionTest()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricFloat, float>();
        var service = mocker.CreateInstance<MetricsService<MyMetricFloat, float>>();

        // Should throw exception!
        service
            .Invoking(i => i.IncCounter(MyMetricFloat.EverythingMetric))
            .Should().Throw<MultipleMetricsException>().WithMessage($"Multiple Instruments found for Metric '{MyMetricFloat.EverythingMetric}'.");
    }

    /// <summary>
    /// Make Sure Missing Tag Exception is thrown when the required tags are missing
    /// </summary>
    [Fact]
    public void RequiredTagsExceptionTest()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricLong, long>();
        var service = mocker.CreateInstance<MetricsService<MyMetricLong, long>>();

        // Should Throw Exceptions
        service
            .Invoking(i => i.IncCounter(MyMetricLong.CounterMetric))
            .Should().Throw<MissingRequiredTagsException>().WithMessage("Missing Required Tag(s): Tag1, Tag2, CounterName");
        service
            .Invoking(i => i.DecCounter(MyMetricLong.UpDownMetric))
            .Should().Throw<MissingRequiredTagsException>().WithMessage("Missing Required Tag(s): Tag1, Tag2");
    }

    /// <summary>
    /// Make Sure Assert Testing is being done correctly for all built-in types.
    /// </summary>
    [Fact]
    public void AssertAmountTests()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricLong, long>();

        // Int
        IMetricsService<MyMetricInt, int> intService = mocker.CreateInstance<MetricsService<MyMetricInt, int>>();
        intService
            .Should().NotBeNull();
        intService
            .Invoking(i => i.IncCounter(MyMetricInt.UpDownMetric, 0))
            .Should().Throw<ArgumentOutOfRangeException>();
        intService
            .Invoking(i => i.IncCounter(MyMetricInt.UpDownMetric))
            .Should().NotThrow<ArgumentOutOfRangeException>();
        intService
            .Invoking(i => i.DecCounter(MyMetricInt.UpDownMetric, 1))
            .Should().NotThrow<ArgumentOutOfRangeException>();

        // Long
        IMetricsService<MyMetricLong, long> longService = mocker.CreateInstance<MetricsService<MyMetricLong, long>>();
        longService
            .Should().NotBeNull();
        longService
            .Invoking(i => i.IncCounter(MyMetricLong.UpDownMetric, 0L))
            .Should().Throw<ArgumentOutOfRangeException>();
        longService
            .Invoking(i => i.IncCounter(MyMetricLong.UpDownMetric))
            .Should().NotThrow<ArgumentOutOfRangeException>();
        longService
            .Invoking(i => i.DecCounter(MyMetricLong.UpDownMetric, 1L))
            .Should().NotThrow<ArgumentOutOfRangeException>();

        // Decimal
        IMetricsService<MyMetricDecimal, decimal> decimalService = mocker.CreateInstance<MetricsService<MyMetricDecimal, decimal>>();
        decimalService
            .Should().NotBeNull();
        decimalService
            .Invoking(i => i.IncCounter(MyMetricDecimal.UpDownMetric, 0m))
            .Should().Throw<ArgumentOutOfRangeException>();
        decimalService
            .Invoking(i => i.IncCounter(MyMetricDecimal.UpDownMetric))
            .Should().NotThrow<ArgumentOutOfRangeException>();
        decimalService
            .Invoking(i => i.DecCounter(MyMetricDecimal.UpDownMetric, 1m))
            .Should().NotThrow<ArgumentOutOfRangeException>();

        // Double
        IMetricsService<MyMetricDouble, double> doubleService = mocker.CreateInstance<MetricsService<MyMetricDouble, double>>();
        doubleService
            .Should().NotBeNull();
        doubleService
            .Invoking(i => i.IncCounter(MyMetricDouble.UpDownMetric, 0d))
            .Should().Throw<ArgumentOutOfRangeException>();
        doubleService
            .Invoking(i => i.IncCounter(MyMetricDouble.UpDownMetric))
            .Should().NotThrow<ArgumentOutOfRangeException>();
        doubleService
            .Invoking(i => i.DecCounter(MyMetricDouble.UpDownMetric, 1d))
            .Should().NotThrow<ArgumentOutOfRangeException>();

        // Float
        IMetricsService<MyMetricFloat, float> floatService = mocker.CreateInstance<MetricsService<MyMetricFloat, float>>();
        floatService
            .Should().NotBeNull();
        floatService
            .Invoking(i => i.IncCounter(MyMetricFloat.UpDownMetric, 0f))
            .Should().Throw<ArgumentOutOfRangeException>();
        floatService
            .Invoking(i => i.IncCounter(MyMetricFloat.UpDownMetric))
            .Should().NotThrow<ArgumentOutOfRangeException>();
        floatService
            .Invoking(i => i.DecCounter(MyMetricFloat.UpDownMetric, 1f))
            .Should().NotThrow<ArgumentOutOfRangeException>();

        // Long (Implicit)
        IMetricsService<MyMetricLong> longImpliedService = mocker.CreateInstance<MetricsService<MyMetricLong>>();
        longImpliedService
            .Should().NotBeNull();
        longImpliedService
            .Invoking(i => i.IncCounter(MyMetricLong.UpDownMetric, 0L))
            .Should().Throw<ArgumentOutOfRangeException>();
        longImpliedService
            .Invoking(i => i.IncCounter(MyMetricLong.UpDownMetric))
            .Should().NotThrow<ArgumentOutOfRangeException>();
        longImpliedService
            .Invoking(i => i.DecCounter(MyMetricLong.UpDownMetric, 1L))
            .Should().NotThrow<ArgumentOutOfRangeException>();

        // Byte
        IMetricsService<MyMetricByte, byte> byteImpliedService = mocker.CreateInstance<MetricsService<MyMetricByte, byte>>();
        byteImpliedService
            .Should().NotBeNull();
        byteImpliedService
            .Invoking(i => i.IncCounter(MyMetricByte.UpDownMetric, 0))
            .Should().Throw<ArgumentOutOfRangeException>();
        byteImpliedService
            .Invoking(i => i.IncCounter(MyMetricByte.UpDownMetric))
            .Should().NotThrow<ArgumentOutOfRangeException>();
        byteImpliedService
            .Invoking(i => i.DecCounter(MyMetricByte.UpDownMetric, 1))
            .Should().NotThrow<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Make Sure Extensions throw error when the service is null!
    /// </summary>
    [Fact]
    public void ExtensionServiceNullTests()
    {
        Action act;
        act = () => MetricsServiceExtensions.CreateDisposableCounter<MyMetricLong, long>(null!, MyMetricLong.CounterMetric);
        act.Should().Throw<ArgumentNullException>();
        act = () => MetricsServiceExtensions.CreateDisposableCounter<MyMetricLong, long>(null!, MyMetricLong.CounterMetric, (Func<IEnumerable<KeyValuePair<string, object?>>>?) null);
        act.Should().Throw<ArgumentNullException>();
        act = () => MetricsServiceExtensions.IncCounter<MyMetricLong, long>(null!, MyMetricLong.CounterMetric);
        act.Should().Throw<ArgumentNullException>();
        act = () => MetricsServiceExtensions.IncCounter<MyMetricLong, long>(null!, MyMetricLong.CounterMetric, InstrumentType.Counter);
        act.Should().Throw<ArgumentNullException>();
        act = () => MetricsServiceExtensions.DecCounter<MyMetricLong, long>(null!, MyMetricLong.CounterMetric);
        act.Should().Throw<ArgumentNullException>();
        act = () => MetricsServiceExtensions.DecCounter<MyMetricLong, long>(null!, MyMetricLong.CounterMetric, InstrumentType.Counter);
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Make sure a Disposable Counter cannot be created with something that isn't the Up/Down Counter
    /// </summary>
    [Fact]
    public void CreateDisposableCounterWithInvalidInstrument()
    {
        // Create Service
        var mocker = CreateMocker<MyMetricLong, long>();
        var service = mocker.CreateInstance<MetricsService<MyMetricLong, long>>();

        // Should Throw Exceptions
        service
            .Invoking(i => i.CreateDisposableCounter(MyMetricLong.CounterMetric))
            .Should().Throw<ArgumentException>().WithMessage($"Metric '{MyMetricFloat.CounterMetric.GetDisplayName()}' does not support instrument type '{InstrumentType.UpDownCounter.GetDisplayName()}'.");

    }
}