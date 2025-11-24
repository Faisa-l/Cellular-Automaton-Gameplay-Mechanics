using UnityEngine;

/// <summary>
/// Handles drawing the grid itself.
/// Currently, will draw its grid using the prefab.
/// </summary>
public class GridVisualiser : MonoBehaviour
{
    GameObject prefab;
    Grid grid;

    GameObject[] drawnObjects;

    // Load the visualiser with data
    public void Initialise(GameObject prefab, Grid grid)
    {
        this.prefab = prefab;
        this.grid = grid;
    }

    // Draws the grid 
    public void Draw()
    {
        drawnObjects ??= new GameObject[grid.Size];

        for (int i = 0; i < grid.Size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.localPosition = Vector3.right * i;
            drawnObjects[i] = obj;
        }
    }
}

