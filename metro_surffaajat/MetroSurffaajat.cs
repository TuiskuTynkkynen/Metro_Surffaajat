using Jypeli;
using Silk.NET.Maths;
using Metro_Surffaajat.renderer;

namespace Metro_Surffaajat;

/// @author Tuisku Tynkkynen
/// @version 16.10.2025
/// <summary>
/// Enum of the layers used by Renderer.
/// </summary>
public enum RenderLayers
{
    Default
}


/// @author Tuisku Tynkkynen
/// @version 16.10.2025
/// <summary>
/// The main game class.
/// Owns game state and controls updating state.
/// </summary>
public class MetroSurffaajat : PhysicsGame
{
    /// <summary>
    /// The Renderer used for testing renderer architecture.
    /// </summary>
    private Renderer<RenderLayers> _renderer;
    /// <summary>
    /// Level used for testing game and renderer architecture.
    /// </summary>
    private game.Level _level;
    
    
    /// <summary>
    /// Jypeli PhysicsGame initialization that is automatically called by the library.
    /// Initializes state, input and game update timer.
    /// </summary>
    public override void Begin()
    {
        BoundingRectangle normalizedDeviceCoordinates = new BoundingRectangle(0, 0, 2, 2);
        Camera.ZoomTo(normalizedDeviceCoordinates);
        
        _renderer = new Renderer<RenderLayers>(() => CreateGameObjectArray(256));
        
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    
    
    /// <summary>
    /// Jypeli PhysicsGame update method that is automatically called by the library.
    /// Used to update and render the game level.
    /// </summary>
    /// <param name="deltaTime">Time since last frame</param>
    protected override void Update(Time deltaTime)
    {   
        Camera3D camera = new Camera3D(new Vector3D<float>(0.0f, 2.0f, 0.0f), pitch: -45.0f);
        
        _renderer.BeginRender(camera);
        
        _level.Update((float)deltaTime.SinceLastUpdate.TotalSeconds);
        _level.Render(ref _renderer);
        
        _renderer.EndRender();
    
        base.Update(deltaTime);
    }

    
    /// <summary>
    /// Creates an array of GameObjects, initializes them as default GameObjects
    /// of size (0, 0), and Adds them to the game.
    /// </summary>
    /// <param name="size">Size of the array</param>
    /// <returns>Array of initialized and Added GameObjects</returns>
    private GameObject[] CreateGameObjectArray(uint size)
    {
        GameObject[] objects = new GameObject[size];
        
        for (uint i = 0; i < size; i++)
        {
            objects[i] = new GameObject(0, 0);
            Add(objects[i]);
        }

        return objects;
    }
}