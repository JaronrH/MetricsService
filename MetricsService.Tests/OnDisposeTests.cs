using FluentAssertions;
using MetricsService.Models;

namespace MetricsService.Tests;

public class OnDisposeTests
{
    /// <summary>
    /// Make sure OnDispose Class works as intended.
    /// </summary>
    [Fact]
    public void EnsureExecutionOnDestroy()
    {
        var updated = 0;
        var onDisposed = new OnDispose(() => ++updated);
        updated.Should().Be(0, "Should not be updated yet!");
        onDisposed
            .Invoking(i => i.Dispose())
            .Should().NotThrow("No exception should be thrown when disposing.");
        updated.Should().Be(1, "Should be updated now!");
        onDisposed
            .Invoking(i => i.Dispose())
            .Should().NotThrow("No exception should be thrown when disposing additional times.");
        updated.Should().Be(1, "Should not call Action again despite Dispose being called again!");
    }
}