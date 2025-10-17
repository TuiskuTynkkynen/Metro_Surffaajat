using System;

namespace Metro_Surffaajat.utility;

/// @author Tuisku Tynkkynen
/// @version 17.10.2025
/// <summary>
/// Represents a 3d rotation as a pitch, roll and yaw angles in radians.
/// </summary>
/// <typeparam name="T">. Must be an unmanaged floating point type</typeparam>
public readonly struct Rotation<T> : IEquatable<Rotation<T>> 
    where T : unmanaged, System.Numerics.IFloatingPoint<T>
{
    /// <summary>
    /// Internal array for storing pitch, roll and yaw.
    /// </summary>
    private readonly T[] _angles;
    
    /// <summary>
    /// Pitch as radians.
    /// </summary>
    public T Pitch 
    {
        get => this._angles[0];
        set => this._angles[0] = value;
    }
    /// <summary>
    /// Roll as radians.
    /// </summary>
    public T Roll 
    {
        get => this._angles[1];
        set => this._angles[1] = value;
    }
    /// <summary>
    /// Yaw as radians.
    /// </summary>
    public T Yaw 
    {
        get => this._angles[2];
        set => this._angles[2] = value;
    }
    
    /// <summary>
    /// Pitch as degrees.
    /// </summary>
    public T PitchDegrees 
    {
        get => RadianToDegree(this._angles[0]);
        set => this._angles[0] = DegreeToRadian(value);
    }
    /// <summary>
    /// Roll as degrees.
    /// </summary>
    public T RollDegrees 
    {
        get => RadianToDegree(this._angles[1]);
        set => this._angles[1] = DegreeToRadian(value);
    }
    /// <summary>
    /// Yaw as degrees.
    /// </summary>
    public T YawDegrees 
    {
        get => RadianToDegree(this._angles[2]);
        set => this._angles[2] = DegreeToRadian(value);
    }


    /// <summary>
    /// Creates a rotation with all angles set to 0.
    /// </summary>
    public Rotation()
    {
        _angles = [T.Zero, T.Zero, T.Zero];
    }
    
    
    /// <summary>
    /// Creates a Rotation from pitch, roll, and yaw angles in radians.
    /// </summary>
    /// <param name="pitch">Pitch in radians</param>
    /// <param name="roll">Roll in radians</param>
    /// <param name="yaw">Yaw in radians</param>
    public Rotation(T? pitch = null, T? roll = null, T? yaw = null)
    {
        _angles = [pitch ?? T.Zero, roll ?? T.Zero, yaw ?? T.Zero];
    }
    
    
    /// <summary>
    /// Returns a rotation with all angles set to 0.
    /// </summary>
    public static Rotation<T> Zero => new();
    
    /// <summary>
    /// Creates a Rotation from pitch, roll, and yaw angles in radians.
    /// The same as using the constructor.
    /// </summary>
    /// <param name="pitch">Pitch in radians</param>
    /// <param name="roll">Roll in radians</param>
    /// <param name="yaw">Yaw in radians</param>
    public static Rotation<T> FromRadians(T? pitch = null, T? roll = null, T? yaw = null) => new Rotation<T>(pitch, roll, yaw);
    
    
    /// <summary>
    /// Creates a Rotation from pitch, roll, and yaw angles in degrees.
    /// </summary>
    /// <param name="pitch">Pitch in degrees</param>
    /// <param name="roll">Roll in degrees</param>
    /// <param name="yaw">Yaw in degrees</param>
    public static Rotation<T> FromDegrees(T? pitch = null, T? roll = null, T? yaw = null) => new Rotation<T>(DegreeToRadian(pitch ?? T.Zero), DegreeToRadian(roll ?? T.Zero), DegreeToRadian(yaw ?? T.Zero));
    
    
    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="value">The angle in degrees</param>
    /// <returns>The angle in radians</returns>
    public static T DegreeToRadian(T value)
    {
        return (value * (T.Pi / T.CreateChecked(180.0)));
    }
    
    
    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    /// <param name="value">The angle in radians</param>
    /// <returns>The angle in degrees</returns>
    public static T RadianToDegree(T value)
    {
        return (value * (T.CreateChecked(180.0) / T.Pi));
    }


    /// <summary>
    /// Compares if two Rotations are equal.
    /// </summary>
    /// <param name="a">First Rotation</param>
    /// <param name="b">Second Rotation</param>
    /// <returns>True if the Rotations are equal and false otherwise</returns>
    public static bool operator ==(Rotation<T> a, Rotation<T> b)
    {
        return a.Equals(b);
    }


    /// <summary>
    /// Compares if two Rotations are not equal.
    /// </summary>
    /// <param name="a">First Rotation</param>
    /// <param name="b">Second Rotation</param>
    /// <returns>True if the Rotations are not equal and false otherwise</returns>
    public static bool operator !=(Rotation<T> a, Rotation<T> b)
    {
        return !a.Equals(b);
    }

    
    /// <summary>
    /// Compares this Rotation instance to provided Rotation.
    /// </summary>
    /// <param name="other">Rotation this is compared to</param>
    /// <returns>True if this and other are equal and false otherwise</returns>
    public bool Equals(Rotation<T> other)
    {
        return Pitch.Equals(other.Pitch) && Roll.Equals(other.Pitch) && Yaw.Equals(other.Yaw);
    }
    
    
    /// <summary>
    /// Compares this Rotation instance to provided object.
    /// </summary>
    /// <param name="obj">Object this is compared to</param>
    /// <returns>True if this and obj are equal and false otherwise</returns>
    public override bool Equals(object obj)
    {
        return obj is Rotation<T> other && Equals(other);
    }

    
    /// <summary>
    /// Returns the hash code for this Rotation instance.
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return _angles.GetHashCode();
    }

}