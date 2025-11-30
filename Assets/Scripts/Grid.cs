using System;
using System.Diagnostics;

/// <summary>
/// A grid represents the area which cells inhabit.
/// </summary>
public struct Grid
{
    // All the cells actually live here
    Cell[] cells;
    int neighbourhoodSize;
    FunctionLibrary.FunctionName functionName;
    FunctionLibrary.Function function;

    // NOTE: neighbourhoodSize refers to how many cells left/right are in the neighbourhood

    // Reference the cells by calling the grid as an array
    public Cell this [int index]
    {
        get => cells[index];
        set => cells[index] = value;
    }
    public readonly int Size => cells.Length;
    
    // Load the grid
    public void Initialise(int size, Cell[] initialState, FunctionLibrary.FunctionName funcName, int nSize = 1)
    {
        cells = new Cell[size];
        for (int i = 0; i <= size - 1; i++)
        {
            cells[i].state = initialState[i].state;
        }

        neighbourhoodSize = nSize;
        functionName = funcName;
        function = FunctionLibrary.GetFunction(functionName);
    }

    // Unload the grid
    public void Dispose() => cells = null;


    // Update the state of cell i based on its neighbourhood
    readonly CellState UpdateCell(int i, Cell[] neighbourhood)
    {
        CellState outState = cells[i].state;

        // Call a function to use here
        outState = function(cells[i], neighbourhood);

        return outState;
    }

    // Progress through the next time step of the grid
    public readonly void Update()
    {
        Cell[] newCells = new Cell[cells.Length];
        Array.Copy(cells, newCells, cells.Length);

        for (int i = 0;i < cells.Length; i++)
        {
            Cell[] neighbourhood = GetCellNeighbourhood(i);

            CellState newState = UpdateCell(i, neighbourhood);
            newCells[i].state = newState;
        }
        Array.Copy(newCells, cells, newCells.Length);
    }

    // Get the neighbourhood of cell index i
    private readonly Cell[] GetCellNeighbourhood(int i)
    {
        Cell[] neighbourhood = new Cell[neighbourhoodSize * 2];

        // Set default state
        // Maybe this should be in Cell's constructor
        for (int k = 0; k < neighbourhoodSize + 1; k++)
        {
            neighbourhood[k].state = CellState.Dead;
        }

        for (int j = 1; j < neighbourhoodSize + 1; j++)
        {
            if (i - j >= 0)
            {
                neighbourhood[j - 1] = cells[i - j];
            }
            if (i + j < cells.Length)
            {
                neighbourhood[j] = cells[i + j];
            }
        }

        return neighbourhood;
    }
}