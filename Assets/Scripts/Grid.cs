/// <summary>
/// A grid represents the area which cells inhabit.
/// </summary>
public struct Grid
{
    // All the cells actually live here
    Cell[] cells;

    // Load the grid
    public void Initialise(int size)
    {
        cells = new Cell[size];
    }

    // Unload the grid
    public void Dispose() => cells = null;

    // Reference the cells by calling the grid as an array
    public Cell this [int index]
    {
        get => cells[index];
        set => cells[index] = value;
    }

    public int Size => cells.Length;
}

