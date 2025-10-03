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
/// 
/// </summary>
public class MetroSurffaajat : PhysicsGame
{
    private GameObject[] _objectPool = new GameObject[2];
    private Model _cube;
    
    
    public override void Begin()
    {
        for (uint i = 0; i < _objectPool.Length; i++)
        {
            _objectPool[i] = new GameObject(0, 0);
            Add(_objectPool[i]);
        }
        
        _cube = new Model(ModelType.DualCube);
        _cube.Position.Z = -2.0f;
        
        Matrix4X4<float> perspective = Matrix4X4.CreatePerspective(0.1f, 0.1f, 0.1f, 1000f);
        
        Timer.CreateAndStart(0.016, delegate
        {
            ArraySegment<GameObject> view = new ArraySegment<GameObject>(_objectPool, 0, _cube.GetSubModelCount());
            _cube.Render(view, perspective);
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