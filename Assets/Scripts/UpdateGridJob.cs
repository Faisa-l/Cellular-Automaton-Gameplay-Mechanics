using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;


/// <summary>
/// Updates each cell in the given grid
/// </summary>
[BurstCompile(CompileSynchronously = true)]
public struct UpdateGridJob : IJobFor
{
    /* Ran on each cell in grid.cells.
     * Run the cell function to determine next state.
     * Update this cell in the output grid.
     */

    // Inputs
    [ReadOnly]
    public Grid grid;
    public FunctionPointer<FunctionLibrary.Function> cellFunction;

    // Outputs
    [WriteOnly, NativeDisableParallelForRestriction]
    public NativeArray<Cell> outCells;

    public void Execute(int index)
    {
        CellState outState = cellFunction.Invoke(index, in grid);
        
        // Write to output
        var cell = new Cell { state = outState };
        outCells[index] = cell;
    }
}
