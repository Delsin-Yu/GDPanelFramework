using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GDPanelFramework.Utils.Pooling;

internal static class Pool
{
    private static class BackingPool<T>
    {
        private static readonly Stack<T> Pool = new();
        public static bool TryPop([MaybeNullWhen(false)] out T result) => Pool.TryPop(out result);
        public static void Push(T item) => Pool.Push(item);
    }

    internal static T Get<T>(Func<T> creationHandler)
    {
        if (!BackingPool<T>.TryPop(out var instance)) instance = creationHandler();
        return instance;
    }

    internal static void Collect<T>(T instance) => BackingPool<T>.Push(instance);
}