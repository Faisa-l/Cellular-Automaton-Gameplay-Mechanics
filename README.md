To access the project, clone the project and open in Unity 6000.1.9f1. Different states of the project can be accessed by checking out at specific commits.

### How to use

The main automaton is the "Cellular Automaton" game object. Running the scene sets every cell to the "starting cell", and you can left click any cell to replace it with the "painted cell". You can adjust the visuals of cells based on their `CellMaterial` in the Grid Visualiser component.

Arrow keys move one of the "Actor" game objects, and the Game Entity component. It is recommended to not set both active and to try move them around.

You can select the automaton functions in the main automaton. "Update World Simuation" and "Process Health Decay" are the main tested functions. Game of Life does work but requires manually setting the visualisation to display the output.

### Controls
- WASD - move camera.
- Arrow keys - move Actor / Game Entity
- Space - tick main automaton forward one step.
- Q - automatically tick main automaton forward (adjust speed in "updates per second").
