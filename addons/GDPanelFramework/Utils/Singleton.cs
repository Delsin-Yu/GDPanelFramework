using System;

namespace GodotPanelFramework.Utils;

internal static class Singleton<T>
{
    private static T? _instance;

    internal static T GetInstance(Func<T> creationHandler)
    {
        _instance ??= creationHandler();
        return _instance;
    }
}