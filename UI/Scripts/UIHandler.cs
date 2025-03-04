using GameHelperCharacters;
using Godot;
using Godot.Collections;
using ScenesHelper.ObjectsHelper;

namespace UI;

public partial class UIHandler : Node3D
{
    private InfoPanelController _infoPanel;
	private Node _hoveredObject;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _infoPanel = GD.Load<PackedScene>("res://UI/Nodes/InfoPanel.tscn").Instantiate<InfoPanelController>();
        CallDeferred("add_child", _infoPanel);
	}

	/// <summary>
    /// Handles right-click interactions.
    /// </summary>
    /// <param name="clickedObject">The object that was clicked.</param>
    /// <param name="screenPosition">The position of the mouse click.</param>
    public void HandleRightClick(Node clickedObject, Vector2 screenPosition)
    {
        GD.Print(clickedObject.GetType().Name, "");

        if (clickedObject is IUIDisplayable interactiveObject)
        {
            GD.Print($"Right click on {clickedObject.Name}");
            _infoPanel.DisplayInfo(
                interactiveObject.GetTitleUI(),
                interactiveObject.GetNameUI(),
                //interactiveObject.GetIconUi(),
                interactiveObject.GetDescriptionUI(),
                screenPosition
            );
            _infoPanel.Visible = true;
        }
    }

    /// <summary>
    /// Handles hover interactions.
    /// </summary>
    /// <param name="hoveredObject">The object currently hovered over.</param>
    public void HandleHoverInteraction(Node hoveredObject)
    {
        if(_infoPanel.Visible){
            if (hoveredObject != _hoveredObject)
            {
                // Hide the panel if the mouse moves away from the object and the UI
                if (_hoveredObject != null && !_infoPanel.IsMouseOver())
                {
                    _infoPanel.HidePanel();
                }

                _hoveredObject = hoveredObject;
            }
            else if (hoveredObject == null && !_infoPanel.IsMouseOver())
            {
                // Hide the panel if the mouse is not over the object or the UI
                _infoPanel.HidePanel();
                _hoveredObject = null;
            }
        }
    }
}
