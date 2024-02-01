using System;
using System.Collections.Generic;
using GDPanelSystem.Core.Panels;
using Godot;

namespace GDPanelSystem.Core.Core;

public static class PanelManager
{
    private class QueuePool
    {
        private readonly Stack<Queue<object>> _reusableQueuePool = new();

        public Queue<object> Get()
        {
            if (_reusableQueuePool.TryPop(out var result))
            {
                return result;
            }

            return new();
        }

        public void Release(Queue<object> queue)
        {
            queue.Clear();
            _reusableQueuePool.Push(queue);
        }
    }

    private record struct PanelRootInfo(Node Owner, Control Root);

    private static readonly QueuePool _queuePool = new();
    
    private static readonly Dictionary<PackedScene, Queue<object>> _bufferedPanels = new();

    #error Initialize
    private static readonly Stack<PanelRootInfo> _panelRoots = new();

    private static Control GetCurrentPanelRoot() => _panelRoots.Peek().Root;
    
    
    private static void PushPanelToPanelStack()
    
    public static TPanel CreateOrGetPanel<TPanel>(this PackedScene packedPanel, Action<TPanel> initializeCallback = null) where TPanel : UIPanelBase<,>
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
    
    public static ArgsBuilder<TPanel> OpenPanel<TPanel>(this TPanel panel) where TPanel : UIPanel => new(panel);
}