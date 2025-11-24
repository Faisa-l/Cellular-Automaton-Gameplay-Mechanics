/// <summary>
/// A grid represents the area which cells inhabit.
/// </summary>
public struct Grid
{
    // All the cells actually live here
    Cell[] cells;

    // Load the grid
    public void Initialise(int size, Cell[] initialState)
    {
        cells = new Cell[size];
        for (int i = 0; i <= size - 1; i++)
        {
            cells[i].state = initialState[i].state;
        }
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

