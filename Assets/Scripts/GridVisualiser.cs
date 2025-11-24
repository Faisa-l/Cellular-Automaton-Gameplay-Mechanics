using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles drawing the grid itself.
/// Currently, will draw its grid using the prefab.
/// </summary>
public class GridVisualiser : MonoBehaviour
{
    [SerializeField]
    StateColor[] colorMap;

    GameObject prefab;
    Grid grid;

    GameObject[] drawnObjects;
    Dictionary<CellState, Color> colorPairs;
    static MaterialPropertyBlock block;

    // Load the visualiser with data
    public void Initialise(GameObject prefab, Grid grid)
    {
        this.prefab = prefab;
        this.grid = grid;
        block = new MaterialPropertyBlock();
        colorPairs = new Dictionary<CellState, Color>();

        foreach (var pair in colorMap)
        {
            colorPairs.Add(pair.state, pair.color);
        }
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
            ApplyCellColour(i);
        }
    }

    void ApplyCellColour(int i)
    {
        var obj = drawnObjects[i];
        var renderer = obj.GetComponent<Renderer>();
        colorPairs.TryGetValue(grid[i].state, out var color);
        Debug.Log(grid[i].state);

        renderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", color);
        renderer.SetPropertyBlock(block);
    }
}

