using UnityEngine;

/// <summary>
/// Evaluates a given CellularAutomaton to equate to health.
/// </summary>
public class HealthEvaluation : MonoBehaviour, IEvaluator
{
    public enum EvaluationType { IsAlive, RawCount, HealthCount }
    
    [SerializeField]
    CellularAutomaton cellularAutomaton;

    [SerializeField, Tooltip("Method of evaluating the cell's value.")]
    EvaluationType mode = EvaluationType.RawCount;

    [SerializeField, Min(0f)]
    float minimumHealthThreshold, maximumHealthThreshold;

    public float Evaluation { get; set; }

    /// <summary>
    /// The calculated health of the automaton.
    /// </summary>
    public float Health => Evaluation - minimumHealthThreshold;

    public void Initialise()
    {
        Evaluation = 0;
        if (!TryGetComponent(out cellularAutomaton))
        {
            cellularAutomaton = gameObject.AddComponent<CellularAutomaton>();
        }

    }

    public void Evaluate()
    {
        float count = 0;
        for (int i = 0; i < cellularAutomaton.Grid.Size; i++)
        {
            Cell cell = cellularAutomaton.Grid[i];
            if (cell.state == CellState.Alive)
            {
                // Update the count based on how to evaluate
                switch (mode)
                {
                    case EvaluationType.RawCount:
                        count++;
                        break;
                    case EvaluationType.HealthCount:
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
    }
}
