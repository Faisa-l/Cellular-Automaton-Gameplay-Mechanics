/// <summary>
/// Holds all the types of functions you could use in a Grid's UpdateCell call.
/// </summary>
public static class FunctionLibrary
{
    // All functions will return the cell state for the given cell
    public delegate CellState Function(Cell target, Cell[] neighbourhood);
    public enum FunctionName { SwitchState , InheritNeighbourSingular , GameOfLife}

    static Function[] functions = { SwitchState , InheritNeighbourSingular , GameOfLife};

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
    public static CellState SwitchState(Cell target, Cell[] neighbourhood)
    {
        CellState outState = target.state == CellState.Dead ? CellState.Alive : CellState.Dead;

        return outState;
    }

    /// <summary>
    /// Return alive if only one neighbour is alive.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="neighbourhood"></param>
    public static CellState InheritNeighbourSingular(Cell target, Cell[] neighbourhood)
    {
        int i = 0;
    
        foreach (var cell in neighbourhood)
        {
            if (cell.state == CellState.Alive) i++;
        }

        return i == 1 ? CellState.Alive : CellState.Dead;
    }

    /// <summary>
    /// Return alive if the count of alive neighbours is in the range [2,3].
    /// </summary>
    /// <param name="target"></param>
    /// <param name="neighbourhood"></param>
    public static CellState GameOfLife(Cell target, Cell[] neighbourhood)
    {
        int count = 0;

        foreach (var cell in neighbourhood)
        {
            if (cell.state == CellState.Alive) count++;
        }

        if (target.state == CellState.Alive)
        {
            return count == 2 || count == 3 ? CellState.Alive : CellState.Dead ;
        }
        else
        { 
            return count == 3 ? CellState.Alive: CellState.Dead;
        }
    }
}

