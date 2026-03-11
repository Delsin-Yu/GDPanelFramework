using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GDPanelFramework.Panels;
using GDPanelFramework.Panels.Tweener;
using GDPanelFramework.Utils.Pooling;
using Godot;
using GDPanelFramework;

// using System.Linq;

namespace GDPanelFramework;

/// <summary>
/// <see cref="PanelManager"/> is the core module of GDPanelFramework, it manages panel opening and closing as well as communicates between panel layers, and activates/deactivates them at appropriate times.<br/>
/// This module provides you access to public APIs that responsible for:<br/>
/// 1. Creating panels from PackedScene (<see cref="CreatePanel{TPanel}"/>) and initiating panel opening behavior.<br/>
/// 2. Exposes the system-wide <see cref="DefaultPanelTweener"/>.<br/>
/// 3. Configuring parent for the opening panels through <see cref="PushPanelContainer"/> and <see cref="PopPanelContainer"/>.<br/>
/// 4. Dispatches the <see cref="InputEvent"/>s to the active panels and lets you configures the system-wide <see cref="UICancelActionName"/>.
/// </summary>
public static partial class PanelManager
{
    /// <summary>
    /// A delegate that defines a method to configure the panel container <see cref="Control"/> for the root panel layer.
    /// </summary>
    public delegate Control ConfigurePanelContainerHandler(CanvasLayer panelContainerLayer);

    /// <summary>
    /// Initializes the GDPanelFramework, and allocates a dedicated container for panels.
    /// </summary>
    /// <param name="customHandler">An optional custom handler to configure the root panel container.</param>
    public static void Initialize(ConfigurePanelContainerHandler? customHandler = null)
    {
        if (Engine.IsEditorHint()) return;
        InitializePanelRoot(customHandler);
    }

    private static readonly List<IGlobalInputListener> GlobalInputListeners = [];

    /// <summary>
    /// Registers a global input listener to receive all input events processed by the PanelManager.
    /// </summary>
    public static void AddGlobalInputListener(IGlobalInputListener listener) =>
        GlobalInputListeners.Add(listener);

    /// <summary>
    /// Unregisters a global input listener from receiving input events processed by the PanelManager.
    /// </summary>
    public static void RemoveGlobalInputListener(IGlobalInputListener listener) =>
        GlobalInputListeners.Remove(listener);

    /// <summary>
    /// Gets or sets the default input registration behavior for panels when no specific behavior is provided.
    /// </summary>
    public static InputRegistrationBehavior DefaultInputRegistrationBehavior { get; set; }

    /// <summary>
    /// Defines the input registration behavior for panels.
    /// </summary>
    public enum InputRegistrationBehavior
    {
        /// <summary>
        /// The registered input action is triggered when the associated input is pressed.
        /// </summary>
        Press,
        /// <summary>
        /// The registered input action is triggered when the associated input is released.
        /// </summary>
        Release,
    }

    internal static InputActionPhase GetInputActionPhase(InputActionPhase? actionPhase)
    {
        if (actionPhase is null)
            return DefaultInputRegistrationBehavior switch
            {
                InputRegistrationBehavior.Press => InputActionPhase.Pressed,
                InputRegistrationBehavior.Release => InputActionPhase.Released,
                _ => throw new UnreachableException(),
            };
        return actionPhase.Value;
    }

    private record struct PanelRootInfo(Node? Owner, Control Root);


    private class PanelBuffer
    {
        public readonly Dictionary<PackedScene, Stack<UIPanelBaseCore>> BufferedPanels = new();

        public bool TryGetPanel(PackedScene prefab, [NotNullWhen(true)] out UIPanelBaseCore? instance)
        {
            instance = null;
            if (!BufferedPanels.Remove(prefab, out var panelInstanceStack)) return false;

            if (!panelInstanceStack.TryPop(out var panelInstance))
            {
                BufferedPanels.Remove(prefab);
                return false;
            }

            if (panelInstanceStack.Count == 0) BufferedPanels.Remove(prefab);

            if (!GodotObject.IsInstanceValid(panelInstance))
            {
                panelInstance.Dispose();
                return false;
            }

            instance = panelInstance;
            return true;
        }

        public void BufferPanel(PackedScene prefab, UIPanelBaseCore instance)
        {
            if (!BufferedPanels.TryGetValue(prefab, out var panelInstanceStack))
            {
                panelInstanceStack = [];
                BufferedPanels.Add(prefab, panelInstanceStack);
            }

            panelInstanceStack.Push(instance);
        }
    }

    private static readonly PanelBuffer Buffer = new();
    private static readonly List<(Guid Token, string Name, PanelBuffer Buffer)> ScopedPanelBuffers = [];
    private static readonly Stack<UIPanelBaseCore> PanelStack = new();
    private static readonly Stack<PanelRootInfo> PanelContainers = new();
    private static RootPanelContainer? RootPanelContainer;

    /// <summary>
    /// Begins a scoped panel management session, during which panels opened will be buffered separately.
    /// </summary>
    /// <remarks>
    /// Panels opened during this scoped session will be buffered separately, and when the session ends, all buffered panels will be freed.
    /// This is useful for managing panels in specific contexts, such as game states.
    /// </remarks>
    public static Guid BeginScopedPanelManagement(string name)
    {
        var token = Guid.NewGuid();
        ScopedPanelBuffers.Add((token, name, new()));
        return token;
    }
    
    /// <summary>
    /// Ends a scoped panel management session identified by the provided token.
    /// </summary>
    /// <remarks>
    /// All panels buffered during the scoped session will be freed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when there is no active scoped panel buffer or the provided token does not match the active scoped panel buffer.</exception>
    public static void EndScopedPanelManagement(Guid token)
    {
        if(ScopedPanelBuffers.Count == 0) throw new InvalidOperationException("No scoped panel buffer to end!");
        var (topToken, name, buffer) = ScopedPanelBuffers[^1];
        if (topToken != token) throw new InvalidOperationException($"The currently active scoped panel buffer is '{name}', which does not match the provided token!");
        ScopedPanelBuffers.RemoveAt(ScopedPanelBuffers.Count - 1);
        foreach (var bufferedPanelStack in buffer.BufferedPanels.Values)
            while (bufferedPanelStack.TryPop(out var panelInstance))
            {
                panelInstance.QueueFree();
            }
        buffer.BufferedPanels.Clear();
    }

    /// <summary>
    /// Gets the current panel root container <see cref="Control"/> where subsequent opening panels will be parented to.
    /// </summary>
    /// <remarks>
    /// This method is intended for enable creating customized panel root containers, do not use this method to reparent panels or delete the returned container.
    /// </remarks>
    private static Control GetCurrentPanelRoot()
    {
        if (RootPanelContainer is not null) return PanelContainers.Peek().Root;
        InitializePanelRoot(null);
        return PanelContainers.Peek().Root;
    }

    private static void InitializePanelRoot(ConfigurePanelContainerHandler? customHandler)
    {
        if (RootPanelContainer is not null) throw new InvalidOperationException("Panel root is already initialized!");
        RootPanelContainer = new(customHandler);
        PanelContainers.Push(new(null, RootPanelContainer.Container));
    }

    private static void PushPanelToPanelStack<TPanel>(TPanel panelInstance, PreviousPanelVisual previousPreviousPanelVisual) where TPanel : UIPanelBaseCore
    {
        // Ensure the current panel is at the front most.
        var parent = GetCurrentPanelRoot();
        var oldParent = panelInstance.GetParent();
        if (oldParent == parent) parent.MoveToFront();
        else panelInstance.Reparent(parent);

        // Pushes a panel to new layer, disables gui handling for the previous panel. 
        if (PanelStack.TryPeek(out var topmostPanel)) topmostPanel.SetPanelActiveState(false, previousPreviousPanelVisual);

        PanelStack.Push(panelInstance);
        panelInstance.SetPanelActiveState(true, PreviousPanelVisual.Visible);
    }

    internal static void HandlePanelClose<TPanel>(TPanel closingPanel, PreviousPanelVisual previousPreviousPanelVisual, ClosePolicy closePolicy) where TPanel : UIPanelBaseCore
    {

        var topPanel = PanelStack.Peek();

        ExceptionUtils.ThrowIfClosingPanelIsNotTopPanel(closingPanel, topPanel);

        PanelStack.Pop();

        if (PanelStack.TryPeek(out topPanel))
        {
            topPanel.SetPanelActiveState(true, previousPreviousPanelVisual);
            topPanel.TryRestoreSelection();
        }

        if (closePolicy == ClosePolicy.Delete)
        {
            closingPanel.PanelCloseTweenFinishToken.Register(closingPanel.QueueFree);
            return;
        }

        closingPanel.ThrowIfUnsupportedClosePolicy(closePolicy);

        var sourcePrefab = closingPanel.SourcePrefab!;


        var panelBuffer = Buffer;
        if(ScopedPanelBuffers.Count > 0) panelBuffer = ScopedPanelBuffers[^1].Buffer;
        panelBuffer.BufferPanel(sourcePrefab, closingPanel);
    }

    internal static bool ProcessInputEvent(InputEvent inputEvent)
    {
        foreach (var listener in GlobalInputListeners)
            listener.OnGlobalInput(inputEvent);

        if (!PanelStack.TryPeek(out var topPanel)) return false;

        var cachedWrapper = new CachedInputEvent(inputEvent);

        var hasAccepted = topPanel.ProcessPanelInput(ref cachedWrapper);

        cachedWrapper.Dispose();

        return hasAccepted;
    }

    internal readonly struct CachedInputEvent
    {
        private readonly Dictionary<StringName, bool> _actionHasEvent;

        public CachedInputEvent(InputEvent @event)
        {
            Event = @event;
            _actionHasEvent = Pool.Get<Dictionary<StringName, bool>>(() => new(ReferenceEqualityComparer.Instance));
            Phase = Event.IsPressed() ? InputActionPhase.Pressed : InputActionPhase.Released;
        }

        private class ReferenceEqualityComparer : IEqualityComparer<StringName>
        {
            public static readonly ReferenceEqualityComparer Instance = new();

            public bool Equals(StringName? x, StringName? y) => ReferenceEquals(x, y);

            public int GetHashCode(StringName obj) => obj.IsEmpty.GetHashCode();
        }

        public InputActionPhase Phase { get; }
        public InputEvent Event { get; }

        public bool ActionHasEventCached(StringName action)
        {
            if (_actionHasEvent.TryGetValue(action, out var result)) return result;
            result = InputMap.ActionHasEvent(action, Event);
            _actionHasEvent.Add(action, result);
            return result;
        }

        public void Dispose()
        {
            _actionHasEvent.Clear();
            Pool.Collect(_actionHasEvent);
        }
    }

    /// <summary>
    /// Access the default system-wide <see cref="IPanelTweener"/>.
    /// </summary>
    public static IPanelTweener DefaultPanelTweener { get; set; } = NonePanelTweener.Instance;

    /// <summary>
    /// Access the system-wide <see cref="InputEvent"/> name that is considered the UI Cancel Action.
    /// </summary>
    public static string UICancelActionName { get; set; } = BuiltinInputNames.UICancel;

    /// <summary>
    /// Pushes a new <see cref="Control"/> as the container for subsequent opening panels to the container stack.
    /// </summary>
    /// <remarks>
    /// Pushing and popping actions must be parallel, that is, the topmost container must be popped before you can pop the other containers.
    /// </remarks>
    /// <param name="owner">The owner that perform this action.</param>
    /// <param name="newRoot">The <see cref="Control"/> that is becoming the container for subsequent opening panels.</param>
    public static void PushPanelContainer(Node owner, Control newRoot)
    {
        if (RootPanelContainer is null) InitializePanelRoot(null);
        PanelContainers.Push(new(owner, newRoot));
    }

    /// <summary>
    /// Pops the topmost container from the container stack, which makes the next topmost <see cref="Control"/> become the container for subsequent opening panels.
    /// </summary>
    /// <remarks>
    /// Pushing and popping actions must be parallel, that is, the topmost container must be popped before you can pop the other containers.
    /// </remarks>
    /// <param name="requester">The owner that performs this action.</param>
    /// <exception cref="ArgumentNullException">The requester is null.</exception>
    /// <exception cref="InvalidOperationException">The requester is not the owner of the topmost container of the container stack.</exception>
    public static void PopPanelContainer(Node requester)
    {
        ArgumentNullException.ThrowIfNull(requester);
        ExceptionUtils.ThrowIfUnauthorizedPanelRootOwner(requester, PanelContainers.Peek().Owner);
        var (_, control) = PanelContainers.Pop();
        if (RootPanelContainer is null) InitializePanelRoot(null);
        foreach (var panel in Enumerable.OfType<UIPanelBaseCore>(control.GetChildren())) panel.Reparent(RootPanelContainer!.Container);
    }

    /// <summary>
    /// Try to create an instance of the <typeparamref name="TPanel"/> from the supplied <paramref name="packedPanel"/>.
    /// </summary>
    /// <param name="packedPanel">The <see cref="PackedScene"/> to create the panel from.</param>
    /// <param name="createPolicy">When set to <see cref="CreatePolicy.TryReuse"/>, the system will try reuse an existing cached instance if possible.</param>
    /// <param name="initializeCallback">A delegate that gets called on the instance for pre-initialization.</param>
    /// <typeparam name="TPanel">The panel type to create from the <see cref="PackedScene"/></typeparam>
    /// <returns>The instance of the specified panel.</returns>
    /// <exception cref="InvalidOperationException">Throws when the system is unable to cast the instance of the <paramref name="packedPanel"/> to desired <typeparamref name="TPanel"/> type.</exception>
    public static TPanel CreatePanel<TPanel>(
        this PackedScene packedPanel,
        CreatePolicy createPolicy = CreatePolicy.TryReuse,
        Action<TPanel>? initializeCallback = null) where TPanel : UIPanelBaseCore
    {
        TPanel panelInstance;

        if (createPolicy == CreatePolicy.TryReuse && TryGetBufferedPanel(packedPanel, out var untypedPanelInstance))
        {
            panelInstance = (TPanel)untypedPanelInstance;
            initializeCallback?.Invoke(panelInstance);
            panelInstance.MoveToFront();
            return panelInstance;
        }

        panelInstance = packedPanel.InstantiateOrNull<TPanel>();
        if (panelInstance is null) throw new InvalidOperationException($"Unable to cast {packedPanel.ResourcePath} to {typeof(TPanel)}!");

        GetCurrentPanelRoot().AddChild(panelInstance);
        initializeCallback?.Invoke(panelInstance);
        panelInstance.InitializePanelInternal(packedPanel);
        return panelInstance;
    }

    internal static TPanel CreateRuntimePanel<TPanel>(Action<TPanel>? initializeCallback = null)
        where TPanel : UIPanelBaseCore, new()
    {
        var panelInstance = new TPanel();
        GetCurrentPanelRoot().AddChild(panelInstance);
        initializeCallback?.Invoke(panelInstance);
        panelInstance.InitializePanelInternal();
        return panelInstance;
    }
    
    private static bool TryGetBufferedPanel(PackedScene packedPanel, [NotNullWhen(true)] out UIPanelBaseCore? panelInstance)
    {
        for (var i = ScopedPanelBuffers.Count - 1; i >= 0; i--)
        {
            if (ScopedPanelBuffers[i].Buffer.TryGetPanel(packedPanel, out panelInstance))
                return true;
        }
        return Buffer.TryGetPanel(packedPanel, out panelInstance);
    }
}