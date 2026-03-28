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

    [ReadOnly]
    public FunctionPointer<FunctionLibrary.Function> updateFunction;

    // Outputs
    [WriteOnly, NativeDisableParallelForRestriction]
    public NativeArray<Cell> output;

    [WriteOnly]
    public NativeList<int>.ParallelWriter updatedIndices;

    [WriteOnly, NativeDisableParallelForRestriction]
    public NativeParallelMultiHashMap<int, int>.ParallelWriter movementRequests;

    public void Execute(int index)
    {
        // update function needs to take in movement requests array 
        updateFunction.Invoke(index, in grid, in movementRequests, out Cell newCell);
        output[index] = newCell;

        if (newCell != grid[index]) updatedIndices.AddNoResize(index);
    }
}
