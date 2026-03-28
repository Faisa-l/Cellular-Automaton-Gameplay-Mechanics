using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

/// <summary>
/// Holds all the types of functions you could use in a <see cref="Grid.Update"/> call.
/// Each function is to be handled as a function pointer to be called in the <see cref="UpdateGridJob"/>.
/// Generally, the functions will only do work so long as the cell it's working on is alive.
/// </summary>
[BurstCompile(CompileSynchronously = true)]
public static class FunctionLibrary
{
    // All functions will return the cell state for the given cell
    public delegate void Function(int index, in Grid grid, in NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests, out Cell output);
    public enum FunctionName { SwitchState , InheritNeighbourSingular , GameOfLife , ProcessHealthDecay , UpdateWorldSimulation}

    static Function[] functions = { SwitchState , InheritNeighbourSingular , GameOfLife , ProcessHealthDecay , UpdateWorldSimulation};

    // Grid the functions will work in reference to
    public static Grid grid;

    /// <summary>
    /// Return the function callback for a grid function.
    /// </summary>
    /// <param name="name"> Enum of the function's name. </param>
    /// <returns> Callback to the function of that name. </returns>
    public static Function GetFunction(FunctionName name) => functions[(int)name];

    /// <summary>
    /// Switches the state of the target cell to the opposite state.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static void SwitchState(int index, in Grid grid, in NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests, out Cell output)
    {
        output = new()
        {
            state = grid[index].state == CellState.Dead ? CellState.Alive : CellState.Dead
        };
    }


    /// <summary>
    /// Return alive if only one neighbour is alive.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static void InheritNeighbourSingular(int index, in Grid grid, in NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests, out Cell output)
    {
        // For now this is going unused

        /* To remove
        int j = 0;
        foreach (var i in neighbourhood)
        {
            if (grid[i].state == CellState.Alive) j++;
        }*/
        output = new();
    }

    /// <summary>
    /// Return alive if the count of alive neighbours is in the range [2,3].
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static void GameOfLife(int index, in Grid grid, in NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests, out Cell output)
    {
        int count = 0;
        output = grid[index];

        for (int i = 0; i < grid.NeighbourhoodLength + 1; i++)
        {
            grid.TryGetNeighbourhoodCellIndex(index, i, out int neighbour);
            if (neighbour != -1 && neighbour != index)
            {
                count = (grid[neighbour].state == CellState.Alive) ? count + 1 : count;
            }
        }

        if (grid[index].state == CellState.Alive)
        {
            output.state = count == 2 || count == 3 ? CellState.Alive : CellState.Dead;
        }
        else
        {
            output.state = count == 3 ? CellState.Alive: CellState.Dead;
        }
    }

    // Possibly need to make change in output.health - 1f -> recalculate 1f 

    /// <summary>
    /// Reduces the health for the cell index for each neighbour with a <see cref="Cell.healthDecayStack"/>. Also decrement that value for this cell.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static void ProcessHealthDecay(int index, in Grid grid, in NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests, out Cell output)
    {
        output = grid[index];
        output.healthDecayStack = math.max(0f, output.healthDecayStack - 1f);

        for (int i = 0; i < grid.NeighbourhoodLength + 1; i++)
        {
            if (grid.TryGetNeighbourhoodCellIndex(index, i, out int neighbour) && neighbour != index)
            {
                if (grid[neighbour].healthDecayStack > 0)
                { 
                    output.health = math.max(0f, output.health - grid[neighbour].decayDamage);
                }
            }
        }
    }

    /// <summary>
    /// Updates a cell's values based on the properties of itself and its neighbours, and schedules cell movements.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static void UpdateWorldSimulation(int index, in Grid grid, in NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests, out Cell output)
    {
        output = grid[index];
        for (int i = 0; i < grid.NeighbourhoodLength + 1; i++)
        {
            if (grid.TryGetNeighbourhoodCellIndex(index, i, out int neighbour) && neighbour != index)
            {

                // This cell is empty checks
                if (grid[index].isEmpty == 1)
                {
                    // Liquid checks
                    if (grid[neighbour].liquidLevel > 0) BecomeLiquidFromNeighbour(neighbour, in grid, ref output);

                    // --- MOVEMENT CHECK ---
                    UpdateCellDrift(index, in grid, in movementRequests, i, neighbour);
                }
                else
                {
                    if (grid[index].liquidLevel > 0) UpdateLiquidLevel(ref output);
                }
            }
        }

        // Create movement requests 
        static void UpdateCellDrift(int index, in Grid grid, in NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests, int i, int neighbour)
        {
            int2 neighbourDrift = grid[neighbour].drift;
            switch (i)
            {
                case 1: // Up
                    if (neighbourDrift.y == -1) movementRequests.Add(index, neighbour);
                    break;

                case 3: // Left
                    if (neighbourDrift.x == 1) movementRequests.Add(index, neighbour);
                    break;

                case 5: // Right
                    if (neighbourDrift.x == -1) movementRequests.Add(index, neighbour);
                    break;

                case 7: // Down
                    if (neighbourDrift.y == 1) movementRequests.Add(index, neighbour);
                    break;

                default:
                    break;
            }
        }

        // When cell is empty, it is replaced by the liquid of the neighbour
        static void BecomeLiquidFromNeighbour(int neighbour, in Grid grid, ref Cell output)
        {
            float neighbourLiquids = grid[neighbour].liquidLevel;
            output = grid[neighbour];
            output.liquidLevel -= 1;
        }

        // If the cell's liquid were to update then the level should be reduced.
        static void UpdateLiquidLevel(ref Cell output) => output.liquidLevel -= 1;
    }
}

