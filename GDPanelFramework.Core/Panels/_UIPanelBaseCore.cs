using System;
using System.Collections.Generic;
using System.Threading;
using GDPanelSystem.Core.Panels.Tweener;
using Godot;

namespace GDPanelSystem.Core.Panels;

/// <summary>
/// The fundamental type for all panels, do not inherit this type.
/// </summary>
public abstract partial class _UIPanelBaseCore : Control
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
    private readonly Dictionary<Control, CachedControlInfo> _cachedChildrenControlInfos = new();
    private IPanelTweener? _panelTweener;
    internal CancellationTokenSource _panelCloseTokenSource = new();
    internal CancellationTokenSource _panelOpenTweenFinishTokenSource = new();
    internal CancellationTokenSource _panelCloseTweenFinishTokenSource = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> that gets canceled when the <see cref="UIPanel.ClosePanel"/> / <see cref="UIPanelArg{TOpenArg,TCloseArg}.ClosePanel"/> calls.
    /// </summary>
    public CancellationToken? PanelCloseToken => _panelCloseTokenSource?.Token;

    /// <summary>
    /// A <see cref="CancellationToken"/> that gets canceled when the opening animation finishes.
    /// </summary>
    public CancellationToken? PanelOpenTweenFinishToken => _panelOpenTweenFinishTokenSource?.Token;

    /// <summary>
    /// A <see cref="CancellationToken"/> that gets canceled when the opening animation finishes.
    /// </summary>
    public CancellationToken? PanelCloseTweenFinishToken => _panelCloseTweenFinishTokenSource?.Token;

    internal PackedScene? SourcePrefab { get; private set; }

    internal record struct CachedControlInfo(FocusModeEnum FocusMode, MouseFilterEnum MouseFilter);

    internal PanelStatus CurrentPanelStatus { get; set; } = PanelStatus.Uninitialized;

    internal virtual void InitializePanelInternal(PackedScene sourcePrefab)
    {
        SourcePrefab = sourcePrefab;
        SetPanelChildAvailability(false);
    }

    /// <summary>
    /// The <see cref="IPanelTweener"/> assigned to this panel, assigning null will cause this panel fallbacks to the <see cref="PanelManager.DefaultPanelTweener"/>.
    /// </summary>
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

    internal void SetPanelChildAvailability(bool enabled) => NodeUtils.SetNodeChildAvailability(this, _cachedChildrenControlInfos, enabled);

    /// <summary>
    /// Using the <see cref="PanelTweener"/> to hide this panel.
    /// </summary>
    /// <param name="onFinish">Calls when then hiding animation completes.</param>
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

    
    /// <summary>
    /// Using the <see cref="PanelTweener"/> to show this panel.
    /// </summary>
    /// <param name="onFinish">Calls when then showing animation completes.</param>
    protected void ShowPanel(Action? onFinish = null)
    {
        Visible = true;
        PanelTweener.Show(this, onFinish);
    }
}