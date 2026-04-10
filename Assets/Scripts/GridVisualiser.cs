using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles drawing the grid itself.
/// Currently, it will draw its grid using the prefab.
/// </summary>
public class GridVisualiser : MonoBehaviour
{
    [SerializeField]
    StateColor[] colorMap;

    [SerializeField]
    CellMaterialValues[] materialConfigurations;

    [SerializeField]
    Material fallbackMaterial;

    [Header("Cell attribute adjustments")]
    [SerializeField, Min(0.01f)]
    float maxTemperatureSmoothness = 50f;

    [SerializeField, Min(0.01f)]
    float maxHealthTransparency = 10f;

    [SerializeField, Range(0f, 1f), Tooltip("How much heat contributes to emission")]
    float heatEmission = 0.3f;

    [SerializeField, Min(0f)]
    float maxEmissionIntensity = 5f;

    [SerializeField, Min(0f)]
    float metallicLiquid = 0.5f;

    GameObject prefab;
    Grid grid;

    GameObject[] drawnObjects;
    Renderer[] renderers;
    Dictionary<CellState, Color> colorPairs;
    Dictionary<CellMaterial, CellMaterialValues> materialMap;
    static MaterialPropertyBlock block;

    /// <summary>
    /// Load the visualiser with data.
    /// </summary>
    /// <param name="prefab">Prefab game object to represent each cell.</param>
    /// <param name="grid">Grid instance to visualise.</param>
    public void Initialise(GameObject prefab, Grid grid)
    {
        this.prefab = prefab;
        this.grid = grid;
        block = new MaterialPropertyBlock();
        colorPairs = new Dictionary<CellState, Color>();
        materialMap = new Dictionary<CellMaterial, CellMaterialValues>();

        foreach (var pair in colorMap) colorPairs.Add(pair.state, pair.color);
        foreach (var values in materialConfigurations) materialMap.Add(values.type, values);
    }

    // Draws the grid 
    public void Draw()
    {
        drawnObjects ??= new GameObject[grid.Size];
        renderers ??= new Renderer[grid.Size];

        for (int i = 0; i < grid.Size; i++)
        {
            grid.GetRowColumn(i, out int row, out int column);
            Vector3 position = new(row, 0, column);
            GameObject obj = Instantiate(prefab, transform);

            obj.transform.localPosition = position;
            drawnObjects[i] = obj;
            renderers[i] = obj.GetComponent<Renderer>();
            ApplyCellColour(i);
        }
    }

    // Changes the colour of the cells inside the specified list.
    // Should make this take in an array of cells to update
    public void UpdateVisualisation(int[] updated)
    {
        for (int i = 0; i < updated.Length; i++)
        {
            ApplyCellColour(updated[i]);
        }
    }

    public void UpdateVisualisation(int cell) => ApplyCellColour(cell);


    // Individually change the colour of cell i
    // MIGHT be necessary to make a custom material shader since this gets very slow if it needs to update a lot of materials
    void ApplyCellColour(int i)
    {
        Cell target = grid[i];          // Should be a reference to the cell but you can't do that without an (unsafe) extension function
        var renderer = renderers[i];
        materialMap.TryGetValue(target.material, out var materialValues);
        var color = materialValues.color;

        // Adjust based on cell properties
        color.a = Mathf.Clamp(target.health / maxHealthTransparency, 0f, 1f);
        float temperatureSmoothness = Mathf.Clamp(materialValues.smoothness + (target.temperature / maxTemperatureSmoothness), 0f, 1f);
        float emissiveHeat = Mathf.Clamp((target.heat * heatEmission) + materialValues.emissionIntensity, 0f, maxEmissionIntensity);
        float metallicMapLiquid = Mathf.Clamp((target.liquidLevel * metallicLiquid) + materialValues.metallicMap, 0f, 1f);

        // Apply material changes (this part is unoptimised)
        renderer.GetPropertyBlock(block);
        if (materialValues.texture != null) block.SetTexture("_BaseMap", materialValues.texture);
        block.SetColor("_BaseColor", color);
        block.SetColor("_EmissionColor", materialValues.emissionColor * emissiveHeat);
        block.SetFloat("_Metallic", metallicMapLiquid);
        block.SetFloat("_Smoothness", temperatureSmoothness);
        renderer.SetPropertyBlock(block); 
        
    }

    /// <summary>
    /// Returns a the cell which intersects a ray.
    /// </summary>
    /// <param name="ray">Ray to query a cell's position.</param>
    /// <param name="index">Output index of the cell.</param>
    public bool TryGetTouchedCellIndex(Ray ray, out int index)
    {
        // Where ray intersects the xz plane
        Vector3 intersection = ray.origin - ray.direction * (ray.origin.y /  ray.direction.y);

        int row = Mathf.RoundToInt(intersection.x);
        int column = Mathf.RoundToInt(intersection.z);

        return grid.TryGetCellIndex(row, column, out index);
    }

    /// <summary>
    /// Returns the world space position of the cell at index.
    /// </summary>
    /// <param name="index">Index of the cell.</param>
    /// <param name="position">Output position of the cell's world space position.</param>
    /// <returns></returns>
    public bool TryGetCellPosition(int index, out Vector3 position)
    {
        position = Vector3.zero;
        if (index < 0 || index > drawnObjects.Length) return false;
        position = drawnObjects[index].transform.position;
        return true;
    }
}