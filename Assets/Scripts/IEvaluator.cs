
// NOTE: The general idea for this was so you can serialize a field with this interface but this specific project cannot do that (and it's a bit too complicated to set up).
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