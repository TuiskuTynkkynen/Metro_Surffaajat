using System;
using Jypeli;
using Silk.NET.Maths;

namespace Metro_Surffaajat.renderer;

/// @author Tuisku Tynkkynen
/// @version 04.10.2025
/// <summary>
/// Class for storing view related data and the view perspective matrix.
/// </summary>
public class Camera3D
{
    /// <summary>
    /// The up vector of the world. Positive Y is currently the up direction.
    /// </summary>
    private readonly Vector3D<float> _worldUp = new Vector3D<float>(0.0f, 1.0f, 0.0f);
    
    /// <summary>
    /// Up vector of the Camera.
    /// </summary>
    private Vector3D<float> _up;
    /// <summary>
    /// Direction vector of the Camera. Points to where the Camera is looking at.
    /// </summary>
    private Vector3D<float> _direction = new Vector3D<float>(0.0f, 0.0f, -1.0f);
    /// <summary>
    /// Position vector of the Camera.
    /// </summary>
    private Vector3D<float> _position = Vector3D<float>.Zero;

    /// <summary>
    /// Camera field of view in degrees.
    /// </summary>
    private float _fov;
    /// <summary>
    /// Camera aspect ratio
    /// </summary>
    private float _aspectRatio;
    
    /// <summary>
    /// View perspective matrix of the Camera.
    /// Automatically updates when the Camera is updated.
    /// </summary>
    public Matrix4X4<float> ViewPerspectiveMatrix { get; private set; }

    
    /// <summary>
    /// Constructor for Camera with view direction vector
    /// </summary>
    /// <param name="position">Camera position</param>
    /// <param name="direction">Camera view direction</param>
    /// <param name="fov">Camera field of view in degrees</param>
    /// <param name="aspectRatio">Camera aspect ratio</param>
    public Camera3D(Vector3D<float>? position = null, Vector3D<float>? direction = null,  float fov = 90.0f, float aspectRatio = 1.0f)
    {   
        _fov = fov;
        _aspectRatio = aspectRatio;
        
        Update(position, direction);
    }

    
    /// <summary>
    /// Constructor for Camera with view angles.
    /// </summary>
    /// <param name="position">Camera position</param>
    /// <param name="pitch">Camera view pitch in degrees</param>
    /// <param name="yaw">Camera yaw pitch in degrees</param>
    /// <param name="fov">Camera field of view in degrees</param>
    /// <param name="aspectRatio">Camera aspect ratio</param>
    public Camera3D(Vector3D<float>? position = null, float pitch = 0.0f, float yaw = -90.0f,  float fov = 90.0f, float aspectRatio = 1.0f)
    {   
        _fov = fov;
        _aspectRatio = aspectRatio;
        
        Update(position, pitch, yaw);
    }
    
    
    /// <summary>
    /// Update Camera position and direction.
    /// </summary>
    /// <param name="position">New Camera position. No change if null</param>
    /// <param name="direction">New Camera view direction. No change if null</param>
    public void Update(Vector3D<float>? position = null, Vector3D<float>? direction = null)
    {
        _position = position ?? _position;
        _direction = direction ?? _direction;

        UpdateMatrix();
    }
    
    
    /// <summary>
    /// Update Camera position and direction.
    /// </summary>
    /// <param name="position">New Camera position. No change if null</param>
    /// <param name="pitch">New Camera view pitch in degrees.</param>
    /// <param name="yaw">New Camera view yaw in degrees.</param>
    public void Update(Vector3D<float>? position = null, float pitch = 0.0f, float yaw = -90.0f)
    {
        _position = position ?? _position;

        _direction.X = (float)(Math.Cos(Angle.DegreeToRadian(yaw)) * Math.Cos(Angle.DegreeToRadian(pitch)));
        _direction.Y = (float)Math.Sin(Angle.DegreeToRadian(pitch));
        _direction.Z = (float)(Math.Sin(Angle.DegreeToRadian(yaw)) * Math.Cos(Angle.DegreeToRadian(pitch)));
        
        UpdateMatrix();
    }

    
    /// <summary>
    /// Update Camera field of view and aspect ratio.
    /// </summary>
    /// <param name="fov">New Camera field of view in degrees.</param>
    /// <param name="aspectRatio">New Camera aspect ratio.</param>
    public void Update(float fov = 90.0f, float aspectRatio = 1.0f)
    {
        _fov = fov;
        _aspectRatio = aspectRatio;
        UpdateMatrix();
    }
    
    
    /// <summary>
    /// Updates the Camera view perspective matrix.
    /// </summary>
    private void UpdateMatrix()
    {
        Vector3D<float> cameraRight = Vector3D.Normalize(Vector3D.Cross(_direction, -_worldUp));
        _up = Vector3D.Normalize(Vector3D.Cross(_direction, cameraRight));

        ViewPerspectiveMatrix = Matrix4X4.CreateLookAt(_position, _position + _direction, _up);
        ViewPerspectiveMatrix *= Matrix4X4.CreatePerspectiveFieldOfView((float)Angle.DegreeToRadian(_fov), _aspectRatio, 0.01f, 10000.0f);
    }
}