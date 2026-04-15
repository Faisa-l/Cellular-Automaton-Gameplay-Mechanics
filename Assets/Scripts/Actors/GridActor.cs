using UnityEngine;

/// <summary>
/// An entity that exists on a grid.
/// </summary>
public class GridActor : MonoBehaviour
{
    [SerializeField]
    PlayerInputDirection playerInput;

    [SerializeField]
    bool useRandomMoveTo = false;
    
    [SerializeField]
    Vector2Int startingPosition;

    public Vector2Int moveTo;
    public Vector3 offset;

    Vector2Int pos;
    public Vector2Int GridPosition
    {
        get => pos;
        set
        {
            pos = value;
            transform.position = new Vector3(pos.y, 0, pos.x) + offset;
        }
    }

    private void OnValidate()
    {
        Mathf.Clamp(moveTo.x, -1, 1);
        Mathf.Clamp(moveTo.y, -1, 1);
    }

    private void Start() => GridPosition = startingPosition;

    public void ActorUpdate(in Grid grid)
    {
        if (useRandomMoveTo)
        {
            moveTo.x = Random.Range(-1, 1);
            moveTo.y = Random.Range(-1, 1);
        }
        else if (playerInput != null)
        {
            moveTo.x = (int)playerInput.input.x;
            moveTo.y = -(int)playerInput.input.y;
        }

        // Check if the destination is valid
        // If it is outside the grid, don't move (for now)
        Vector2Int predictedPosition = GridPosition + moveTo;
        if (grid.TryGetCellIndex(predictedPosition, out int destinationIndex))
        {
            if (grid[destinationIndex].material == CellMaterial.Water && grid[destinationIndex].isEmpty == 1)
            {
                moveTo = new(0, 0);
            }
        }   
        else moveTo = new(0, 0);
    }
}