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
/// @version 16.10.2025
/// <summary>
/// Owns obstacle data and provides getters for querying data by ObstacleType. 
/// </summary>
public static class Obstacles
{
    /// @author Tuisku Tynkkynen
    /// @version 16.10.2025
    /// <summary>
    /// Structure for storing obstacle related data-
    /// </summary>
    /// <param name="modelType">Type of Model associated with obstacle</param>
    /// <param name="bounds">AABB of the obstacle</param>
    /// <param name="continueChance">How likely the next obstacle is to have the same ObstacleType</param>
    /// <param name="validNext">Array of ObstacleTypes that can follow this ObstacleType</param>
    private readonly struct ObstacleData(ModelType modelType, BoundingCuboid<float> bounds, float continueChance, ObstacleType[] validNext)
    {   
        public readonly ModelType ModelType = modelType;
        public readonly BoundingCuboid<float> Bounds = bounds;
        public readonly float ContinueChance = continueChance;
        public readonly ObstacleType[] ValidNext = validNext;

    }
        
    
    /// <summary>
    /// Map from ObstacleType to ObstacleData.
    /// </summary>
    private static readonly Dictionary<ObstacleType, ObstacleData> ObstacleDataMap = new(){
        {ObstacleType.None, new (ModelType.Invalid, new BoundingCuboid<float>(), 0.75f, [ObstacleType.TrainStart])},
        {ObstacleType.TrainStart, new (ModelType.WhiteCube, new (Vector3D<float>.One), 0.0f, [ObstacleType.Train])},
        {ObstacleType.Train, new (ModelType.BlueCube, new (Vector3D<float>.One), 0.75f, [ObstacleType.TrainEnd])},
        {ObstacleType.TrainEnd, new (ModelType.WhiteCube, new (Vector3D<float>.One), 0.0f, [ObstacleType.None])},
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
    public static ObstacleType GetNextObstacle(ObstacleType current)
    {
        if (current >= ObstacleType.Invalid)
        {
            Debug.Fail("Obstacle type should not be Invalid");
            return ObstacleType.None;
        }

        if (RandomGen.NextDouble(0.0, 1.0) <= ObstacleDataMap[current].ContinueChance)
        {
            return current;
        }

        return RandomGen.SelectOne(ObstacleDataMap[current].ValidNext);
    }
}