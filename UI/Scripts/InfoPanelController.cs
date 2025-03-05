using Godot;
using System;

namespace Game.UI;

public partial class InfoPanelController : Control
{
	private Label _titleLabel;
    //private TextureRect _iconTextureRect;
    private Label _nameLabel;
    private RichTextLabel _descriptionLabel;

	public override void _Ready()
	{
        _titleLabel = GetNode<Label>("TitleLabel");
        //_iconTextureRect = GetNode<TextureRect>("IconTexture");
        _nameLabel = GetNode<Label>("NameLabel");
        _descriptionLabel = GetNode<RichTextLabel>("DescriptionLabel");

        Visible = false;
	}

	/// <summary>
    /// Displays information about an object in the panel.
    /// </summary>
    /// <param name="title">The title of the panel.</param>
    /// <param name="name">The name of the object.</param>
    /// <param name="icon">The icon of the object.</param>
    /// <param name="description">The description of the object.</param>
    /// <param name="position">The position to display the panel.</param>
    public void DisplayInfo(string title, string name, /*Texture2D icon,*/ string description, Vector2 screenPosition)
    {
        _titleLabel.Text = title;
        _nameLabel.Text = name;
        //_iconTextureRect.Texture = icon;
        _descriptionLabel.Text = description;

        var panelSize = GetMinimumSize();
        Position = screenPosition - new Vector2(panelSize.X / 2, panelSize.Y);

        // Ensure the panel stays within the screen bounds
        var viewportSize = GetViewport().GetVisibleRect().Size;
        Position = new Vector2(
            Mathf.Clamp(Position.X, 0, viewportSize.X - panelSize.X),
            Mathf.Clamp(Position.Y, 0, viewportSize.Y - panelSize.Y)
        );

        Visible = true;
    }

    /// <summary>
    /// Checks if the mouse is over the UI.
    /// </summary>
    /// <returns>True if the mouse is over the UI, otherwise false.</returns>
    public bool IsMouseOver()
    {
        var mousePosition = GetGlobalMousePosition();
        return GetGlobalRect().HasPoint(mousePosition);
    }

	/// <summary>
    /// Hides the panel.
    /// </summary>
    public void HidePanel()
    {
        Visible = false;
    }
}