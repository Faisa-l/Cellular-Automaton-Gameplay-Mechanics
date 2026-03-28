using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Defines the states of a cell in a grid.
/// Cells define data which can be used to compute their next state.
/// </summary>
/// 
[System.Serializable]
public partial struct Cell
{
    // Primary state of the cell
    public CellState state;
    
    public CellMaterial material;

    // All types of cell data are expressed as a float.
    public float
        health,                 // Health of a cell.
        healthDecayStack,       // If the cell reduces the health of its neighbours.
        damage,                 // Damage the cell can deal to others (health damage).
        appliedDecayStack,
        decayDamage,
        liquidLevel;

    public int2 drift;          // How much the cell should move in either direction every update

    // bools are represented with ints (technically by bytes but for our purposes they are interchangeable)
    [Range(0, 1)]
    public int isEmpty;        // If another cell can move into this cell

}

// Default cell types
public partial struct Cell
{
    public static Cell DefaultRock = new ()
    {
        state = CellState.Alive,
        material = CellMaterial.Rock,
        health = 0,
        healthDecayStack = 0,
        damage = 0,
        appliedDecayStack = 0,
        decayDamage = 0,
        drift = 0,
        isEmpty = 0
    };

    public static Cell DefaultAir = new ()
    {
        state = CellState.Alive,
        material = CellMaterial.Air,
        health = 0,
        healthDecayStack = 0,
        damage = 0,
        appliedDecayStack = 0,
        decayDamage = 0,
        drift = 0,
        isEmpty = 1
    };

    public static Cell DefaultWater = new ()
    {
        state = CellState.Alive,
        material = CellMaterial.Water,
        health = 0,
        healthDecayStack = 0,
        damage = 0,
        appliedDecayStack = 0,
        decayDamage = 0,
        drift = 0,
        isEmpty = 0
    };
}


