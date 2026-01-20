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

    Grid grid;
    GridVisualiser visualiser;

    private void OnDisable()
    {
        grid.Dispose();
    }

    public void Initialise()
    {
        grid = new Grid();
        if (!TryGetComponent(out visualiser))
        {
            visualiser = gameObject.AddComponent<GridVisualiser>();
        }

        grid.Initialise(rows, columns, functionName, neighbourhoodSize);
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
            ToggleCellLiving(index);
            visualiser.UpdateVisualisation();
        }

    }

    private void ToggleCellLiving(int index)
    {
        if (grid[index].state == CellState.Alive)
        {
            grid.UpdateCellState(index, CellState.Dead);
        }
        else if (grid[index].state == CellState.Dead)
        {
            grid.UpdateCellState(index, CellState.Alive);
        }
    }

}
