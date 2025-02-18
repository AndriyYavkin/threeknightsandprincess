using Godot;

namespace Characters.EntityModularNodesScripts;

public interface IInteractable
{
    public string EntityName { get; set; }
    public EntitiesTypes Type { get; set; }

    void Interact(CharacterBody3D character);
    void Initialize();
}