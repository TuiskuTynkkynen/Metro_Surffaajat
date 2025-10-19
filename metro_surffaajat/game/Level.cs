using System.Diagnostics;
using System.Linq;
using Jypeli;
using Metro_Surffaajat.renderer;
using Metro_Surffaajat.utility;

namespace Metro_Surffaajat.game;

/// @author Tuisku Tynkkynen
/// @version 19.10.2025
/// <summary>
/// Game level class. Owns and manages obstacles. 
/// </summary>
public class Level
{
    /// <summary>
    /// Size of the level i.e. how many rows of obstacles.
    /// </summary>
    private readonly int _size;
    /// <summary>
    /// Buffer of obstacles.
    /// </summary>
    private readonly CircularBuffer<TrackSegment> _tracks;
    /// <summary>
    /// The offset of obstacle positions used to achieve smooth scrolling.
    /// In range from 0.0 to 1.0.
    /// </summary>
    private float _positionOffset;
    
    
    /// <summary>
    /// Creates a Level of specified size.
    /// </summary>
    /// <param name="size">Size of the level</param>
    public Level(int size = 10)
    {
        _size = size;
        _tracks = new CircularBuffer<TrackSegment>(size);
        
        _tracks.Add(new TrackSegment());

        while (_tracks.Count < size)
        {
            _tracks.Add(_tracks.Back().GetNext());
        }
    }


    /// <summary>
    /// Updates the Level.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        _positionOffset += deltaTime;

        if (_positionOffset >= 1.0f)
        {
            _positionOffset -= 1.0f;
            
            var next = _tracks.Back().GetNext();
            _tracks.Add(next);
        }
    }

    
    /// <summary>
    /// Renders all obstacles in the Level.
    /// </summary>
    /// <param name="renderer">Renderer used for rendering</param>
    public void Render(ref Renderer<RenderLayers> renderer)
    {
        float z = -_size + _positionOffset;
        
        // Enumerate in reverse for back to front draw order 
        foreach (var trackSegment in _tracks.Reverse())
        {
            z += 1.0f;

            var models = trackSegment.GetModels();
            while (models.MoveNext())
            {
                if (models.Current == null)
                {
                    Debug.Fail("All models should be valid");
                }

                Model model = models.Current;
                models.Current.Position.Z = z;
                models.Current.Tint = Utility.Lerp(Color.White, Color.SkyBlue, (double)z / (-_size + 1));

                renderer.Submit(ref model, RenderLayers.Default);
            }
        }
    }
}