using UnityEngine;
using UnityEngine.InputSystem;
using static FunctionLibrary;

/// <summary>
/// Component which handles the computation and drawing of a cellular automaton
/// </summary>
public class CellularAutomaton : MonoBehaviour
{
    [SerializeField, Tooltip("Prefab used to display each cell")]
    GameObject cellPrefab;

    [SerializeField, Tooltip("Number of roaws in the cellular automaton"), Range(1, 50)]
    int rows = 6;

    [SerializeField, Tooltip("Size of the rows in the cellular automaton"), Range(1, 50)]
    int columns = 8;

    [SerializeField, Range(1, 5)]
    int neighbourhoodSize;

    [SerializeField]
    FunctionName functionName;

    [SerializeField, Min(0)]
    int updatesPerSecond = 24;

    [Header("Cells to add")]
    [SerializeField]
    Cell startingCell;

    [SerializeField]
    Cell paintedCell;

    Grid grid;
    GridVisualiser visualiser;
    bool updateEachFrame;

    public ref Grid Grid => ref grid;
    float UpdateRepeatRate => 1f / updatesPerSecond;
    public void UpdateVisualisation() => visualiser.UpdateVisualisation();


    private void OnDisable()
    {
        grid.Dispose();
    }

    public void Initialise()
    {
        updateEachFrame = false;
        grid = new Grid();
        if (!TryGetComponent(out GridVisualiser v))
        {
            visualiser = gameObject.AddComponent<GridVisualiser>();
        }
        else
        {
            visualiser = v;
        }

        grid.Initialise(rows, columns, functionName, startingCell, neighbourhoodSize);
        visualiser.Initialise(cellPrefab, grid);
        visualiser.Draw();
    }

    public void NextTick()
    {
        grid.Update();
        visualiser.UpdateVisualisation();
    }

    // For input call
    public void OnTouchedCell()
    {
        // Get a ray from the mouse to the XZ plane.
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (visualiser.TryGetTouchedCellIndex(ray, out int index))
        {
            OverrideCell(index);
            visualiser.UpdateVisualisation();
        }

    }

    private void OverrideCell(int index)
    {
        grid[index] = paintedCell;
    }

    public void ToggleRepeatingUpdate()
    {
        if (!updateEachFrame)
        {
            updateEachFrame = true;
            InvokeRepeating(nameof(NextTick), 0f, UpdateRepeatRate);
        }
        else
        {
            updateEachFrame = false;
            CancelInvoke(nameof(NextTick));
        }
    }
}
