using System.ComponentModel.DataAnnotations;

namespace MetricsService.Models;

/// <summary>
/// Meter Instrument Types
/// </summary>
public enum InstrumentType
{
    Counter,
    [Display(Name = "Up/Down Counter")]
    UpDownCounter,
    Histogram
}