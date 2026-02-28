using Unity.Collections;
using UnityEngine;

// MIGHT BE REMOVED
/// <summary>
/// Object to set up the layout for how a grid controls its cell's data layout.
/// </summary>
[CreateAssetMenu(fileName = "CellDataLayout", menuName = "Scriptable Objects/CellDataLayout")]
public class CellDataLayout : ScriptableObject
{
    [SerializeField, Tooltip("The layout for this array is copied to a grid's \"data map\". Each index will map to the value for the cell's data array.")]
    string[] cellDataLayout;

    public string[] Layout => cellDataLayout;

    // Strings must cleanly cast themselves as a FixedString32Bytes, so try cast them as this and warn if this breaks.
    private void OnValidate()
    {
        string test = "";
        try
        {
            foreach (string st in cellDataLayout)
            {
                test = st;
                new FixedString32Bytes(test);
            }
        }
        catch
        {
            if (test == string.Empty) return;
            Debug.LogWarning($"CellDataLayout: Field \"{test}\" cannot cast into FixedString32Bytes.");
        }
    }
}
