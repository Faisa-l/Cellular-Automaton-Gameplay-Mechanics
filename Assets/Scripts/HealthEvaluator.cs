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

    Grid Grid => cellularAutomaton.Grid;

    /// <summary>
    /// The calculated health of the automaton.
    /// </summary>
    public float Health => Evaluation - minimumHealthThreshold;

    public void Initialise()
    {
        Evaluation = 0;
        if (!TryGetComponent(out cellularAutomaton))
        {
            cellularAutomaton= gameObject.AddComponent<CellularAutomaton>();
        }

    }

    public void Evaluate()
    {
        float count = 0;
        for (int i = 0; i < Grid.Size; i++)
        {
            Cell cell = Grid[i];
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
        Debug.Log($"Health: {Evaluation}");
    }

    /// <summary>
    /// Deal damage to the cell at row, column.
    /// </summary>
    /// <param name="row">Row of cell.</param>
    /// <param name="column">Index of cell.</param>
    /// <param name="damage">Damage to deal to the cell's health.</param>
    public void DamageCell(int row, int column, float damage)
    {
        if (!Grid.TryGetCellIndex(row, column, out int index)) return;
        else DamageCell(index, damage);
    }

    /// <summary>
    /// Deal damage to the cell at index.
    /// </summary>
    /// <param name="index">Index of cell.</param>
    /// <param name="damage">Damage to deal to the cell's health.</param>
    /// 
    public void DamageCell(int index, float damage)
    {
        if (index < 0 || index >= Grid.Size) return;
        var newCell = Grid[index];
        newCell.health -= damage;
        cellularAutomaton.Grid[index] = newCell;
        cellularAutomaton.UpdateVisualisation();
    }
}

/// <summary>
/// Interface for evaluating a <see cref="CellularAutomaton"/>.
/// </summary>
public interface IEvaluator
{
    /// <summary>
    /// Result of the evaluation, recalculated after calling <see cref="Evaluate"/>.
    /// </summary>
    public float Evaluation { get; set; }

    /// <summary>
    /// Evaluates the cellular automaton to a single value. Result is stored in <see cref="Evaluation"/>. 
    /// </summary>
    public void Evaluate();
}
