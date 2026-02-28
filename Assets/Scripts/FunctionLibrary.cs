using AOT;
using Unity.Burst;

/// <summary>
/// Holds all the types of functions you could use in a Grid's UpdateCell call.
/// </summary>
[BurstCompile(CompileSynchronously = true)]
public static class FunctionLibrary
{
    // All functions will return the cell state for the given cell
    public delegate void Function(int index, in Grid grid, out Cell output);
    public enum FunctionName { SwitchState , InheritNeighbourSingular , GameOfLife , ProcessHealthDecay}

    static Function[] functions = { SwitchState , InheritNeighbourSingular , GameOfLife , ProcessHealthDecay};

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
    public static void SwitchState(int index, in Grid grid, out Cell output)
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
    public static void InheritNeighbourSingular(int index, in Grid grid, out Cell output)
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
    public static void GameOfLife(int index, in Grid grid, out Cell output)
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

    /// <summary>
    /// Reduces the health for the cell index for each neighbour with a healthDecayStack. Also decrement that value for this cell.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static void ProcessHealthDecay(int index, in Grid grid, out Cell output)
    {
        output = grid[index];
        output.healthDecayStack--;

        for (int i = 0; i < grid.NeighbourhoodLength + 1; i++)
        {
            if (grid.TryGetNeighbourhoodCellIndex(index, i, out int neighbour) && neighbour != index)
            {
                if (grid[neighbour].healthDecayStack > 0) output.health--;
            }
        }
    }
}

