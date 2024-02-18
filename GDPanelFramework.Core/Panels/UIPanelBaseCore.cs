using System;
using System.Collections.Generic;
using GDPanelSystem.Core.Panels.Tweener;
using GDPanelSystem.Utils.AsyncInterop;
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
    
    private Control? _cachedSelection;
    private bool _isShownInternal;
    private readonly Dictionary<Control, CachedControlInfo> _cachedChildrenControlInfos= new();
    private IPanelTweener? _panelTweener;
    
    
    internal record struct CachedControlInfo(FocusModeEnum FocusMode, MouseFilterEnum MouseFilter);

    internal PanelStatus CurrentPanelStatus { get; set; } = PanelStatus.Uninitialized;

    internal abstract void InitializePanelInternal();

    protected IPanelTweener PanelTweener
    {
        get => _panelTweener ?? PanelManager.DefaultPanelTweener;
        set => _panelTweener = value;
    }
    
    internal SelectionCachingResult CacheCurrentSelection(ref Control? currentSelection)
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
            Control? control = null;
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

	protected void HidePanel(Action? onFinish = null) => PanelTweener.Hide(this, onFinish);

	protected void ShowPanel(Action? onFinish = null) => PanelTweener.Show(this, onFinish);
}