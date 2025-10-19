using System;
using System.Collections.Generic;
using Jypeli;

namespace Metro_Surffaajat.renderer;

/// @author Tuisku Tynkkynen
/// @version 19.10.2025
/// <summary>
/// Collection of RenderBuffers used as render layers.
/// Automates RenderBuffer management.
/// </summary>
public class Renderer<TLayerEnum>
    where TLayerEnum : Enum
{
    /// <summary>
    /// RenderBuffers to be used as layers.
    /// </summary>
    private readonly Dictionary<TLayerEnum, RenderBuffer> _buffers = new ();
    /// <summary>
    /// Camera used for rendering current frame.
    /// </summary>
    private Camera3D _camera;

    
    /// <summary>
    /// Creates a Renderer and instantiates RenderBuffers with GameObject
    /// arrays created with the provided generator.
    /// </summary>
    /// <param name="bufferGenerator">Generator for GameObject arrays.
    /// GameObjects should be Added to the game in the order they are in the
    /// created array </param>
    public Renderer(Func<GameObject[]> bufferGenerator)
    {
        var types = Enum.GetValues(typeof(TLayerEnum));

        foreach (TLayerEnum type in types)
        {
            _buffers.Add(type, new RenderBuffer(bufferGenerator()));
        }
    }
    
    
    /// <summary>
    /// Creates a Renderer and instantiates RenderBuffers with the provided
    /// GameObject arrays.
    /// </summary>
    /// <param name="buffers">Arrays of GameObjects. GameObjects should be
    /// Added to the game in the order they are in the arrays</param>
    /// <exception cref="ArgumentException">buffers must contain at least the
    /// same number of arrays as there ara layers specified by TLayerEnum</exception>
    public Renderer(GameObject[][] buffers)
    {
        var types = (TLayerEnum[])Enum.GetValues(typeof(TLayerEnum));

        if (buffers.Rank < types.Length)
        {
            throw new ArgumentException("Rank of provided GameObject array must be grater or equal to number of elements of TLayerEnum");
        }

        for (int i = 0; i < types.Rank; i++)
        {
            _buffers.Add(types[i], new RenderBuffer(buffers[i]));
        }
    }
    
    
    /// <summary>
    /// Begins a new frame. The provided Camera3D will be used for rendering
    /// during the frame. 
    /// </summary>
    /// <param name="camera">Camera used for rendering</param>
    /// <exception cref="InvalidOperationException">If BeginRender has already
    /// been called, frame must be ended with EndRender before beginning the
    /// next frame</exception>
    public void BeginRender(Camera3D camera)
    {
        if (_camera != null)
        {
            throw new InvalidOperationException("Current frame has to be ended with EndRender() before beginning new frame");
        }
        
        _camera = camera;

        foreach (var (_, buffer) in _buffers)
        {
            buffer.BeginRender();
        }
    }

    
    /// <summary>
    /// Renders a model to the specified layer.
    /// </summary>
    /// <param name="model">Model to be rendered</param>
    /// <param name="layer">Layer the model is rendered to</param>
    /// <exception cref="InvalidOperationException">Frame must be started with BeginRender before rendering Models</exception>
    public void Submit(ref readonly Model model, TLayerEnum layer)
    {
        if (_camera == null)
        {
            throw new InvalidOperationException("Frame has to be began with BeginRender() before models can be rendered");
        }
     
        model.Render(_buffers[layer], ref _camera);
    }
    
    
    /// <summary>
    /// Ends rendering for the current frame.
    /// </summary>
    /// <exception cref="InvalidOperationException">Frame must be started with BeginRender before it is ended</exception>
    public void EndRender()
    {
        if (_camera == null)
        {
            throw new InvalidOperationException("Frame has to be began with BeginRender() before it can be ended");
        }

        _camera = null;
        foreach (var (_, buffer) in _buffers)
        {
            buffer.EndRender();
        }
    }
}

