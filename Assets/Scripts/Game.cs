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

    private void Awake()
    {
        automaton.Initialise();
    }

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
}
