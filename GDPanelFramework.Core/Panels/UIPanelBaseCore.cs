using System.Collections.Generic;
using Godot;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelBaseCore : Control
{
    internal enum PanelStatus
    {
        Uninitialized,
        Initialized,
        Opened,
        Closed
    }
    
    private Control _cachedSelection;
    private bool _isShownInternal;
    private readonly Dictionary<Control, CachedControlInfo> _cachedChildrenControlInfos= new();

    internal record struct CachedControlInfo(FocusModeEnum FocusMode, MouseFilterEnum MouseFilter);

    internal PanelStatus CurrentPanelStatus { get; set; } = PanelStatus.Uninitialized;

    internal abstract void InitializePanelInternal();
    
    internal SelectionCachingResult CacheCurrentSelection(ref Control currentSelection)
    {
        _cachedSelection = null;
        currentSelection ??= GetViewport().GuiGetFocusOwner();
        if (currentSelection == null) return SelectionCachingResult.NoSelections;
        if (!IsAncestorOf(currentSelection)) return SelectionCachingResult.NotAChild;
        _cachedSelection = currentSelection;
        return SelectionCachingResult.Successful;
    }

    internal void TryRestoreSelection(ref bool success)
    {
        if (success) return;

        if (_cachedSelection is null) return;

        success = true;
        _cachedSelection.CallDeferred(Control.MethodName.GrabFocus);
        _cachedSelection = null;
    }
    
    internal void SetPanelActiveState(bool active, LayerVisual layerVisual)
    {
        if (!active)
        {
            Control control = null;
            CacheCurrentSelection(ref control);

            if (layerVisual == LayerVisual.Hidden)
            {
                _isShownInternal = false;
                HidePanel();
            }

            NodeUtils.SetNodeChildAvailability(this, _cachedChildrenControlInfos, false);
        }
        else
        {
            NodeUtils.SetNodeChildAvailability(this, _cachedChildrenControlInfos, true);

            if (_isShownInternal) return;
            
            _isShownInternal = true;
            ShowPanel();
        }
    }

    internal void HidePanel()
    {
        
    }
    
    internal void ShowPanel()
    {
        
    }
}