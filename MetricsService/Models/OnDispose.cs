using System;

namespace MetricsService.Models;

/// <summary>
/// Run an action when this item is disposed.
/// </summary>
/// <param name="onDisposeAction">Action to run when disposed is called.</param>
internal class OnDispose(Action onDisposeAction) : IDisposable
{
    /// <summary>
    /// Run an action when this item is disposed with <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="onDisposeAction">Action to run when disposed is called.</param>
    public static IDisposable Synchronous(Action onDisposeAction) => new OnDispose(onDisposeAction);

    private bool _isDisposed;

    #region IDisposable

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        onDisposeAction();
    }

    #endregion
}