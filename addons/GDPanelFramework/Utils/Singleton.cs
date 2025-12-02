using System;

namespace GodotPanelFramework.Utils;

static class Singleton<T>
{
    private static T? _instance;

    internal static T GetInstance(Func<T> creationHandler)
    {
        _instance ??= creationHandler();
        return _instance;
    }
}