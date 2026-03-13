using UnityEngine;

/// <summary>
/// Broad class referring to anything damageable, (abstractly) represented as an object composed of multiple cells.
/// This uses a <see cref="HealthEvaluation"/> to describe its health. 
/// </summary>
public class Entity : MonoBehaviour
{
    [SerializeField, Tooltip("This should be customised with how the entity will handle damage.")]
    HealthEvaluation healthEvaluation;

    [SerializeField]
    CellularAutomaton entityAutomaton, baseAutomaton;

    [SerializeField]
    ValueObject<float> healthObject;

    float Health => healthEvaluation.Health;

    private void OnEnable()
    {
        entityAutomaton.OnUpdate += AutomatonProcess;
    }

    private void OnDisable()
    {
        entityAutomaton.OnUpdate -= AutomatonProcess;
    }

    private void AutomatonProcess()
    {
        ProcessWithBaseAutomaton();
        healthEvaluation.Evaluate();
        if (healthObject) healthObject.Value = Health;
    }

    void ProcessWithBaseAutomaton()
    {
        // Could be a job
        // Iterate through cells in the entity automaton and compare them with the cells in the base automaton to handle interactions
        for (int i = 0; i < entityAutomaton.Grid.Size; i++)
        {
            if (!entityAutomaton.TryProjectCellOntoOtherAutomaton(baseAutomaton, i, out int baseIndex)) continue;
        
            // Store changes to make to the cell and apply them later
            Cell changes = entityAutomaton.Grid[i];

            // Deal damage to this cell
            float damage = baseAutomaton.Grid[baseIndex].damage;
            changes.health = Mathf.Max(changes.health - damage, 0f);

            // Apply decaying damage stacks (actual decaying damage calculation is handled later).
            float appliedDecayStack = baseAutomaton.Grid[baseIndex].appliedDecayStack;
            changes.healthDecayStack = appliedDecayStack;

            // Apply changes
            entityAutomaton.OverrideCell(i, changes);
        }
    }

}