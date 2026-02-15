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

    public Grid Grid { get; private set; }
    public GridVisualiser Visualiser { get; private set; }
    bool updateEachFrame;

    float UpdateRepeatRate => 1 / (float)updatesPerSecond;

    private void OnDisable()
    {
        Grid.Dispose();
    }

    public void Initialise()
    {
        updateEachFrame = false;
        Grid = new Grid();
        if (!TryGetComponent(out GridVisualiser v))
        {
            Visualiser = gameObject.AddComponent<GridVisualiser>();
        }
        else
        {
            Visualiser = v;
        }

        Grid.Initialise(rows, columns, functionName, neighbourhoodSize);
        Visualiser.Initialise(cellPrefab, Grid);
        Visualiser.Draw();
    }

    public void NextTick()
    {
        Grid.Update();
        Visualiser.UpdateVisualisation();
    }

    // For input call
    public void OnTouchedCell()
    {
        // Get a ray from the mouse to the XZ plane.
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Visualiser.TryGetTouchedCellIndex(ray, out int index))
        {
            ToggleCellLiving(index);
            Visualiser.UpdateVisualisation();
        }

    }

    private void ToggleCellLiving(int index)
    {
        if (Grid[index].state == CellState.Alive)
        {
            Grid.UpdateCellState(index, CellState.Dead);
        }
        else if (Grid[index].state == CellState.Dead)
        {
            Grid.UpdateCellState(index, CellState.Alive);
        }
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
