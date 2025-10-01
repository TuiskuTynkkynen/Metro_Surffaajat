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
    private Model _whiteCube;
    public override void Begin()
    {
        GameObject gameObject = new GameObject(100, 100);
        
        Add(gameObject);

        _whiteCube = new Model(ModelType.BlueCube);
        _whiteCube.Position.Z = -2.0f;
        
        Matrix4X4<float> perspective = Matrix4X4.CreatePerspective(0.1f, 0.1f, 0.1f, 1000f);
        
        Timer.CreateAndStart(0.016, delegate { _whiteCube.Render(ref gameObject, perspective); });
        
        Keyboard.Listen(Key.W, ButtonState.Down, delegate { _whiteCube.Position.Z += 0.1f; }, null);
        Keyboard.Listen(Key.A, ButtonState.Down, delegate { _whiteCube.Position.X -= 0.1f; }, null);
        Keyboard.Listen(Key.S, ButtonState.Down, delegate { _whiteCube.Position.Z -= 0.1f;  }, null);
        Keyboard.Listen(Key.D, ButtonState.Down, delegate { _whiteCube.Position.X += 0.1f; }, null);
        Keyboard.Listen(Key.Space, ButtonState.Down, delegate { _whiteCube.Position.Y += 0.1f; }, null);
        Keyboard.Listen(Key.LeftShift, ButtonState.Down, delegate { _whiteCube.Position.Y -= 0.1f; }, null);
        
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
}