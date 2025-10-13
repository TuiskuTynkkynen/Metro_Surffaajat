using System;
using Jypeli;

namespace Metro_Surffaajat.renderer;

/// @author Tuisku Tynkkynen
/// @version 13.10.2025
/// <summary>
/// Circular buffer of GameObjects used for 3d rendering.
/// </summary>
public class RenderBuffer(GameObject[] gameObjects)
{
    /// <summary>
    /// Preallocated buffer of GameObjects.
    /// </summary>
    private GameObject[] _objectPool = gameObjects;
    /// <summary>
    /// Index of the next GameObject to be used for rendering.
    /// </summary>
    private int _head;
    
    
    /// <summary>
    /// Begins rendering for current frame.
    /// </summary>
    public void BeginRender()
    {
        _head = 0;
    }
    
    
    /// <summary>
    /// Gets the next GameObject for rendering. If all GameObjects have already
    /// been rendered to, returns the GameObject that was rendered to first
    /// i.e. the bottommost object in draw order
    /// </summary>
    /// <returns>Next GameObject for rendering</returns>
    public ref GameObject GetNext()
    {
        int nextIndex = _head % _objectPool.Length;
        _head++;
        
        return ref _objectPool[nextIndex];
    }
    
    
    /// <summary>
    /// Ends rendering for current frame and prevents unused
    /// GameObjects from being rendered to screen.
    /// </summary>
    public void EndRender()
    {
        for (int i = _head; i < _objectPool.Length; i++)
        {
            _objectPool[i].Size = Vector.Zero;
        }
    }

    
    /// <summary>
    /// Gets a view into internal GameObject array.
    /// </summary>
    /// <returns>View into internal GameObject array.</returns>
    public ArraySegment<GameObject> GetGameObjects()
    {
        return _objectPool;
    }
}