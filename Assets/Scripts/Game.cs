using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// Handles the logic for the entire game
/// </summary>
public class Game : MonoBehaviour
{
    [SerializeField]
    CellularAutomaton automaton;

    [SerializeField]
    HealthEvaluation healthEvaluation;

    [SerializeField]
    PlayerInputDirection inputDirection;

    [SerializeField]
    TextMeshProUGUI inspectorLabel;

    [SerializeField]
    bool cellInspectorEnabled = true;

    // Bind to an event to proceed the tick of the automaton
    public void ProceedTick(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            automaton.NextTick();
        }
    }

    public void TouchedCell(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            automaton.OnTouchedCell();
        }
    }

    public void ToggleAutomatonRunning(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            automaton.ToggleRepeatingUpdate();
        }
    }

    public void PerformEvaluation(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            healthEvaluation.Evaluate();
        }
    }

    public void OnMoveInput(CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        inputDirection.input = input;
    }

    public void ToggleGridInspector(CallbackContext context)
    {
        if (!cellInspectorEnabled) return; 

        if (context.phase == InputActionPhase.Started)
        {
            InspectCell();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            inspectorLabel.text = "";
        }
    }

    // Uses reflection to output the names of fields and their value
    void InspectCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!automaton.Visuasliser.TryGetTouchedCellIndex(ray, out int index)) return;

        Cell cell = automaton.Grid[index];
        automaton.Grid.GetRowColumn(index, out int row, out int column);
        string output = "";
        var cellFieldInfo = typeof(Cell).GetFields();

        // Build string to display 
        output += $"index (row, column) : {index} ({row}, {column})\n";
        for (int i = 0; i < cellFieldInfo.Length; i++)
        {
            output += $"{cellFieldInfo[i].Name} : {cellFieldInfo[i].GetValue(cell)}\n";
        }
        inspectorLabel.text = output;
    }
}
