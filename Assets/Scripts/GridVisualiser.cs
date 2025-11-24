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
    static MaterialPropertyBlock block;

    // Load the visualiser with data
    public void Initialise(GameObject prefab, Grid grid)
    {
        this.prefab = prefab;
        this.grid = grid;

        block = new MaterialPropertyBlock();
    }

    // Draws the grid 
    public void Draw()
    {
        drawnObjects ??= new GameObject[grid.Size];

        for (int i = 0; i < grid.Size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.localPosition = Vector3.right * i;
            ApplyCellColour(obj);
            drawnObjects[i] = obj;
        }
    }

    void ApplyCellColour(GameObject obj)
    {
        var renderer = obj.GetComponent<Renderer>();
        renderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", Random.ColorHSV());
        renderer.SetPropertyBlock(block);
    }
}

