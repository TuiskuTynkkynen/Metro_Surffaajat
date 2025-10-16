using System.Linq;
using Jypeli;
using Metro_Surffaajat.renderer;
using Metro_Surffaajat.utility;
using Silk.NET.Maths;

namespace Metro_Surffaajat.game;


/// @author Tuisku Tynkkynen
/// @version 16.10.2025
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
    private readonly CircularBuffer<ObstacleType[]> _obstacles;
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
        _obstacles = new CircularBuffer<ObstacleType[]>(size);
        
        _obstacles.Add([ObstacleType.None, ObstacleType.None, ObstacleType.None]);

        while (_obstacles.Count < size)
        {
            _obstacles.Add(Next(_obstacles.Back()));
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
            
            var next = Next(_obstacles.Back());
            _obstacles.Add(next);
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
        foreach (ObstacleType[] track in _obstacles.Reverse())
        {
            z += 1.0f;
            
            for (int i = 0; i < track.Length; i++)
            {
                // Iterate over the array from the outermost elements to the
                // innermost i.e. first the left obstacle, then the right one
                // and the middle one last. This is done to ensure the correct
                // draw order
                int index = i % 2 == 0? i / 2: track.Length - i / 2 - 1;
                int x = index - track.Length / 2;
                
                Model model = new Model(Obstacles.GetModelType(track[index]))
                {
                    Position = new Vector3D<float>(x, 0, z),
                    Tint = Utility.Lerp(Color.White, Color.SkyBlue, (double)z / (-_size + 1))
                };

                renderer.Submit(ref model, RenderLayers.Default);
            }
        }
    }
    

    /// <summary>
    /// Calculates a new row of obstacles based on specified current row.
    /// </summary>
    /// <param name="current">The current row of obstacles</param>
    /// <returns>The next row of obstacles</returns>
    private static ObstacleType[] Next(ObstacleType[] current)
    {
        ObstacleType[] result = new ObstacleType[current.Length];

        for (int i = 0; i < current.Length; i++)
        {
            result[i] = Obstacles.GetNextObstacle(current[i]);
        }

        return result;
    }
}