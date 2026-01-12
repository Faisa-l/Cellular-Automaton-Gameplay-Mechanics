using System;

using static FunctionLibrary;

/// <summary>
/// A grid represents the area which cells inhabit.
/// </summary>
public struct Grid
{
    // All the cells actually live here
    Cell[] cells;
    int neighbourhoodSize;
    FunctionName functionName;
    Function function;

    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public readonly int Size => (Rows * Columns);

    // NOTE: neighbourhoodSize refers to how many cells left/right are in the neighbourhood

    // Reference the cells by calling the grid as an array
    public Cell this [int index]
    {
        get => cells[index];
        set => cells[index] = value;
    }

    /// <summary>
    /// Load the grid.
    /// </summary>
    /// <param name="rows"> Number of rows. </param>
    /// <param name="columns"> Length of each row. </param>
    /// <param name="initialState"> Starting state of the Grid. </param>
    /// <param name="funcName"> Function name to perform on the neighbourhood. </param>
    /// <param name="nSize"> Size of the neighbourhood, measured as the distance from a given cell. </param>
    public void Initialise(int rows, int columns, Cell[] initialState, FunctionName funcName, int nSize = 1)
    {
        Rows = rows;
        Columns = columns;
        cells = new Cell[Size];
        for (int i = 0; i <= Size - 1; i++)
        {
            cells[i].state = CellState.Dead;
        }

        neighbourhoodSize = nSize;
        functionName = funcName;
        function = GetFunction(functionName);
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

    // Get the neighbourhood of the cell index.
    // Possible change: Return int[] of neighbourhood indexes
    private readonly Cell[] GetCellNeighbourhood(int index)
    {
        // CURRENTLY ONLY SUPPORTS NEIGHBOURHOOD SIZE OF 1
        // Using a lazy method: Combine neighbours of the individual rows above-below and this.
        Cell[] neighbourhood = new Cell[4 * neighbourhoodSize * (neighbourhoodSize + 1)];
        int pointer = 0;
        GetRowColumn(index, out int row, out _);

        // Check rows above and below
        for (int i = 1; i < neighbourhoodSize + 1; i++)
        {
            if (row - i >= 0)
            {
                var nbrs = new Cell[(neighbourhoodSize * 2) + 1];
                GetRowNeighbours(index - (Columns * i), ref nbrs, true);

                Array.Copy(nbrs, 0, neighbourhood, pointer, nbrs.Length);
                pointer += nbrs.Length;
            }

            if (row + i < Rows)
            {
                var nbrs = new Cell[(neighbourhoodSize * 2) + 1];
                GetRowNeighbours(index + (Columns * i), ref nbrs, true);

                Array.Copy(nbrs, 0, neighbourhood, pointer, nbrs.Length);
                pointer += nbrs.Length;
            }
        }

        // Check this row
        Cell[] thisNbrs = new Cell[neighbourhoodSize * 2];
        GetRowNeighbours(index, ref thisNbrs);
        Array.Copy(thisNbrs, 0, neighbourhood, pointer, thisNbrs.Length);

        return neighbourhood;
    }

    /// <summary>
    /// Returns the left-right neighbourhood of cells at a given index.
    /// </summary>
    /// <param name="index">Cell index to search.</param>
    /// <param name="neighbours">Cell array to write into.</param>
    /// <param name="andSelf">Whether this should write the indexed cell into the array, as the last element.</param>
    private readonly void GetRowNeighbours(int index, ref Cell[] neighbours, bool andSelf = false)
    {
        if (neighbourhoodSize == 0) return;

        int p = 0;
        for (int i = 1; i < neighbourhoodSize + 1; i++)
        {
            GetRowColumn(index, out int row, out _);

            // Left of
            if (index - i >= ((row - 1) * Columns) + Columns)
            {
                neighbours[p] = cells[index - i];
                p++;
            }
            // Right of 
            if (index + i < (row * Columns) + Columns)
            {
                neighbours[p] = cells[index + i];
                p++;
            }
        }
        if (andSelf)
        {
            neighbours[^1] = cells[index];
        }
    }

    /// <summary>
    /// Returns the row and column of a given cell index.
    /// </summary>
    /// <param name="i">Index of the cell.</param>
    /// <param name="row">Row of that index.</param>
    /// <param name="column">Column of that index.</param>
    public readonly void GetRowColumn(int i, out int row, out int column)
    {
        row = i / Columns;
        column = i % Columns;
    }

    /// <summary>
    /// Return the cell array of an entire row.
    /// </summary>
    public readonly Cell[] GetRow(int row)
    {
        Cell[] arr = new Cell[Columns];
        int pointer = (row - 1) * Columns;
        
        Array.Copy(cells, pointer, arr, 0, Columns);
        return arr;
    }

    /// <summary>
    /// Returns the index for a cell at a given row and column.
    /// </summary>
    /// <param name="row">Row of cell.</param>
    /// <param name="column">Column of cell.</param>
    /// <param name="index">Output index of the cell.</param>
    /// <returns>Whether the cell exists.</returns>
    public readonly bool TryGetCellIndex(int row, int column, out int index)
    {
        // Row and column must be in range
        bool valid = row >= 0 && Rows > row && column >= 0 && Columns > column;
        index = valid ? (row * Columns) + column : -1;

        return valid;
    }

    public readonly void UpdateCellState(int index, CellState state) => cells[index].state = state;
}