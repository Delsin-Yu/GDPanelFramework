using System;
using System.Collections;
using System.Collections.Generic;
using GDPanelSystem.Core.Panels;
using Godot;

namespace GDPanelSystem.Core.Core;

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

    private static Control GetCurrentPanelRoot() => _panelRoots.Peek().Root;

    private static Stack<UIPanelBaseCore> PushPanelStack()
    {
        Stack<UIPanelBaseCore> newInstance = _stackPool.Get();
        _panelStack.Push(newInstance);
        return newInstance;
    }

    internal static TPanel PushPanelToPanelStack<TPanel>(TPanel panelInstance, PanelLayer panelLayer, LayerVisual previousLayerVisual) where TPanel : UIPanelBaseCore
    {
        Stack<UIPanelBaseCore> focusingPanelStack;

        // Ensure the current panel is at the front most.
        var parent = GetCurrentPanelRoot();
        var oldParent = panelInstance.GetParent();
        if (oldParent == parent) parent.MoveToFront();
        else panelInstance.Reparent(parent);

        // When pushing a panel to the current layer, create an initial panel stack if necessary, and sets the focusingPanelStack to the topmost stack.
        if (panelLayer == PanelLayer.SameLayer)
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

        return panelInstance;
    }
    
    public static TPanel CreateOrGetPanel<TPanel>(this PackedScene packedPanel, Action<TPanel> initializeCallback = null) where TPanel : UIPanelBaseCore
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
    
    public static UIPanel.OpenArgsBuilder OpenPanel<TPanel>(this TPanel panel) where TPanel : UIPanel => 
        new(panel);
    public static UIPanelParam<TOpenParam, TCloseParam>.OpenArgsBuilder OpenPanel<TOpenParam, TCloseParam>(this UIPanelParam<TOpenParam, TCloseParam> panel, TOpenParam param) => 
        new(panel, param);
}