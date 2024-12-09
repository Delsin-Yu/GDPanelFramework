using System;
using System.Collections.Generic;
using System.Threading;
using GDPanelFramework.Panels.Tweener;
using GDPanelFramework.Utils.Pooling;
using Godot;

namespace GDPanelFramework.Panels;

/// <summary>
/// The fundamental type for all panels, do not inherit this type.
/// </summary>
[GlobalClass]
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
    private readonly Dictionary<Control, CachedControlInfo> _cachedChildrenControlInfos = new();
    private readonly HashSet<Control> _mouseOnlyControls = new();
    private IPanelTweener? _panelTweener;
    private string? _cachedName;
    internal CancellationTokenSource? PanelCloseTokenSource;
    internal CancellationTokenSource? PanelOpenTweenFinishTokenSource;
    internal CancellationTokenSource? PanelCloseTweenFinishTokenSource;

    internal PackedScene? SourcePrefab { get; private set; }
    internal string LocalName => _cachedName ??= Name;

    internal record struct CachedControlInfo(FocusModeEnum FocusMode, MouseFilterEnum MouseFilter);

    internal PanelStatus CurrentPanelStatus { get; set; } = PanelStatus.Uninitialized;

    internal virtual void InitializePanelInternal(PackedScene sourcePrefab)
    {
        SourcePrefab = sourcePrefab;
        SetPanelActiveState(false, PreviousPanelVisual.Hidden, true);
    }

    /// <summary>
    /// A <see cref="CancellationToken"/> that gets canceled when the <see cref="UIPanel.ClosePanel"/> / <see cref="UIPanelArg{TOpenArg,TCloseArg}.ClosePanel"/> calls.
    /// </summary>
    public CancellationToken PanelCloseToken => (PanelCloseTokenSource ??= new()).Token;

    /// <summary>
    /// A <see cref="CancellationToken"/> that gets canceled when the opening animation finishes.
    /// </summary>
    public CancellationToken PanelOpenTweenFinishToken => (PanelOpenTweenFinishTokenSource ??= new()).Token;

    /// <summary>
    /// A <see cref="CancellationToken"/> that gets canceled when the opening animation finishes.
    /// </summary>
    public CancellationToken PanelCloseTweenFinishToken => (PanelCloseTweenFinishTokenSource ??= new()).Token;

    /// <summary>
    /// The <see cref="IPanelTweener"/> assigned to this panel, assigning null will cause this panel fallbacks to the <see cref="PanelManager.DefaultPanelTweener"/>.
    /// </summary>
    protected IPanelTweener PanelTweener
    {
        get => _panelTweener ?? PanelManager.DefaultPanelTweener;
        set => _panelTweener = value;
    }

    /// <summary>
    /// Sets a <see cref="Control"/> under the current panel to only react with mouse interaction and does not grab focus when pressed. 
    /// </summary>
    protected void SetMouseOnly(Control control)
    {
        _mouseOnlyControls.Add(control);
        if (control.MouseFilter != MouseFilterEnum.Ignore) control.MouseFilter = MouseFilterEnum.Stop;
        control.FocusMode = FocusModeEnum.None;
    }
    
    internal void CacheCurrentSelection(ref Control? currentSelection)
    {
        _cachedSelection = null;
        currentSelection ??= GetViewport()?.GuiGetFocusOwner();
        if (currentSelection == null) return;
        if (!IsAncestorOf(currentSelection)) return;
        _cachedSelection = currentSelection;
    }

    internal void TryRestoreSelection()
    {
        if (_cachedSelection is null) return;

        _cachedSelection.GrabFocus();
        _cachedSelection = null;
    }

    internal void SetPanelActiveState(bool active, PreviousPanelVisual previousPanelVisual, bool useNoneTweener = false)
    {
        if (!active)
        {
            Control? control = null;
            CacheCurrentSelection(ref control);
            CancelPressedInput();

            if (previousPanelVisual == PreviousPanelVisual.Hidden)
            {
                _isShownInternal = false;
                HidePanel(useNoneTweener: useNoneTweener);
            }

            SetPanelChildAvailability(false);
        }
        else
        {
            SetPanelChildAvailability(true);

            if (_isShownInternal) return;

            _isShownInternal = true;
            ShowPanel(useNoneTweener: useNoneTweener);
        }
    }
    
    internal void SetPanelChildAvailability(bool enabled) => NodeUtils.SetNodeChildAvailability(this, _mouseOnlyControls, _cachedChildrenControlInfos, enabled);

    private static IPanelTweener GetPanelTweener(IPanelTweener panelTweener, bool useNonTweener)
    {
        if (useNonTweener) return NonePanelTweener.Instance;
        return panelTweener;
    }

    private void TweenHide(IPanelTweener tweener)
    {
        tweener.Hide(this, () => Visible = false);
    }

    private void TweenHide(IPanelTweener tweener, Action onFinish)
    {
        tweener.Hide(
            this,
            () =>
            {
                Visible = false;
                onFinish();
            }
        );
    }

    internal virtual void Cleanup()
    {
        PanelCloseTokenSource?.Dispose();
        PanelOpenTweenFinishTokenSource?.Dispose();
        PanelCloseTweenFinishTokenSource?.Dispose();

        SourcePrefab = null;

        _cachedSelection = null;
        _panelTweener = null;

        _mouseOnlyControls.Clear();
        _registeredInputEventNames.Clear();

        foreach (var registeredInputEvent in _registeredInputEvent.Values)
        {
            registeredInputEvent.Reset();
            Pool.Collect(registeredInputEvent);
        }

        _registeredInputEvent.Clear();

        _mappedCancelEvent.Clear();

        Dispose();
    }

    /// <summary>
    /// Using the <see cref="PanelTweener"/> to hide this panel.
    /// </summary>
    /// <param name="onFinish">Calls when then hiding animation completes.</param>
    /// <param name="useNoneTweener">Use <see cref="NonePanelTweener"/> to hide this panel.</param>
    protected void HidePanel(Action? onFinish = null, bool useNoneTweener = false)
    {
        var tweener = GetPanelTweener(PanelTweener, useNoneTweener);

        if (onFinish == null) TweenHide(tweener);
        else TweenHide(tweener, onFinish);
    }

    /// <summary>
    /// Using the <see cref="PanelTweener"/> to show this panel.
    /// </summary>
    /// <param name="onFinish">Calls when then showing animation completes.</param>
    /// <param name="useNoneTweener">Use <see cref="NonePanelTweener"/> to show this panel.</param>
    protected void ShowPanel(Action? onFinish = null, bool useNoneTweener = false)
    {
        Visible = true;
        GetPanelTweener(PanelTweener, useNoneTweener).Show(this, onFinish);
    }
}