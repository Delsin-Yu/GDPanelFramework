using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GDPanelSystem.Utils.Pooling;

internal static class Pool
{
    private static class BackingPool<T>
    {
        private static readonly Stack<T> _pool = new();
        public static bool TryPop([MaybeNullWhen(false)] out T result) => _pool.TryPop(out result);
        public static void Push(T item) => _pool.Push(item);
    }

    internal static T Get<T>(Func<T> creationHandler)
    {
        if (!BackingPool<T>.TryPop(out var instance)) instance = creationHandler();
        return instance;
    }

    internal static void Collect<T>(T instance) => BackingPool<T>.Push(instance);
}