using System.Collections.Generic;
using Godot;

namespace GDPanelSystem.Core.Panels;

public abstract class UIPanelBaseCore : Control
{
    private Control _cachedSelection;
    private bool _isShownInternal;
    private readonly Dictionary<Control, CachedControlInfo> _cachedChildrenControlInfos= new();

    internal record struct CachedControlInfo(FocusModeEnum FocusMode, MouseFilterEnum MouseFilter);
    

    
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

            Utils.SetNodeChildAvailability(this, _cachedChildrenControlInfos, false);
        }
        else
        {
            Utils.SetNodeChildAvailability(this, _cachedChildrenControlInfos, true);

            if (!_isShownInternal)
            {
                _isShownInternal = true;
                ShowPanel();
            }
        }
    }

    internal void HidePanel()
    {
        
    }
    
    internal void ShowPanel()
    {
        
    }
}