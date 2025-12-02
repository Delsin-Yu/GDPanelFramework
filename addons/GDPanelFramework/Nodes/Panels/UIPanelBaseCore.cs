using System;
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
    /// <summary>
    /// Indicates the current internal state of the panel.
    /// </summary>
    public enum PanelStatus
    {
        /// <summary>
        /// The panel has not yet been initialized by the PanelManager, this is the state when the panel has exited the constructor.
        /// </summary>
        Uninitialized,
        /// <summary>
        /// The panel has been initialized by the PanelManager and not yet opened, this is the state when the panel instance is created and returned from <see cref="PanelManager.CreatePanel"/>.
        /// </summary>
        Initialized,
        /// <summary>   
        /// The panel has been opened by one of the OpenPanel methods, this is the state when the panel is visible and interactable.
        /// </summary>
        Opened,
        /// <summary>
        /// The panel has been closed and, maybe reopened later or gets freed, this is the state when the panel is not visible and not interactable.
        /// </summary>
        Closed,
    }

    private Control? _cachedSelection;
    private bool _isShownInternal;
    private IPanelTweener? _panelTweener;
    private string? _cachedName;
    internal CancellationTokenSource? PanelCloseTokenSource;
    internal CancellationTokenSource? PanelOpenTweenFinishTokenSource;
    internal CancellationTokenSource? PanelCloseTweenFinishTokenSource;


    /// <summary>
    /// Called when the system is closing the panel (after <see cref="UIPanelBase{TOpenArg,TCloseArg}._OnPanelClose"/>).
    /// </summary>
    /// <remarks>
    /// This event is considered "Protected", that is, throwing an exception inside the override of this event will not cause the framework to malfunction.
    /// </remarks>
    public event Action? OnPanelClosed;
    private protected void InvokePanelClosed() => OnPanelClosed?.Invoke();

    internal PackedScene? SourcePrefab { get; private set; }
    internal string LocalName => _cachedName ??= Name;

    internal record struct CachedControlInfo(FocusModeEnum FocusMode, MouseFilterEnum MouseFilter);

    /// <summary>
    /// The current status of the panel.
    /// </summary>
    public PanelStatus CurrentPanelStatus { get; internal set; } = PanelStatus.Uninitialized;

    internal virtual void InitializePanelInternal(PackedScene sourcePrefab)
    {
        SourcePrefab = sourcePrefab;
        SetPanelActiveState(false, PreviousPanelVisual.Hidden, true);
    }

    /// <summary>
    /// A <see cref="CancellationToken"/> that gets canceled when the <see cref="UIPanel.ClosePanel"/> / <see cref="UIPanelArg2{TOpenArg,TCloseArg}.ClosePanel"/> calls.
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
        if (!IsInstanceValid(_cachedSelection)) return;

        if (_cachedSelection.GetFocusModeWithOverride() != FocusModeEnum.None)
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

    internal void SetPanelChildAvailability(bool enabled)
    {
        FocusBehaviorRecursive = enabled ? FocusBehaviorRecursiveEnum.Inherited : FocusBehaviorRecursiveEnum.Disabled;
        MouseBehaviorRecursive = enabled ? MouseBehaviorRecursiveEnum.Inherited : MouseBehaviorRecursiveEnum.Disabled;
        _OnPanelAvailabilityChanged(enabled);
    }

    /// <summary>
    /// Called when the panel availability changes.
    /// </summary>
    /// <param name="enabled">Indicates the panel is available or not.</param>
    protected virtual void _OnPanelAvailabilityChanged(bool enabled) { }

    private static IPanelTweener GetPanelTweener(IPanelTweener panelTweener, bool useNonTweener)
    {
        if (useNonTweener) return NonePanelTweener.Instance;
        return panelTweener;
    }

    private void TweenHide(IPanelTweener tweener) => tweener.Hide(this, () => Visible = false);

    private void TweenHide(IPanelTweener tweener, Action onFinish) =>
        tweener.Hide(
            this,
            () =>
            {
                Visible = false;
                onFinish();
            }
        );

    internal virtual void Cleanup()
    {
        PanelCloseTokenSource?.Dispose();
        PanelOpenTweenFinishTokenSource?.Dispose();
        PanelCloseTweenFinishTokenSource?.Dispose();

        SourcePrefab = null;

        _cachedSelection = null;
        _panelTweener = null;
        OnPanelClosed = null;

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