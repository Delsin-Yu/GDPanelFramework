using System.Runtime.InteropServices;

namespace GDPanelFramework;

/// <summary>
/// A struct that used as a <see cref="System.Void"/> type in generic panels.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0)]
public struct Empty
{
    /// <summary>
    /// The default value for <see cref="Empty"/>.
    /// </summary>
    public static readonly Empty Default = new();
}