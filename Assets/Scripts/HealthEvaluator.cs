using System;
using UnityEngine;

/// <summary>
/// Evaluates a given CellularAutomaton to equate to health.
/// </summary>
public class HealthEvaluation : MonoBehaviour
{
    public enum EvaluationType { IsAlive, RawCount, ValueCount }
    
    [SerializeField]
    CellularAutomaton cellularAutomaton;

    [SerializeField, Tooltip("Name of value in the cell to evaluate.")]
    string EvaluationValue = string.Empty;

    [SerializeField, Tooltip("Method of evaluating the cell's value.")]
    EvaluationType mode = EvaluationType.RawCount;

    /// <summary>
    /// Result of the evaluation, recalculated after calling Evaluate().
    /// </summary>
    public float Evaluation { get; private set; }

    public void Initialise()
    {
        Evaluation = 0;
        if (!TryGetComponent(out cellularAutomaton))
        {
            cellularAutomaton= gameObject.AddComponent<CellularAutomaton>();
        }
    }

    // Evaluates the cellular automaton to a single value. Result is stored in Evaluation.
    void Evaluate()
    {
        Grid grid = cellularAutomaton.Grid;

        float count = 0;
        for (int i = 0; i < grid.Size; i++)
        {
            Cell cell = grid[i];
            if (cell.state == CellState.Alive)
            {
                // Update the count based on how to evaluate
                switch (mode)
                {
                    case EvaluationType.RawCount:
                        count++;
                        break;
                    case EvaluationType.ValueCount:
                        count++;
                        break;
                    case EvaluationType.IsAlive:
                        count++;
                        break;
                    default:
                        count++;
                        break;
                }
            }
        }
        Evaluation = count;
    }
}
