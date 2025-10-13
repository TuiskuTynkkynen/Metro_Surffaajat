using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using Silk.NET.Maths;
using Metro_Surffaajat.renderer;

namespace Metro_Surffaajat;

/// @author Tuisku Tynkkynen
/// @version 01.10.2025
/// <summary>
/// The main game class.
/// Owns game state and controls updating state.
/// </summary>
public class MetroSurffaajat : PhysicsGame
{
    /// <summary>
    /// Pool of GameObjects used for rendering.
    /// GameObjects are preallocated to reduce excessive Add and Remove calls.
    /// </summary>
    private GameObject[] _objectPool = new GameObject[2];
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
        
        for (uint i = 0; i < _objectPool.Length; i++)
        {
            _objectPool[i] = new GameObject(0, 0);
            Add(_objectPool[i]);
        }
        
        _cube = new Model(ModelType.DualCube);
        _cube.Position.Z = -2.0f;
        
        Camera3D camera = new Camera3D(new Vector3D<float>(0.0f, 1.0f, 0.0f), pitch: -45.0f);
        
        Timer.CreateAndStart(0.016, delegate
        {
            ArraySegment<GameObject> view = new ArraySegment<GameObject>(_objectPool, 0, _cube.GetSubModelCount());
            _cube.Render(view, ref camera);
        });
        
        Keyboard.Listen(Key.W, ButtonState.Down, delegate { _cube.Position.Z += 0.1f; }, null);
        Keyboard.Listen(Key.A, ButtonState.Down, delegate { _cube.Position.X -= 0.1f; }, null);
        Keyboard.Listen(Key.S, ButtonState.Down, delegate { _cube.Position.Z -= 0.1f;  }, null);
        Keyboard.Listen(Key.D, ButtonState.Down, delegate { _cube.Position.X += 0.1f; }, null);
        Keyboard.Listen(Key.Space, ButtonState.Down, delegate { _cube.Position.Y += 0.1f; }, null);
        Keyboard.Listen(Key.LeftShift, ButtonState.Down, delegate { _cube.Position.Y -= 0.1f; }, null);
        
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
}