using System;
using System.Runtime.CompilerServices;
using GDPanelFramework.Utils.Pooling;

namespace GDPanelFramework.Utils.AsyncInterop;

/// <summary>
/// A helper class that provide methods translate a delegate-callback styled method to async/await-compatible styled method.
/// </summary>
public static class AsyncInterop
{
    private static AsyncAwaitable Create(Action<Action> sourceMethod)
    {
        var instance = Pool.Get<AsyncAwaitable>(() => new());
        instance.Init(sourceMethod);
        _ = instance.IsCompleted;
        return instance;
    }

    private static AsyncAwaitable<T> Create<T>(Action<Action<T>> sourceMethod)
    {
        var instance = Pool.Get<AsyncAwaitable<T>>(() => new());
        instance.Init(sourceMethod);
        _ = instance.IsCompleted;
        return instance;
    }

    /// <summary>
    /// Convert a delegate-callback styled method with void returning to async/await-compatible styled method.
    /// </summary>
    /// <param name="sourceMethod">The method to convert from.</param>
    /// <returns>An awaitable that, when awaited, will asynchronously continues after the associated delegate-callback styled method has complete.</returns>
    public static AsyncAwaitable ToAsync(Action<Action> sourceMethod) => Create(sourceMethod);

    /// <summary>
    /// Convert a delegate-callback styled method that returns <typeparamref name="T"/> to async/await-compatible styled method.
    /// </summary>
    /// <param name="sourceMethod">The method to convert from.</param>
    /// <returns>An awaitable that, when awaited, will asynchronously continues after the associated delegate-callback styled method has complete.</returns>
    public static AsyncAwaitable<T> ToAsync<T>(Action<Action<T>> sourceMethod) => Create(sourceMethod);
}

/// <inheritdoc cref="AsyncAwaitableBase{T}"/>
public sealed class AsyncAwaitable : INotifyCompletion
{
    internal AsyncAwaitable()
    {
    }

    private readonly AsyncAwaitableBase<Empty> _backing = new();

    internal void Init(Action<Action> callbackFunction) => _backing.Init(callback => callbackFunction(() => callback(Empty.Default)));

    /// <inheritdoc cref="AsyncAwaitableBase{T}.OnCompleted"/>
    public void OnCompleted(Action continuation) => _backing.OnCompleted(continuation);

    /// <inheritdoc cref="AsyncAwaitableBase{T}.IsCompleted"/>
    public bool IsCompleted => _backing.IsCompleted;
    
    /// <inheritdoc cref="AsyncAwaitableBase{T}.GetResult"/>
    public void GetResult() => _backing.GetResult();

    /// <inheritdoc cref="AsyncAwaitableBase{T}.GetAwaiter{T}"/>
    public AsyncAwaitable GetAwaiter() => _backing.GetAwaiter(this);
}

/// <inheritdoc cref="AsyncAwaitableBase{T}"/>
public sealed class AsyncAwaitable<T> : INotifyCompletion
{
    internal AsyncAwaitable()
    {
    }

    private readonly AsyncAwaitableBase<T> _backing = new();

    internal void Init(Action<Action<T>> sourceMethod) => _backing.Init(sourceMethod);

    /// <inheritdoc cref="AsyncAwaitableBase{T}.OnCompleted"/>
    public void OnCompleted(Action continuation) => _backing.OnCompleted(continuation);

    /// <inheritdoc cref="AsyncAwaitableBase{T}.IsCompleted"/>
    public bool IsCompleted => _backing.IsCompleted;

    /// <inheritdoc cref="AsyncAwaitableBase{T}.GetResult"/>
    public T GetResult() => _backing.GetResult();

    /// <inheritdoc cref="AsyncAwaitableBase{T}.GetAwaiter{T}"/>
    public AsyncAwaitable<T> GetAwaiter() => _backing.GetAwaiter(this);
}

/// <summary>
/// An awaitable that, when awaited, will asynchronously continues after the associated delegate-callback styled method completes.
/// </summary>
internal class AsyncAwaitableBase<T>
{
    private Action<Action<T>>? _sourceMethod;
    private Action? _continuation;
    private T? _result;
    private bool _isActive;
    private bool _isStarted;
    private bool _isCompleted;

    private void ThrowIfNotActive()
    {
        if (!_isActive) throw new InvalidOperationException("GetResult was called on this AsyncAwaitable, any further usage is not supported.");
    }

    /// <summary>
    /// Executes the associated delegate-callback styled method if not already, and returns true if the method completes instantly.
    /// </summary>
    internal bool IsCompleted
    {
        get
        {
            ThrowIfNotActive();
            if (!_isStarted)
            {
                var success = DelegateRunner.RunProtected(_sourceMethod, Complete, "Delegate Execution", "Async Interop");
                if (!success)
                {
                    Collect();
                    throw new InvalidOperationException("Exception thrown when invoking async backing method.");
                }
            }
            _isStarted = true;
            return _isCompleted;
        }
    }

    /// <summary>
    /// Gets the result of the completed, associated delegate-callback styled method
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws when the method is not complete.</exception>
    internal T GetResult()
    {
        var requiresThrow = !_isCompleted;
        var result = _result!;
        Collect();
        if (requiresThrow) throw new InvalidOperationException("The associated method has not completed!");
        return result;
    }

    internal void Init(Action<Action<T>> sourceMethod)
    {
        _sourceMethod = sourceMethod;
        _isActive = true;
    }

    private void Complete(T result)
    {
        _isCompleted = true;
        _result = result;
        RunContinuation();
    }

    private void RunContinuation()
    {
        var success = DelegateRunner.RunProtected(_continuation, "Async Continuation", "Async Interop");
        if (!success)
        {
            Collect();
            throw new InvalidOperationException("Exception thrown when invoking continuation method.");
        }
    }

    private void Collect()
    {
        _isActive = false;
        _isStarted = false;
        _isCompleted = false;
        _result = default;
        _continuation = null;
        Pool.Collect(this);
    }
    
    /// <summary>
    /// Registers a method that gets called when the associated delegate-callback styled method completes.
    /// </summary>
    /// <param name="continuation">A method that gets called when the associated delegate-callback styled method completes.</param>
    internal void OnCompleted(Action continuation)
    {
        ThrowIfNotActive();
        _continuation = continuation;
        if (_isCompleted) RunContinuation();
    }

    /// <summary>
    /// Gets an awaiter for this awaitable.
    /// </summary>
    /// <returns>The awaiter for this awaitable.</returns>
    internal TSelf GetAwaiter<TSelf>(TSelf instance)
    {
        ThrowIfNotActive();
        return instance;
    }
}