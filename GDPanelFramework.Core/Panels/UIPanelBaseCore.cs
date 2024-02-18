using System;
using System.Collections.Generic;
using System.Threading;
using GDPanelSystem.Core.Panels.Tweener;
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
    internal CancellationTokenSource _panelCloseTokenSource  = new();
    internal CancellationTokenSource _panelOpenTweenFinishTokenSource  = new();
    internal CancellationTokenSource _panelCloseTweenFinishTokenSource  = new();
    
        
    public CancellationToken? PanelCloseToken => _panelCloseTokenSource?.Token;
    public CancellationToken? PanelOpenTweenFinishToken => _panelOpenTweenFinishTokenSource?.Token;
    public CancellationToken? PanelCloseTweenFinishToken => _panelCloseTweenFinishTokenSource?.Token;
    
    internal PackedScene? SourcePrefab { get; private set; }
    
    internal record struct CachedControlInfo(FocusModeEnum FocusMode, MouseFilterEnum MouseFilter);

    internal PanelStatus CurrentPanelStatus { get; set; } = PanelStatus.Uninitialized;

    internal virtual void InitializePanelInternal(PackedScene sourcePrefab)
    {
        SourcePrefab = sourcePrefab;
        SetPanelChildAvailability(false);
    }

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

            SetPanelChildAvailability(false);
        }
        else
        {
            SetPanelChildAvailability(true);

            if (_isShownInternal) return;
            
            _isShownInternal = true;
            ShowPanel();
        }
    }

    internal void SetPanelChildAvailability(bool enabled) => 
        NodeUtils.SetNodeChildAvailability(this, _cachedChildrenControlInfos, enabled);

    protected void HidePanel(Action? onFinish = null)
    {
        PanelTweener.Hide(
            this,
            () =>
            {
                Visible = false;
                onFinish?.Invoke();
            }
        );
    }

    protected void ShowPanel(Action? onFinish = null)
    {
        Visible = true;
        PanelTweener.Show(this, onFinish);
    }
}