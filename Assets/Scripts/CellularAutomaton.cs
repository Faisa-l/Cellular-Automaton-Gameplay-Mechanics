using UnityEngine;

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
    Cell[] initialState;

    [SerializeField]
    FunctionName functionName;

    Grid grid;
    GridVisualiser visualiser;

    public void Initialise()
    {
        grid = new Grid();
        if (!TryGetComponent(out visualiser))
        {
            visualiser = gameObject.AddComponent<GridVisualiser>();
        }

        grid.Initialise(rows, columns, initialState, functionName, neighbourhoodSize);
        visualiser.Initialise(cellPrefab, grid);
        visualiser.Draw();
    }

    public void NextTick()
    {
        grid.Update();
        visualiser.UpdateVisualisation();
    }

}
