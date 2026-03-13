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
    public FunctionPointer<FunctionLibrary.Function> updateFunction;

    // Outputs
    [WriteOnly, NativeDisableParallelForRestriction]
    public NativeArray<Cell> output;

    public void Execute(int index)
    {
        updateFunction.Invoke(index, in grid, out Cell newCell);
        output[index] = newCell;
    }
}
