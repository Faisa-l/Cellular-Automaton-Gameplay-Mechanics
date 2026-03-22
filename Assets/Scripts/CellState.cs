/// <summary>
/// Possible states of a cell.
/// Cells are primarily dependant on their CellState and of the states of their neighbours.
/// </summary>
public enum CellState { Dead, Alive }

/// <summary>
/// Possible material of a cell.
/// This determines how cell's can interact and also how they may appear in a visualisation.
/// </summary>
public enum CellMaterial { None, Dead, Alive, Air, Water, Rock, Lava, Grass }

