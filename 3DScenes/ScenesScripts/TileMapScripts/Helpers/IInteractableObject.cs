using Godot;

namespace ScenesHelper.TileMapScripts;

public interface IInteractable
{
    void Interact(CharacterBody3D character);
    void Initialize();
}