using System;
using Godot;
using ScenesHelper.TileMapScripts;

namespace Characters.EntityModularNodesScripts;
public partial class Entity : Node3D, IInteractable
{
    [Export] public string EntityName { get; set; }
    [Export] public EntitiesTypes Type { get; set; }

    public void Interact(CharacterBody3D character)
    {
        GD.Print($"Hello from {Name}");
    }

    public void Initialize()
    {
        MetadataCheck();
        GetAllModules();
    }

    private void MetadataCheck()
    {
        if(Type == EntitiesTypes.NotDefined)
        {
            GD.PrintErr($"Type of Entity is not defined! Entity name: {EntityName}, type of it: {Type}. NotDefined type is assigned!");
            Type = EntitiesTypes.Neutral;
        }
        else
        {
            GD.Print($"Entity is properly initialized named {EntityName}, with type {Type}");
            //Initialize
        }
    }

    private void GetAllModules()
    {
        for(int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            GD.Print($"Module with index {i} and name {child.Name} exist in {EntityName}. Type of it {child.GetType()}");
        }
    }
}