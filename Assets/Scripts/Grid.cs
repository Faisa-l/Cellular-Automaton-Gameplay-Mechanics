using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using static FunctionLibrary;
using Random = Unity.Mathematics.Random;

/// <summary>
/// A grid represents the area which cells inhabit.
/// </summary>
public struct Grid
{
    // Cells live in Grid.cells. Each cell holds its own data.
    // Data is an array of floats. Each index maps to data described in DataMap.
    // NOTE: neighbourhoodSize refers to how many cells left/right are in the neighbourhood

    NativeArray<Cell> cells;
    int neighbourhoodSize;
    FunctionName functionName;
    FunctionPointer<Function> function;

    static Random rand = Random.CreateFromIndex((uint)Time.time);

    // Special const-like variables for cell interactions; these can be adjusted on each grid instance 
    public float MELTING_LIQUID;    // If cell melts, how much liquid should it take
    public float MELTING_HEAT;      // If cell melts, how much hotter should it get

    public Cell BackgroundCell { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public readonly int Size => Rows * Columns;
    public readonly int NeighbourhoodLength => 4 * neighbourhoodSize * (neighbourhoodSize + 1);
    public void UpdateCellState(int index, CellState state) => cells[index] = new Cell { state = state };


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
    /// <param name="funcName"> Function name to perform on the neighbourhood. </param>
    /// <param name="nSize"> Size of the neighbourhood, measured as the distance from a given cell. </param>
    public void Initialise(int rows, int columns, FunctionName funcName, Cell defaultCell, int nSize = 1)
    {
        Rows = rows;
        Columns = columns;
        MELTING_LIQUID = 3f;
        MELTING_HEAT = 8f;
        BackgroundCell = defaultCell;
        
        cells = new NativeArray<Cell>(Size, Allocator.Persistent);
        for (int i = 0; i <= Size - 1; i++)
        {
            cells[i] = defaultCell; 
        }

        neighbourhoodSize = nSize;
        functionName = funcName;
        function = BurstCompiler.CompileFunctionPointer(GetFunction(functionName));
    }

    // Unload the grid
    public void Dispose() => cells.Dispose();

    // Progress through the next time step of the grid
    // Returns an array of cell indexes that were updated
    public readonly int[] Update()
    {
        // Output cell array to replace current grid
        NativeArray<Cell> newCells = new(cells.Length, Allocator.TempJob);
        NativeParallelMultiHashMap<int, int> movementRequests = new(cells.Length, Allocator.TempJob);
        NativeList<int> updatedIndicies = new(cells.Length, Allocator.TempJob);
        UpdateGridJob updateJob = new()
        {
            grid = this,
            updateFunction = function,
            movementRequests = movementRequests.AsParallelWriter(),
            updatedIndices = updatedIndicies.AsParallelWriter(),
            output = newCells
        };

        // Test different batch counts: rows, 1, 32, etc.
        updateJob.ScheduleParallelByRef(cells.Length, 1, default).Complete();

        // Resolve movement requests -> pick a random candidate for each request
        if (functionName == FunctionName.UpdateWorldSimulation)
        {
            var (requests, uniqueCount) = movementRequests.GetUniqueKeyArray(Allocator.TempJob);
            for (int i = 0; i < uniqueCount; i++)
            {
                // Iterate through a request's candidates and randomly decide to make the current candidate a winner
                int request = requests[i];
                var candidates = movementRequests.GetValuesForKey(request);
                int winner = PickRandomCandidate(candidates);
                newCells[request] = newCells[winner];
                newCells[winner] = BackgroundCell;
                updatedIndicies.AddNoResize(winner);
                updatedIndicies.AddNoResize(request);
            }
            requests.Dispose();
        }
        int[] updatedCells = updatedIndicies.AsArray().ToArray();

        // Replace current grid with new cells + cleanup
        cells.CopyFrom(newCells);
        newCells.Dispose();
        movementRequests.Dispose();
        updatedIndicies.Dispose();

        return updatedCells;
    }

    // i.e pick a random item from the candidates enumerator and return it as the winner
    private static int PickRandomCandidate(NativeParallelMultiHashMap<int, int>.Enumerator candidates)
    {
        int count = 0;
        candidates.MoveNext();
        int winner = candidates.Current;
        while (candidates.MoveNext())
        {
            count++;
            if (count == rand.NextInt(0, count)) winner = candidates.Current;
        }

        return winner;
    }

    public readonly bool TryGetNeighbourhoodCellIndex(int index, int neighbourhoodIndex, out int neighbour)
    {
        // Effectively converting the local neighbourhood grid around the index cell to the global grid
        GetRowColumn(index, out int targetRow, out int targetCol);

        // Get local row-column indexes of the neighbourhood index cell
        int span = (int)Math.Sqrt((float)NeighbourhoodLength + 1);
        int neighbourRow = neighbourhoodIndex / span;
        int neighbourCol = neighbourhoodIndex % span;

        // Convert local indexes to global space
        targetRow += neighbourRow - neighbourhoodSize;
        targetCol += neighbourCol - neighbourhoodSize;

        // Update neighbour with new indexes
        return TryGetCellIndex(targetRow, targetCol, out neighbour);
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

    public readonly bool TryGetCellIndex(Vector2Int position, out int index) => TryGetCellIndex(position.y, position.x, out index);


    #region Unused

    // Update the state of cell i based on its neighbourhood
    readonly CellState UpdateCell(int i, Cell[] neighbourhood)
    {
        CellState outState = cells[i].state;

        // Call a function to use here
        // outState = function(cells[i], neighbourhood);

        return outState;
    }

    /*
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
    */

    /*
    /// <summary>
    /// Returns the left-right neighbourhood of cells at a given index.
    /// </summary>
    /// <param name="index">Cell index to search.</param>
    /// <param name="neighbours">Cell array to write into.</param>
    /// <param name="placeAt">Position in array to start writing neighbours at.</param>
    /// <param name="andSelf">Whether this should write the indexed cell into the array, as the last element.</param>
    private readonly void GetRowNeighbours(int index, ref CellNeighbourhood neighbours, int placeAt, bool andSelf = false)
    {
        if (neighbourhoodSize == 0) return;

        int p = placeAt;
        for (int i = 1; i < neighbourhoodSize + 1; i++)
        {
            GetRowColumn(index, out int row, out _);

            // Left of
            if (index - i >= ((row - 1) * Columns) + Columns)
            {
                neighbours[p] = index - i;
                p++;
            }
            // Right of 
            if (index + i < (row * Columns) + Columns)
            {
                neighbours[p] = index + i;
                p++;
            }
        }
        if (andSelf)
        {
            neighbours[neighbours.indexes.Length - 1] = index;
        }
    }

    // Get the neighbourhood of the cell index.
    // Possible change: Return int[] of neighbourhood indexes
    public readonly void GetCellNeighbourhood(int index, ref CellNeighbourhood neighbourhood)
    {
        // Using a lazy method: Combine neighbours of the individual rows above-below and this.
        // neighbourhood = new(4 * neighbourhoodSize * (neighbourhoodSize + 1), Allocator.TempJob);
        // Set the default values for the parition of the array which will be modified

        int pointer = -NeighbourhoodLength + ((1 + index) * NeighbourhoodLength);


        // Check rows above and below
        GetRowColumn(index, out int row, out _);
        for (int i = 1; i < neighbourhoodSize + 1; i++)
        {
            if (row - i >= 0)
            {
                GetRowNeighbours(index - (Columns * i), ref neighbourhood, pointer, true);
                pointer += (neighbourhoodSize * 2) + 1;

                // NativeArray<int>.Copy(nbrs, 0, neighbourhood, pointer, nbrs.Length);
                // nbrs.Dispose();
            }

            if (row + i < Rows)
            {
                GetRowNeighbours(index + (Columns * i), ref neighbourhood, pointer, true);
                pointer += (neighbourhoodSize * 2) + 1;
            }
        }

        // Check this row
        GetRowNeighbours(index, ref neighbourhood, pointer);
        // NativeArray<int>.Copy(thisNbrs, 0, neighbourhood, pointer, thisNbrs.Length);
    }
    */

    #endregion
}