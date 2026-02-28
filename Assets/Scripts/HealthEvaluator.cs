using UnityEngine;

/// <summary>
/// Evaluates a given CellularAutomaton to equate to health.
/// </summary>
public class HealthEvaluation : MonoBehaviour, IEvaluator
{
    public enum EvaluationType { IsAlive, RawCount, ValueCount }
    
    [SerializeField]
    CellularAutomaton cellularAutomaton;

    [SerializeField, Tooltip("Method of evaluating the cell's value.")]
    EvaluationType mode = EvaluationType.RawCount;

    /// <summary>
    /// Result of the evaluation, recalculated after calling Evaluate().
    /// </summary>
    public float Evaluation { get; set; }

    public void Initialise()
    {
        Evaluation = 0;
        if (!TryGetComponent(out cellularAutomaton))
        {
            cellularAutomaton= gameObject.AddComponent<CellularAutomaton>();
        }
    }

    // Evaluates the cellular automaton to a single value. Result is stored in Evaluation.
    public void Evaluate()
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
                        count += cell.health;
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
        Debug.Log($"Health: {Evaluation}");
    }
}

/// <summary>
/// Handles evaluating a cellular automaton.
/// </summary>
public interface IEvaluator
{
    public float Evaluation { get; set; }
    public void Evaluate();
}
