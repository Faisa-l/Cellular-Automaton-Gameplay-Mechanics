using System;
using UnityEngine;

/// <summary>
/// Maps a <see cref="CellMaterial"/> to a <see cref="Material"/>.
/// </summary>
[Serializable]
public struct CellMaterialValues
{
    public CellMaterial type;
    public Texture texture;
    public Color color;

    [ColorUsage(true, true)]
    public Color emissionColor;

    [Min(0f)]
    public float emissionIntensity;

    [Range(0f, 1f)]
    public float metallicMap, smoothness;
}

