using System;
using System.Collections.Generic;

namespace GDPanelSystem.Utils.Pooling;

internal class Pool<T>
{
    private static readonly Stack<T> _pool = new();

    internal static T Get(Func<T> creationHandler)
    {
        if (!_pool.TryPop(out var instance)) instance = creationHandler();
        return instance;
    }

    internal static void Collect(T instance) => _pool.Push(instance);
}