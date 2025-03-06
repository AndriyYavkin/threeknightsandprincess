using Godot;

namespace Game.HelperCharacters;

/// <summary>
/// Represents the base template for entities (e.g., neutral creatures, units on the global map).
/// </summary>
public abstract partial class EntityTemplate : CharacterBody3D, ICharacterTemplate
{
    /// <summary>
    /// Gets or sets the entity's name.
    /// </summary>
    [Export] public string CharacterName { get; set; }

    /// <summary>
    /// Gets or sets the entity's current speed.
    /// </summary>
    [Export] public float Speed { get; set; }

    /// <summary>
    /// Gets or sets the entity's inventory.
    /// </summary>
    public Inventory Inventory { get; set; } = new();

    /// <summary>
    /// Gets or sets the entity's current grid position.
    /// </summary>
    public Vector3I GridPosition { get; set; }

    /// <summary>
    /// Gets or sets the entity's army.
    /// </summary>
    public Army Army { get; set; } = new();

    /// <summary>
    /// Called when the entity is initialized.
    /// </summary>
    public virtual void Initialize()
    {
        GD.Print($"{CharacterName} initialized.");
    }

    /// <summary>
    /// Called when the entity is updated (e.g., during battle or movement).
    /// </summary>
    public virtual void Update()
    {
        GD.Print($"{CharacterName} updated.");
    }

    // Implement IUIDisplayable
    public string GetTitleUI() => CharacterName;
    public string GetNameUI() => CharacterName;
    // public Texture2D GetIconUI() => new Texture2D(); // Placeholder
    public string GetDescriptionUI() => $"Speed: {Speed}";
}