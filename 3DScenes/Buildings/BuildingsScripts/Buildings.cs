using System.Collections.Generic;
using Godot;

namespace Game.Buildings;

public enum Buildings
{
    NotDefined,
    Farm,
    HouseOfArchers,
    KnightsHouse,
}

/// <summary>
/// Represents a registry of all buildings in the game.
/// </summary>
public abstract partial class BuildingsRegister : StaticBody3D
{
    /// <summary>
    /// A dictionary to map artifact types to their corresponding item instances.
    /// </summary>
    protected static readonly Dictionary<Buildings, RecruitBuildingModel> BuildingsLookup = new()
    {
        { Buildings.Farm, new Farm() },
        { Buildings.HouseOfArchers, new HouseOfArchers() }, 
        { Buildings.KnightsHouse, new KnightsHouse() }, 
    };
}