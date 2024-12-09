using System.Collections.Generic;
using GDPanelFramework.Panels;
using Godot;

namespace GDPanelFramework;

internal static class NodeUtils
{
    internal static void SetNodeChildAvailability(
        Node node,
        HashSet<Control> mouseOnlyControls,
        Dictionary<Control, UIPanelBaseCore.CachedControlInfo> cachedChildrenControlInfos,
        bool enabled)
    {
        ToggleFocusModeRecursive(node, enabled, mouseOnlyControls, cachedChildrenControlInfos);
    }

    private static void ToggleFocusModeRecursive(
        Node root,
        bool enable,
        HashSet<Control> mouseOnlyControls,
        Dictionary<Control, UIPanelBaseCore.CachedControlInfo> cachedChildrenControlInfos,
        bool includeInternal = false)
    {
        if (!enable)
        {
            cachedChildrenControlInfos.Clear();
            DisableFocusModeRecursive(root, mouseOnlyControls, cachedChildrenControlInfos, includeInternal);
        }
        else
        {
            EnableFocusModeRecursive(root, mouseOnlyControls, cachedChildrenControlInfos, includeInternal);
        }
    }

    private static void DisableFocusModeRecursive(
        Node root,
        HashSet<Control> mouseOnlyControls,
        Dictionary<Control, UIPanelBaseCore.CachedControlInfo> cachedChildrenControlInfos,
        bool includeInternal = false)
    {
        if (root is not Control control) goto Children;

        var focusMode = control.FocusMode;
        var mouseFilter = control.MouseFilter;
        var isMouseOnly = mouseOnlyControls.Contains(control);

        if (!isMouseOnly && focusMode is Control.FocusModeEnum.None && mouseFilter is Control.MouseFilterEnum.Ignore) goto Children;

        control.FocusMode = Control.FocusModeEnum.None;
        control.MouseFilter = Control.MouseFilterEnum.Ignore;

        if (isMouseOnly) goto Children;
        
        cachedChildrenControlInfos[control] = new(focusMode, mouseFilter);

        Children:
        
        foreach (var child in root.GetChildren(includeInternal))
        {
            if (child is UIPanelBaseCore) continue;
            DisableFocusModeRecursive(child, mouseOnlyControls, cachedChildrenControlInfos, includeInternal);
        }
    }

    private static void EnableFocusModeRecursive(
        Node root,
        HashSet<Control> mouseOnlyControls,
        Dictionary<Control, UIPanelBaseCore.CachedControlInfo> cachedChildrenControlInfos,
        bool includeInternal = false)
    {
        if (root is not Control control) goto Children;

        if (mouseOnlyControls.Contains(control))
        {
            control.FocusMode = Control.FocusModeEnum.None;
            control.MouseFilter = Control.MouseFilterEnum.Stop;
        }
        else if (cachedChildrenControlInfos.Remove(control, out var cached))
        {
            control.FocusMode = cached.FocusMode;
            control.MouseFilter = cached.MouseFilter;
        }

        Children:
        
        foreach (var child in root.GetChildren(includeInternal))
        {
            if (child is UIPanelBaseCore) continue;
            EnableFocusModeRecursive(child, mouseOnlyControls, cachedChildrenControlInfos, includeInternal);
        }
    }
}