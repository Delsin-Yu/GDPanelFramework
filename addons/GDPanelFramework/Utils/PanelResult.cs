using System;
using System.Diagnostics.CodeAnalysis;

namespace GDPanelFramework;

/// <summary>
/// Represents the result of a panel operation that may or may not yield a value.
/// </summary>
public readonly struct PanelResult<TValue>(TValue value, bool hasValue) 
{
    /// <summary>
    /// Attempts to get the resolved value.
    /// </summary>
    /// <param name="resolved">When this method returns <see langword="true"/>, contains the resolved value; otherwise, the default value of <typeparamref name="TValue"/>.</param>
    /// <returns>><see langword="true"/> if the result has a value; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue([NotNullWhen(true)] out TValue? resolved)
    {
        resolved = value;
        return hasValue;
    }
    
    /// <summary>
    /// Gets the value of the result, throwing an exception if the result has no value.
    /// </summary>
    /// <returns>The asserted value of the result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the result has no value.</exception>
    public TValue Unwrap()
    {
        if (!hasValue)
            throw new InvalidOperationException("Cannot unwrap a PanelResult that has no value.");
        return value;
    }
    
    /// <summary>
    /// Indicates whether the result has a value.
    /// </summary>
    public bool HasValue => hasValue;
    
    /// <summary>
    /// Creates a <see cref="PanelResult{TValue}"/> instance that represents no value.
    /// </summary>
    public static PanelResult<TValue> None => new(default!, false);
    
    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="PanelResult{TValue}"/> instance.
    /// </summary>
    public static implicit operator PanelResult<TValue>(TValue value) => new(value, true);
}