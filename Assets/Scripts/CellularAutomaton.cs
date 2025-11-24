using UnityEngine;


/// <summary>
/// Component which handles the computation and drawing of a cellular automaton
/// </summary>
public class CellularAutomaton : MonoBehaviour
{
    [SerializeField, Tooltip("Prefab used to display each cell")]
    GameObject cellPrefab;

    [SerializeField, Tooltip("Size of the cellular automaton"), Range(0, 50)]
    int size;

    [SerializeField]
    Cell[] initialState;

    Grid grid;
    GridVisualiser visualiser;

    void Initialise()
    {
        size = initialState.Length;
        grid = new Grid();
        if (!TryGetComponent<GridVisualiser>(out visualiser))
        {
            visualiser = gameObject.AddComponent<GridVisualiser>();
        }

        grid.Initialise(size, initialState);
        visualiser.Initialise(cellPrefab, grid);

        visualiser.Draw();
    }

    private void Awake()
    {
        Initialise();
    }
}
