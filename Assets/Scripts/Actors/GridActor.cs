using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [Min(0f)]
    public float heatTolerance = 20f, temperatureTolerance = 15f, damageTolerance = 10f;

    // All types of restrictions the cell can have
    public List<Predicate<Cell>> Restrictions;

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

    private void Awake()
    {
        // Some starting restrictions
        Restrictions = new()
        {
            (c) => c.material == CellMaterial.Water,
            (c) => c.isEmpty == 0,
            (c) => c.heat >= heatTolerance,
            (c) => c.temperature >= temperatureTolerance,
            (c) => c.damage >= damageTolerance
        };
    }

    private void Start() => GridPosition = startingPosition;

    private void OnDestroy() => Restrictions?.Clear();

    public void ActorUpdate(in Grid grid)
    {
        if (useRandomMoveTo)
        {
            moveTo.x = Random.Range(-1, 2);
            moveTo.y = Random.Range(-1, 2);
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
            // If any restriction returns true, then the cell cannot move into the destination
            if (Restrictions == null) return;
            foreach (var restriction in Restrictions)
            {
                if (restriction.Invoke(grid[destinationIndex]))
                {
                    moveTo = new(0, 0);
                    break;
                }
            }
        }   
        else moveTo = new(0, 0);
    }
}