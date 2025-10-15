using Jypeli;
using Silk.NET.Maths;

namespace Metro_Surffaajat.utility;

/// @author Tuisku Tynkkynen
/// @version 15.10.2025
/// <summary>
/// Collection of utility functions.
/// </summary>
public static class Utility
{
    /// <summary>
    /// Creates a transformation matrix from the provided vectors.
    /// </summary>
    /// <param name="position">Specifies the amount of translation on each axis. Null and Vector3D.Zero values are ignored</param>
    /// <param name="rotation">Specifies the rotation. Null and Rotation.Zero values are ignored</param>
    /// <param name="scale">Specifies the amount of scale on each axis. Null and Vector3D.One values are ignored</param>
    /// <typeparam name="T">Type of the elements of the matrix and vectors. Must be an unmanaged floating point type</typeparam>
    /// <returns>The transformation matrix of type T</returns>
    public static Matrix4X4<T> CreateTransform<T>(Vector3D<T>? position = null, Rotation<T>? rotation = null, Vector3D<T>? scale = null)
        where T : unmanaged, System.Numerics.IFloatingPoint<T>
    {
        bool requiresScale = scale.HasValue && scale.Value != Vector3D<T>.One; 
        Matrix4X4<T> result = (requiresScale) ?  Matrix4X4.CreateScale(scale.Value) : Matrix4X4<T>.Identity;
        
        bool requiresRotation = rotation.HasValue && rotation.Value != Rotation<T>.Zero; 
        if (requiresRotation)
        {
            result *= Matrix4X4.CreateFromYawPitchRoll(rotation.Value.Yaw, rotation.Value.Pitch, rotation.Value.Roll);
        }
        
        bool requiresTranslation = position.HasValue && position.Value != Vector3D<T>.Zero;
        if (requiresTranslation)
        {
            result *= Matrix4X4.CreateTranslation(position.Value);
        }
        
        return result;
    }


    /// <summary>
    /// Linearly interpolates two colors in sRGB based of specified weight and clamps
    /// result to sRGB.
    /// </summary>
    /// <param name="a">First Color</param>
    /// <param name="b">Second Color</param>
    /// <param name="weight">Weight between 0 and 1</param>
    /// <returns>Interpolated Color</returns>
    public static Color Lerp(Color a, Color b, double weight)
    {
        double alpha = double.Lerp(a.AlphaComponent, b.AlphaComponent, weight) / byte.MaxValue;
        double red = double.Lerp(a.RedComponent, b.RedComponent, weight) / byte.MaxValue;
        double green = double.Lerp(a.GreenComponent, b.GreenComponent, weight) / byte.MaxValue;
        double blue = double.Lerp(a.BlueComponent, b.BlueComponent, weight) / byte.MaxValue;

        return new Color(double.Clamp(red, 0, 1), double.Clamp(green, 0, 1), double.Clamp(blue, 0, 1), double.Clamp(alpha, 0, 1));
    }
}