using System;
using Godot;

namespace Characters.EntityModularNodesScripts;
public partial class Entity : Node3D, IInteractableEntity
{
    [Export] public string EntityName { get; set; }
    [Export] public EntitiesTypes Type { get; set; }

    /// <summary>
    /// Event triggered when the entity is interacted with.
    /// </summary>
    public event Action<CharacterBody3D> OnInteracted;

    /// <summary>
    /// Event triggered when the entity is initialized.
    /// </summary>
    public event Action OnInitialized;

    public void Interact(CharacterBody3D character)
    {
        if (character == null)
        {
            GD.PrintErr("Cannot interact with a null character.");
            throw new ArgumentNullException(nameof(character));
        }

        GD.Print($"Hello from {EntityName}");
        OnInteracted?.Invoke(character);
    }

    public void Initialize()
    {
        if (string.IsNullOrEmpty(EntityName))
        {
            GD.PrintErr("Entity name is not set!");
            EntityName = "Unnamed Entity";
        }

        CheckType();
        GetAllModules();
        OnInitialized?.Invoke();
    }

    /// <summary>
    /// Checks the entity's type and assigns a default type if not defined.
    /// </summary>
    private void CheckType()
    {
        if (Type == EntitiesTypes.NotDefined)
        {
            GD.PrintErr($"Type of Entity is not defined! Entity name: {EntityName}, type of it: {Type}. Neutral type is assigned!");
            Type = EntitiesTypes.Neutral;
        }
        else
        {
            GD.Print($"Entity is properly initialized named {EntityName}, with type {Type}");
        }
    }

    /// <summary>
    /// Gathers all child nodes (modules) of the entity.
    /// </summary>
    private void GetAllModules()
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            GD.Print($"Module with index {i} and name {child.Name} exists in {EntityName}. Type of it: {child.GetType()}");
        }
    }
}