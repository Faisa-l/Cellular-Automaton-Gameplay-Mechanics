
using Unity.Collections;

/// <summary>
/// Defines the states of a cell in a grid.
/// Cells define data which can be used to compute their next state.
/// </summary>
/// 
[System.Serializable]
public struct Cell
{
    // Primary state of the cell
    public CellState state;

    // All types of cell data
    public float
        health,                 // Health of a cell.
        healthDecayStack,       // If the cell reduces the health of its neighbours.
        damage,                 // Damage the cell can deal to others (health damage).
        appliedDecayStack,
        decayDamage;

}


