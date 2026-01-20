using AOT;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// Holds all the types of functions you could use in a Grid's UpdateCell call.
/// </summary>
[BurstCompile(CompileSynchronously = true)]
public static class FunctionLibrary
{
    // All functions will return the cell state for the given cell
    public delegate CellState Function(int index, in Grid grid);
    public enum FunctionName { SwitchState , InheritNeighbourSingular , GameOfLife}

    static Function[] functions = { SwitchState , InheritNeighbourSingular , GameOfLife};

    // Grid the functions will work in reference to
    public static Grid grid;

    /// <summary>
    /// Return the function callback for a grid function.
    /// </summary>
    /// <param name="name"> Enum of the function's name. </param>
    /// <returns> Callback to the function of that name. </returns>
    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    /// <summary>
    /// Switches the state of the target cell to the opposite state.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static CellState SwitchState(int index, in Grid grid)
    {
        CellState outState = grid[index].state == CellState.Dead ? CellState.Alive : CellState.Dead;

        return outState;
    }

    /// <summary>
    /// Return alive if only one neighbour is alive.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="neighbourhood"></param>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static CellState InheritNeighbourSingular(int index, in Grid grid)
    {
        int j = 0;
    
        /* To remove
        foreach (var i in neighbourhood)
        {
            if (grid[i].state == CellState.Alive) j++;
        }*/

        return j == 1 ? CellState.Alive : CellState.Dead;
    }

    /// <summary>
    /// Return alive if the count of alive neighbours is in the range [2,3].
    /// </summary>
    /// <param name="target"></param>
    /// <param name="neighbourhood"></param>
    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(Function))]
    public static CellState GameOfLife(int index, in Grid grid)
    {
        int count = 0;

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
            return count == 2 || count == 3 ? CellState.Alive : CellState.Dead;
        }
        else
        { 
            return count == 3 ? CellState.Alive: CellState.Dead;
        }
    }
}

