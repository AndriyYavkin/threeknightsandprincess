using Godot;

namespace Game.HelperCharacters;

/// <summary>
/// Represents the base template for enemy heroes.
/// </summary>
public abstract partial class CharacterHeroTemplate : EntityTemplate
{
    /// <summary>
    /// Gets or sets the enemy hero's level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the enemy hero's experience points.
    /// </summary>
    public int Experience { get; set; }

    /// <summary>
    /// Called when the enemy hero is initialized.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        GD.Print($"{CharacterName} (Level {Level}) initialized.");
    }

    /// <summary>
    /// Called when the enemy hero is updated (e.g., during battle or movement).
    /// </summary>
    public override void Update()
    {
        base.Update();
        GD.Print($"{CharacterName} (Level {Level}) updated.");
    }
}