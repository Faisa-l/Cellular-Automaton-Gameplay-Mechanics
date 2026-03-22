using System;
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

    [SerializeField, Tooltip("Number of roaws in the cellular automaton"), Range(1, 256)]
    int rows = 6;

    [SerializeField, Tooltip("Size of the rows in the cellular automaton"), Range(1, 256)]
    int columns = 8;

    [SerializeField, Range(1, 5)]
    int neighbourhoodSize;

    [SerializeField]
    FunctionName functionName;

    [SerializeField, Min(0)]
    int updatesPerSecond = 24;

    [SerializeField]
    bool repeatingUpdate;

    [Header("Cells to add")]
    [SerializeField]
    Cell startingCell;

    [SerializeField]
    Cell paintedCell;

    Grid grid;
    bool updateEachFrame = false;
    GridVisualiser visualiser;

    public event Action OnUpdate;
    public GridVisualiser Visuasliser => visualiser;
    public ref Grid Grid => ref GetGrid();
    float UpdateRepeatRate => 1f / updatesPerSecond;

    ref Grid GetGrid() => ref grid;

    private void OnValidate()
    {
        // Clamp drift values to [-1, 1]
        ref var s_drift = ref startingCell.drift;
        if (s_drift.x > 1) s_drift.x = 1;
        if (s_drift.x < -1) s_drift.x = -1;
        if (s_drift.y > 1) s_drift.y = 1;
        if (s_drift.y < -1) s_drift.y = -1;

        ref var p_drift = ref paintedCell.drift;
        if (p_drift.x > 1) p_drift.x = 1;
        if (p_drift.x < -1) p_drift.x = -1;
        if (p_drift.y > 1) p_drift.y = 1;
        if (p_drift.y < -1) p_drift.y = -1;
    }

    private void Start()
    {
        Initialise();
    }

    private void OnEnable()
    {
        if (repeatingUpdate)
        {
            InvokeRepeating(nameof(NextTick), 0f, UpdateRepeatRate);
        }
    }

    private void OnDisable()
    {
        grid.Dispose();

        if (repeatingUpdate)
        {
            CancelInvoke(nameof(NextTick));
        }
    }

    public void Initialise()
    {
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
        OnUpdate?.Invoke();
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
            OverrideCell(index, paintedCell);
        }

    }

    public void OverrideCell(int index, Cell newCell)
    {
        grid[index] = newCell;
        visualiser.UpdateVisualisation();
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

    /// <summary>
    /// Converts the index of a cell in a base automaton to the index of the cell in another automaton, based on the world-space position of the cells.
    /// </summary>
    /// <param name="targetAutomaton">Automaton of the converted cell index.</param>
    /// <param name="baseIndex">Cell index of the base automaton to convert.</param>
    /// <param name="projectedIndex">The converted index of baseIndex in the target automaton.</param>
    public bool TryProjectCellOntoOtherAutomaton(CellularAutomaton targetAutomaton, int baseIndex, out int projectedIndex)
    {
        projectedIndex = -1;
        if (!visualiser.TryGetCellPosition(baseIndex, out var position)) return false;
        return targetAutomaton.visualiser.TryGetTouchedCellIndex(new Ray(position + (Vector3.up * 2f), Vector3.down), out projectedIndex);
    }
}