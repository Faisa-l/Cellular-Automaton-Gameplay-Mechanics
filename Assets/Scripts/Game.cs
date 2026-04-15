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
}
