using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using GDPanelFramework.Utils.Pooling;

namespace GDPanelFramework;

/// <summary>
/// Represents a pooled awaitable operation that completes without returning a value.
/// </summary>
public readonly struct PanelAwaitable
{
    private readonly PanelAwaitableCompletionSource<Empty>? _source;
    private readonly short _version;

    internal PanelAwaitable(PanelAwaitableCompletionSource<Empty> source, short version)
    {
        _source = source;
        _version = version;
    }

    /// <summary>
    /// Gets the awaiter for this awaitable.
    /// </summary>
    public Awaiter GetAwaiter() => new(_source, _version);

    /// <summary>
    /// The awaiter for <see cref="PanelAwaitable"/>.
    /// </summary>
    public readonly struct Awaiter : ICriticalNotifyCompletion
    {
        private readonly PanelAwaitableCompletionSource<Empty>? _source;
        private readonly short _version;

        internal Awaiter(PanelAwaitableCompletionSource<Empty>? source, short version)
        {
            _source = source;
            _version = version;
        }

        /// <summary>
        /// Gets whether the awaitable has already completed.
        /// </summary>
        public bool IsCompleted => Source.IsCompleted(_version);

        /// <summary>
        /// Registers the continuation to run when the awaitable completes.
        /// </summary>
        public void OnCompleted(Action continuation) => Source.OnCompleted(_version, continuation);

        /// <summary>
        /// Registers the continuation to run when the awaitable completes.
        /// </summary>
        public void UnsafeOnCompleted(Action continuation) => Source.OnCompleted(_version, continuation);

        /// <summary>
        /// Retrieves the completion result.
        /// </summary>
        public void GetResult() => Source.GetResult(_version);

        private PanelAwaitableCompletionSource<Empty> Source =>
            _source ?? throw new InvalidOperationException("The awaitable was not initialized.");
    }
}

/// <summary>
/// Represents a pooled awaitable operation that completes with a value.
/// </summary>
/// <typeparam name="TResult">The completion result type.</typeparam>
public readonly struct PanelAwaitable<TResult>
{
    private readonly PanelAwaitableCompletionSource<TResult>? _source;
    private readonly short _version;

    internal PanelAwaitable(PanelAwaitableCompletionSource<TResult> source, short version)
    {
        _source = source;
        _version = version;
    }

    /// <summary>
    /// Gets the awaiter for this awaitable.
    /// </summary>
    public Awaiter GetAwaiter() => new(_source, _version);

    /// <summary>
    /// The awaiter for <see cref="PanelAwaitable{TResult}"/>.
    /// </summary>
    public readonly struct Awaiter : ICriticalNotifyCompletion
    {
        private readonly PanelAwaitableCompletionSource<TResult>? _source;
        private readonly short _version;

        internal Awaiter(PanelAwaitableCompletionSource<TResult>? source, short version)
        {
            _source = source;
            _version = version;
        }

        /// <summary>
        /// Gets whether the awaitable has already completed.
        /// </summary>
        public bool IsCompleted => Source.IsCompleted(_version);

        /// <summary>
        /// Registers the continuation to run when the awaitable completes.
        /// </summary>
        public void OnCompleted(Action continuation) => Source.OnCompleted(_version, continuation);

        /// <summary>
        /// Registers the continuation to run when the awaitable completes.
        /// </summary>
        public void UnsafeOnCompleted(Action continuation) => Source.OnCompleted(_version, continuation);

        /// <summary>
        /// Retrieves the completion result.
        /// </summary>
        public TResult GetResult() => Source.GetResult(_version);

        private PanelAwaitableCompletionSource<TResult> Source =>
            _source ?? throw new InvalidOperationException("The awaitable was not initialized.");
    }
}

internal sealed class PanelAwaitableCompletionSource<TResult>
{
    private enum CompletionState : byte
    {
        Pending,
        Succeeded,
        Canceled,
        Faulted,
    }

    private readonly object _gate = new();
    private Action? _continuation;
    private TResult? _result;
    private ExceptionDispatchInfo? _exception;
    private CancellationToken _cancellationToken;
    private CompletionState _state;
    private bool _isActive;
    private short _version;

    private PanelAwaitableCompletionSource() { }

    internal PanelAwaitable<TResult> Awaitable => new(this, _version);

    internal short Version => _version;

    internal static PanelAwaitableCompletionSource<TResult> Create()
    {
        var source = Pool.Get(() => new PanelAwaitableCompletionSource<TResult>());
        source.Prepare();
        return source;
    }

    internal bool IsCompleted(short version)
    {
        lock (_gate)
        {
            Validate(version);
            return _state != CompletionState.Pending;
        }
    }

    internal void OnCompleted(short version, Action continuation)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        lock (_gate)
        {
            Validate(version);

            if (_state == CompletionState.Pending)
            {
                _continuation += continuation;
                return;
            }
        }

        continuation();
    }

    internal TResult GetResult(short version)
    {
        CompletionState state;
        TResult? result;
        ExceptionDispatchInfo? exception;
        CancellationToken cancellationToken;

        lock (_gate)
        {
            Validate(version);

            if (_state == CompletionState.Pending)
                throw new InvalidOperationException("The awaitable operation has not completed yet.");

            state = _state;
            result = _result;
            exception = _exception;
            cancellationToken = _cancellationToken;
            ResetNoLock();
        }

        Pool.Collect(this);

        if (state == CompletionState.Canceled)
            throw new OperationCanceledException(cancellationToken);

        exception?.Throw();
        return result!;
    }

    internal bool TrySetResult(TResult result) => TryComplete(CompletionState.Succeeded, result, null, default);

    internal bool TrySetCanceled(CancellationToken cancellationToken) =>
        TryComplete(CompletionState.Canceled, default, null, cancellationToken);

    internal bool TrySetException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return TryComplete(CompletionState.Faulted, default, ExceptionDispatchInfo.Capture(exception), default);
    }

    private void Prepare()
    {
        lock (_gate)
        {
            unchecked
            {
                _version++;
                if (_version == 0) _version = 1;
            }

            _continuation = null;
            _result = default;
            _exception = null;
            _cancellationToken = default;
            _state = CompletionState.Pending;
            _isActive = true;
        }
    }

    private bool TryComplete(
        CompletionState state,
        TResult? result,
        ExceptionDispatchInfo? exception,
        CancellationToken cancellationToken)
    {
        Action? continuation;

        lock (_gate)
        {
            if (!_isActive || _state != CompletionState.Pending)
                return false;

            _state = state;
            _result = result;
            _exception = exception;
            _cancellationToken = cancellationToken;
            continuation = _continuation;
            _continuation = null;
        }

        continuation?.Invoke();
        return true;
    }

    private void ResetNoLock()
    {
        _continuation = null;
        _result = default;
        _exception = null;
        _cancellationToken = default;
        _state = CompletionState.Pending;
        _isActive = false;
    }

    private void Validate(short version)
    {
        if (!_isActive || version != _version)
            throw new InvalidOperationException("The awaitable is no longer valid.");
    }
}