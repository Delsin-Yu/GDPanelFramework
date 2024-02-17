using System;
using System.Collections;
using System.Collections.Generic;
using GDPanelSystem.Core.Panels;
using Godot;

namespace GDPanelSystem.Core;

public static class PanelManager
{
    private class Pool<T> where T : new()
    {
        private readonly Stack<T> _cache = new();

        public T Get() =>
            _cache.TryPop(out var result) ? result : new T();

        public void Release(T queue) =>
            _cache.Push(queue);
    }

    private record struct PanelRootInfo(Node Owner, Control Root);

    private static readonly Pool<Stack<UIPanelBaseCore>> _stackPool = new();

    private static readonly Dictionary<PackedScene, Queue<object>> _bufferedPanels = new();
    private static readonly Stack<Stack<UIPanelBaseCore>> _panelStack = new();

#warning Initialize
    private static readonly Stack<PanelRootInfo> _panelRoots = new();

    private static bool _panelRootInitialized;

    private static Control GetCurrentPanelRoot()
    {
        if (_panelRootInitialized) return _panelRoots.Peek().Root;
        
        _panelRoots.Push(new PanelRootInfo(RootPanelContainer.PanelRoot, RootPanelContainer.PanelRoot));
        _panelRootInitialized = true;

        return _panelRoots.Peek().Root;
    }

    private static Stack<UIPanelBaseCore> PushPanelStack()
    {
        Stack<UIPanelBaseCore> newInstance = _stackPool.Get();
        _panelStack.Push(newInstance);
        return newInstance;
    }

    private static void PopPanelStack()
    {
        var instance = _panelStack.Pop();
        instance.Clear();
        _stackPool.Release(instance);
    }

    internal static void PushPanelToPanelStack<TPanel>(TPanel panelInstance, OpenLayer openLayer, LayerVisual previousLayerVisual) where TPanel : UIPanelBaseCore
    {
        Stack<UIPanelBaseCore> focusingPanelStack;

        // Ensure the current panel is at the front most.
        var parent = GetCurrentPanelRoot();
        var oldParent = panelInstance.GetParent();
        if (oldParent == parent) parent.MoveToFront();
        else panelInstance.Reparent(parent);

        // When pushing a panel to the current layer, create an initial panel stack if necessary, and sets the focusingPanelStack to the topmost stack.
        if (openLayer == OpenLayer.SameLayer)
        {
            if (_panelStack.Count == 0) PushPanelStack();

            focusingPanelStack = _panelStack.Peek();
            Control currentFocusingControl = null;
            foreach (var item in focusingPanelStack)
            {
                if (item.CacheCurrentSelection(ref currentFocusingControl) is SelectionCachingResult.Successful or SelectionCachingResult.NoSelections) break;
            }
        }
        // When pushing a panel to new layer, disables gui handling for every panels in the topmost panel stack, creates a new panel stack, and sets the focusingPanelStack to the topmost stack. 
        else
        {
            if (_panelStack.TryPeek(out var topmostPanelStack))
                foreach (var item in topmostPanelStack)
                {
                    item.SetPanelActiveState(false, previousLayerVisual);
                }


            focusingPanelStack = PushPanelStack();
        }

        focusingPanelStack.Push(panelInstance);
    }

    internal static void HandlePanelClose<TPanel>(TPanel closingPanel, OpenLayer openLayer, LayerVisual previousLayerVisual) where TPanel : UIPanelBaseCore
    {
        if (openLayer == OpenLayer.SameLayer)
        {
            var operatingLayer = _panelStack.Peek();
            var topPanel = operatingLayer.Peek();

            ExceptionUtils.ThrowIfClosingPanelIsNotTopPanel(closingPanel, topPanel);

            operatingLayer.Pop();

            if (operatingLayer.Count == 0)
            {
                PopPanelStack();
            }

            if(!_panelStack.TryPeek(out operatingLayer)) return;
            topPanel = operatingLayer.Peek();
            var _ = false;
            topPanel.TryRestoreSelection(ref _);
        }
        else
        {
            var operatingLayer = _panelStack.Peek();
            ExceptionUtils.ThrowIfPanelLayerIsGreaterThanOne(operatingLayer);
            var topPanel = operatingLayer.Peek();

            ExceptionUtils.ThrowIfClosingPanelIsNotTopPanel(closingPanel, topPanel);

            PopPanelStack();

            if (!_panelStack.TryPop(out operatingLayer)) return;

            var selectionRestoreResult = false;
            foreach (var panel in operatingLayer)
            {
                panel.SetPanelActiveState(true, previousLayerVisual);
                panel.TryRestoreSelection(ref selectionRestoreResult);
            }
        }
    }

    public static TPanel GetOrCreatePanel<TPanel>(this PackedScene packedPanel, Action<TPanel> initializeCallback = null) where TPanel : UIPanelBaseCore
    {
        TPanel panelInstance;
        if (_bufferedPanels.TryGetValue(packedPanel, out var instanceQueue))
        {
            panelInstance = (TPanel)instanceQueue.Dequeue();
            initializeCallback?.Invoke(panelInstance);
            return panelInstance;
        }

        panelInstance = packedPanel.InstantiateOrNull<TPanel>();
        if (panelInstance is null)
        {
            throw new InvalidOperationException($"Unable to cast {packedPanel.ResourceName} to {typeof(TPanel)}!");
        }

        GetCurrentPanelRoot().AddChild(panelInstance);
        initializeCallback?.Invoke(panelInstance);
        panelInstance.InitializePanelInternal();
        return panelInstance;
    }

    public static UIPanel.OpenArgsBuilder OpenPanel<TPanel>(this TPanel panel) where TPanel : UIPanel
    {
        panel.ThrowIfUninitialized();
        return new UIPanel.OpenArgsBuilder(panel);
    }

    public static UIPanelParam<TOpenParam, TCloseParam>.OpenArgsBuilder OpenPanel<TOpenParam, TCloseParam>(this UIPanelParam<TOpenParam, TCloseParam> panel, TOpenParam param)
    {
        panel.ThrowIfUninitialized();
        return new UIPanelParam<TOpenParam, TCloseParam>.OpenArgsBuilder(panel, param);
    }
}