using System;
using System.Numerics;
using Jypeli;
using Silk.NET.Maths;

namespace Metro_Surffaajat.utility;

/// @author Tuisku Tynkkynen
/// @version 15.10.2025
/// <summary>
/// Represents a cuboid.
/// </summary>
/// <typeparam name="T">Type used for underlying data. Must be an unmanaged floating point type</typeparam>
public class BoundingCuboid<T>
    where T : unmanaged, IFloatingPoint<T>
{
    /// <summary>
    /// Frontal area of the cuboid.
    /// </summary>
    private BoundingRectangle _area = new BoundingRectangle(0, 0, 0, 0);
    /// <summary>
    /// Center of the cuboid in X dimension.
    /// </summary>
    public T X 
    {
        get => T.CreateSaturating(_area.X);
        set => _area.X = Convert.ToDouble(value);
    }
    /// <summary>
    /// Center of the cuboid in Y dimension.
    /// </summary>
    public T Y 
    {
        get => T.CreateSaturating(_area.Y);
        set => _area.Y = Convert.ToDouble(value);
    }

    /// <summary>
    /// Center of the cuboid in Z dimension.
    /// </summary>
    public T Z = T.CreateChecked(0);

    /// <summary>
    /// Size of the cuboid in X dimension.
    /// </summary>
    public T Width
    {
        get => T.CreateSaturating(_area.Width);
        set => _area.Width = Convert.ToDouble(value);
    }

    /// <summary>
    /// Size of the cuboid in Y dimension.
    /// </summary>
    public T Height
    {
        get => T.CreateSaturating(_area.Height);
        set => _area.Height = Convert.ToDouble(value);
    }
    /// <summary>
    /// Size of the cuboid in Y dimension.
    /// </summary>
    public T Depth = T.CreateChecked(0);

    /// <summary>
    /// Center of the cuboid.
    /// </summary>
    public Vector3D<T> Position
    {
        get => new Vector3D<T>(X, Y, Z);
        set
        {
            _area.Position = ToJypeliVector(value);
            Z = value.Z;
        } 
    }
    /// <summary>
    /// Size of the cuboid.
    /// </summary>
    public Vector3D<T> Size
    {
        get => new Vector3D<T>(Width, Height, Depth);
        set
        {
            _area.Size = ToJypeliVector(value);
            Depth = value.Z;
        }
    }


    /// <summary>
    /// Creates BoundingCuboid centered at (0, 0, 0) with size (0, 0, 0); 
    /// </summary>
    public BoundingCuboid() { }
    
    
    /// <summary>
    /// Creates BoundingCuboid from provided size and position.
    /// </summary>
    /// <param name="size">Size of the cuboid</param>
    /// <param name="position">Center point of the cuboid. If null, cuboid is positioned at (0, 0, 0)</param>
    public BoundingCuboid(Vector3D<T> size, Vector3D<T>? position = null)
    {
        Position = position ?? Vector3D<T>.Zero;
        Size = size;
    }

    
    /// <summary>
    /// Compares if provided point is inside BoundingCuboid. 
    /// </summary>
    /// <param name="point">Point to be tested</param>
    /// <returns>True, if the point is inside the cuboid, otherwise false</returns>
    public bool IsInside(Vector3D<T> point)
    {
        T halfDepth = Depth / T.CreateChecked(2.0);
        if (point.Z <= Z + halfDepth && point.Z >= Z - halfDepth )
        {
            return _area.IsInside(ToJypeliVector(point));
        }

        return false;
    }

    
    /// <summary>
    /// Truncates a Vector3D to 2 dimensional Jypeli.Vector.
    /// </summary>
    /// <param name="vector3D">Vector to truncate</param>
    /// <returns>Truncated vector</returns>
    private static Jypeli.Vector ToJypeliVector(Vector3D<T> vector3D)
    {
        return new Jypeli.Vector(Convert.ToDouble(vector3D.X), Convert.ToDouble(vector3D.Y));
    }
}