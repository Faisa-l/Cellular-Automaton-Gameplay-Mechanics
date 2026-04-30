using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles <see cref="GridActor"/> instances moving around an automaton's grid.
/// </summary>
public class TraversalEvaluator : MonoBehaviour
{
    [SerializeField]
    List<GridActor> actors;

    [SerializeField]
    GridActor playerActor;

    [SerializeField]
    CellularAutomaton cellularAutomaton;

    [SerializeField]
    Vector3 actorPositionOffset = new(0, 1.2f, 0);

    // Indexes should map to the 'actors' list
    readonly List<Vector2Int> trackedActorPositions = new();

    private void Awake()
    {
        actors ??= new List<GridActor>();
        foreach (var actor in actors)
        {
            // This should take into account the relative angle between transform and the cellularAutomaton
            actor.offset = actorPositionOffset + (cellularAutomaton.transform.position -  transform.position);
            trackedActorPositions.Add(actor.GridPosition);
        }
    }

    private void OnEnable() => cellularAutomaton.OnUpdate += EvaluateActors;
    private void OnDisable() => cellularAutomaton.OnUpdate -= EvaluateActors;

    // Performs the movement of every tracked actor.
    // Actors request where to move with actor.moveTo; this function actually process that request
    // Has to manage collisions between other actors and any other limitations
    public void EvaluateActors()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            var actor = actors[i];
            actor.ActorUpdate(cellularAutomaton.Grid);
            Vector2Int destination = actor.GridPosition + actor.moveTo;

            // If this destination is occupied then undo the + actor.moveTo
            for (int j = 0; j < trackedActorPositions.Count; j++) 
            {
                if (i == j) continue;
                var actorPosition = trackedActorPositions[j];
                if (actorPosition == destination)
                {
                    destination -= actor.moveTo;
                    break;
                }
            }

            // Move actor and update its tracked position
            actor.GridPosition = trackedActorPositions[i] = destination;
        }
    }
}
