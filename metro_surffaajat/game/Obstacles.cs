using System;
using System.Collections.Generic;
using System.Diagnostics;
using Jypeli;
using Metro_Surffaajat.renderer;
using Metro_Surffaajat.utility;
using Silk.NET.Maths;

namespace Metro_Surffaajat.game;

/// <summary>
/// Currently implemented Obstacles.
/// Invalid obstacles should not appear. 
/// </summary>
public enum ObstacleType
{
    None,
    TrainStart,
    Train,
    TrainEnd,
    Invalid,
}


/// @author Tuisku Tynkkynen
/// @version 19.10.2025
/// <summary>
/// Owns obstacle data and provides getters for querying data by ObstacleType. 
/// </summary>
public static class Obstacles
{
    /// @author Tuisku Tynkkynen
    /// @version 19.10.2025
    /// <summary>
    /// Structure for storing obstacle related data.
    /// </summary>
    /// <param name="modelType">Type of Model associated with obstacle</param>
    /// <param name="bounds">AABB of the obstacle</param>
    /// <param name="minRepeat">The minimum number of times this ObstacleType is repeated</param>
    /// <param name="continueChance">How likely the next obstacle is to have the same ObstacleType</param>
    /// <param name="validNext">Array of ObstacleTypes that can follow this ObstacleType</param>
    private readonly struct ObstacleData(ModelType modelType, BoundingCuboid<float> bounds, uint minRepeat, float continueChance, ObstacleType[] validNext)
    {   
        public readonly ModelType ModelType = modelType;
        public readonly BoundingCuboid<float> Bounds = bounds;
        public readonly uint MinRepeat = minRepeat;
        public readonly float ContinueChance = continueChance;
        public readonly ObstacleType[] ValidNext = validNext;
    }
        
    
    /// <summary>
    /// Map from ObstacleType to ObstacleData.
    /// </summary>
    private static readonly Dictionary<ObstacleType, ObstacleData> ObstacleDataMap = new(){
        {ObstacleType.None, new (ModelType.Invalid, new BoundingCuboid<float>(), 3, 0.75f, [ObstacleType.TrainStart])},
        {ObstacleType.TrainStart, new (ModelType.TrainCargoFront, new (Vector3D<float>.One), 1, 0.0f, [ObstacleType.Train])},
        {ObstacleType.Train, new (ModelType.TrainCargo, new (Vector3D<float>.One), 2, 0.75f, [ObstacleType.TrainEnd])},
        {ObstacleType.TrainEnd, new (ModelType.TrainCargo, new (Vector3D<float>.One), 3, 0.0f, [ObstacleType.None])},
    };
    
    
    /// <summary>
    /// Getter for ModelType associated with ObstacleType.
    /// </summary>
    /// <param name="type">Type of obstacle</param>
    /// <returns>ModelType associated with ObstacleType</returns>
    public static ModelType GetModelType(ObstacleType type)
    {
        if (type >= ObstacleType.Invalid)
        {
            Debug.Fail("Obstacle type should not be Invalid");
            return ModelType.Invalid;
        }
        
        return ObstacleDataMap[type].ModelType;
    }

    
    /// <summary>
    /// Getter for BoundingCuboid associated with ObstacleType.
    /// </summary>
    /// <param name="type">Type of obstacle</param>
    /// <returns>AABB of the ObstacleType</returns>
    public static BoundingCuboid<float> GetBounds(ObstacleType type)
    {
        if (type >= ObstacleType.Invalid)
        {
            Debug.Fail("Obstacle type should not be Invalid");
            return new BoundingCuboid<float>();
        }

        return ObstacleDataMap[type].Bounds;
    }

    
    /// <summary>
    /// Calculates the following obstacle for specified ObstacleType.
    /// </summary>
    /// <param name="current">Type of current obstacle</param>
    /// <returns>Type of the next obstacle</returns>
    public static ObstacleType GetNext(ObstacleType current, uint segmentIndex)
    {
        if (current >= ObstacleType.Invalid)
        {
            Debug.Fail("Obstacle type should not be Invalid");
            return ObstacleType.None;
        }

        if (segmentIndex < ObstacleDataMap[current].MinRepeat || RandomGen.NextDouble(0.0, 1.0) <= ObstacleDataMap[current].ContinueChance)
        {
            return current;
        }

        return RandomGen.SelectOne(ObstacleDataMap[current].ValidNext);
    }
}