
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
        health,
        healthDecayStack;
}


