using Jypeli;
using Silk.NET.Maths;
using Metro_Surffaajat.renderer;

namespace Metro_Surffaajat;

/// @author Tuisku Tynkkynen
/// @version 14.10.2025
/// <summary>
/// Enum of the layers used by Renderer.
/// </summary>
internal enum RenderLayers
{
    Default
}


/// @author Tuisku Tynkkynen
/// @version 14.10.2025
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
    /// Cube 3D model used for testing renderer architecture.
    /// </summary>
    private Model _cube;
    
    
    /// <summary>
    /// Jypeli PhysicsGame initialization that is automatically called by the library.
    /// Initializes state, input and game update timer.
    /// </summary>
    public override void Begin()
    {
        BoundingRectangle normalizedDeviceCoordinates = new BoundingRectangle(0, 0, 2, 2);
        Camera.ZoomTo(normalizedDeviceCoordinates);
        
        _renderer = new Renderer<RenderLayers>(() => CreateGameObjectArray(2));
        
        _cube = new Model(ModelType.DualCube);
        _cube.Position.Z = -2.0f;
        
        Keyboard.Listen(Key.W, ButtonState.Down, delegate { _cube.Position.Z += 0.1f; }, null);
        Keyboard.Listen(Key.A, ButtonState.Down, delegate { _cube.Position.X -= 0.1f; }, null);
        Keyboard.Listen(Key.S, ButtonState.Down, delegate { _cube.Position.Z -= 0.1f;  }, null);
        Keyboard.Listen(Key.D, ButtonState.Down, delegate { _cube.Position.X += 0.1f; }, null);
        Keyboard.Listen(Key.Space, ButtonState.Down, delegate { _cube.Position.Y += 0.1f; }, null);
        Keyboard.Listen(Key.LeftShift, ButtonState.Down, delegate { _cube.Position.Y -= 0.1f; }, null);
        
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    
    
    /// <summary>
    /// Jypeli PhysicsGame update method that is automatically called by the library.
    /// Used to update and render the object in the scene.
    /// </summary>
    /// <param name="deltaTime">Time since last frame</param>
    protected override void Update(Time deltaTime)
    {   
        Camera3D camera = new Camera3D(new Vector3D<float>(0.0f, 1.0f, 0.0f), pitch: -45.0f);
        
        _renderer.BeginRender(camera);
        _renderer.Submit(ref _cube, RenderLayers.Default);
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