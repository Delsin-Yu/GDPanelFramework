using System;
using System.Runtime.CompilerServices;
using GDPanelSystem.Core;
using GDPanelSystem.Utils.Pooling;

namespace GDPanelSystem.Utils.AsyncInterop;

public static class AsyncInterop
{
    private static AsyncAwaitable Create(Action<Action> callbackFunction)
    {
        var instance = Pool.Get<AsyncAwaitable>(() => new());
        instance.Init(callbackFunction);
        return instance;
    }

    private static AsyncAwaitable<T> Create<T>(Action<Action<T>> callbackFunction)
    {
        var instance = Pool.Get<AsyncAwaitable<T>>(() => new());
        instance.Init(callbackFunction);
        return instance;
    }

    public static AsyncAwaitable ToAsync(Action<Action> callbackFunction) => Create(callbackFunction);

    public static AsyncAwaitable<T> ToAsync<T>(Action<Action<T>> callbackFunction) => Create(callbackFunction);
}

public sealed class AsyncAwaitable : INotifyCompletion
{
    internal AsyncAwaitable()
    {
    }

    private readonly AsyncAwaitableBase<Empty> _backing = new();

    internal void Init(Action<Action> callbackFunction) => _backing.Init(callback => callbackFunction(() => callback(Empty.Default)));

    public void OnCompleted(Action continuation) => _backing.OnCompleted(continuation);

    public bool IsCompleted => _backing.IsCompleted;

    public void GetResult() => _backing.GetResult();

    public AsyncAwaitable GetAwaiter() => _backing.GetAwaiter(this);
}

public sealed class AsyncAwaitable<T> : INotifyCompletion
{
    internal AsyncAwaitable()
    {
    }

    private readonly AsyncAwaitableBase<T> _backing = new();

    internal void Init(Action<Action<T>> callbackFunction) => _backing.Init(callbackFunction);

    public void OnCompleted(Action continuation) => _backing.OnCompleted(continuation);

    public bool IsCompleted => _backing.IsCompleted;

    public T GetResult() => _backing.GetResult();

    public AsyncAwaitable<T> GetAwaiter() => _backing.GetAwaiter(this);
}

internal class AsyncAwaitableBase<T>
{
    private Action<Action<T>>? _callbackFunction;
    private Action? _continuation;
    private T? _result;
    private bool _isActive;
    private bool _isCompleted;

    private void ThrowIfNotActive()
    {
        if (!_isActive) throw new InvalidOperationException("GetResult was called on this AsyncAwaitable, any further usage is not supported.");
    }

    internal bool IsCompleted
    {
        get
        {
            ThrowIfNotActive();
            _callbackFunction!(Complete);
            return _isCompleted;
        }
    }

    internal T GetResult()
    {
        _isActive = false;
        _isCompleted = false;
        var result = _result!;
        _result = default;
        _continuation = null;
        Pool.Collect(this);
        return result;
    }

    internal void Init(Action<Action<T>> callbackFunction)
    {
        _callbackFunction = callbackFunction;
        _isActive = true;
    }

    private void Complete(T result)
    {
        _isCompleted = true;
        _result = result;
        _continuation?.Invoke();
    }

    internal void OnCompleted(Action continuation)
    {
        ThrowIfNotActive();
        _continuation = continuation;
    }

    internal TSelf GetAwaiter<TSelf>(TSelf instance)
    {
        ThrowIfNotActive();
        return instance;
    }
}