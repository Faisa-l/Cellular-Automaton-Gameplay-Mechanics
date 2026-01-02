/// <summary>
/// Holds all the types of functions you could use in a Grid's UpdateCell call.
/// </summary>
public static class FunctionLibrary
{
    // All functions will return the cell state for the given cell
    public delegate CellState Function(Cell target, Cell[] neighbourhood);
    public enum FunctionName { SwitchState , InheritNeighbourSingular}

    static Function[] functions = { SwitchState , InheritNeighbourSingular};

    // Return the function callback
    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    public static CellState SwitchState(Cell target, Cell[] neighbourhood)
    {
        CellState outState = target.state == CellState.Dead ? CellState.Alive : CellState.Dead;

        return outState;
    }

    // Return alive if only one neighbour is alive
    public static CellState InheritNeighbourSingular(Cell target, Cell[] neighbourhood)
    {
        int i = 0;
    
        foreach (var cell in neighbourhood)
        {
            if (cell.state == CellState.Alive) i++;
        }

        return i == 1 ? CellState.Alive : CellState.Dead;
    }
}

