using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Silk.NET.Maths;
using Jypeli;
using Metro_Surffaajat.renderer;
using Metro_Surffaajat.utility;

namespace Metro_Surffaajat.game;


/// <summary>
/// Currently implemented CoinTrails.
/// Invalid coin trail should never appear. 
/// </summary>
public enum CoinTrailType : uint
{
    None, 
    Low,
    Left,
    Right,
    High,
    Up,
    Apex,
    Down,
    Invalid
}


/// @author Tuisku Tynkkynen
/// @version 19.10.2025
/// <summary>
/// Owns CoinTrail data and provides getters for querying data by CoinTrailType. 
/// </summary>
public static class CoinTrails
{   
    /// <summary>
    /// Type of model for all coins.
    /// </summary>
    public const ModelType ModelType = renderer.ModelType.Coin;
    public const int MaxSegmentIndex = 2;
    
    /// @author Tuisku Tynkkynen
    /// @version 19.10.2025
    /// <summary>
    /// Structure for storing coin trail related data.
    /// </summary>
    /// <param name="continueChance">How likely the next coin trail is to have the same CoinTrailType</param>
    /// <param name="validOn">Array of ObstacleTypes that this CoinTrailType can be placed on</param>
    /// <param name="validNext">Array of CoinTrailTypes that can follow this CoinTrailType</param>
    private readonly struct CoinTrailData(float continueChance, ObstacleType[] validOn, CoinTrailType[] validNext)
    {   
        public readonly float ContinueChance = continueChance;
        public readonly ObstacleType[] ValidOn = validOn;
        public readonly CoinTrailType[] ValidNext = validNext;
    }

    /// <summary>
    /// Alias for [ObstacleType.None, ObstacleType.TrainStart, ObstacleType.Train, ObstacleType.TrainEnd, ObstacleType.Invalid].
    /// </summary>
    private static readonly ObstacleType[] ValidOnAll = [ObstacleType.None, ObstacleType.TrainStart, ObstacleType.Train, ObstacleType.TrainEnd, ObstacleType.Invalid];
    /// <summary>
    /// Alias for [ObstacleType.None].
    /// </summary>
    private static readonly ObstacleType[] ValidOnNone = [ObstacleType.None];
    /// <summary>
    /// [ObstacleType.TrainStart, ObstacleType.Train, ObstacleType.TrainEnd]
    /// </summary>
    private static readonly ObstacleType[] ValidOnTrain = [ObstacleType.TrainStart, ObstacleType.Train, ObstacleType.TrainEnd];
    
    /// <summary>
    /// Map from CoinTrailType to CoinTrailData.
    /// </summary>
    private static readonly Dictionary<CoinTrailType, CoinTrailData> TrailDataMap = new(){
        {CoinTrailType.None, new (0.75f, ValidOnAll, [CoinTrailType.Low, CoinTrailType.High])},
        {CoinTrailType.Low, new (0.5f, ValidOnNone, [CoinTrailType.None, CoinTrailType.Left, CoinTrailType.Right])},
        {CoinTrailType.Left, new (0.0f, ValidOnNone, [CoinTrailType.None])},
        {CoinTrailType.Right, new (0.0f, ValidOnNone, [CoinTrailType.None])},
        {CoinTrailType.High, new (0.75f, ValidOnTrain, [CoinTrailType.None, CoinTrailType.Up])},
        {CoinTrailType.Up, new (0.0f, [ObstacleType.TrainEnd], [CoinTrailType.Apex])},
        {CoinTrailType.Apex, new (0.0f, ValidOnAll, [CoinTrailType.Down])},
        {CoinTrailType.Down, new (0.0f, ValidOnAll, [CoinTrailType.High])},
    };

    
    /// <summary>
    /// Checks if coin trail can be placed on specified obstacle. 
    /// </summary>
    /// <param name="type">Type of coin trail</param>
    /// <param name="on">Type of obstacle to be checked</param>
    /// <returns>True, if coin trail can be placed on the obstacle, otherwise false</returns>
    public static bool IsValidPlacement(CoinTrailType type, ObstacleType on)
    {
        if (type >= CoinTrailType.Invalid)
        {
            Debug.Fail("Coin trail type should not be Invalid");
            return false;
        }

        return TrailDataMap[type].ValidOn.Contains(on);
    }
    
    
    /// <summary>
    /// Calculates the following coin trail for specified CoinTrailType.
    /// </summary>
    /// <param name="current">Type of current coin trail</param>
    /// <param name="currentSegmentIndex">Index of the current coin in the trail. Must be equal to or less than MaxSegmentIndex</param>
    /// <param name="currentCount">Count of coins on the current segment</param>
    /// <returns>Type of next coin trail</returns>
    public static CoinTrailType GetNext(CoinTrailType current, uint currentSegmentIndex, uint currentCount)
    {
        Debug.Assert(currentSegmentIndex <= MaxSegmentIndex, "Index should be less than maximum segment index");
            
        if (current >= CoinTrailType.Invalid)
        {
            Debug.Fail("Coin trail type should not be Invalid");
            return CoinTrailType.Invalid;
        }

        if (currentSegmentIndex < MaxSegmentIndex || RandomGen.NextDouble(0.0, 1.0) <= TrailDataMap[current].ContinueChance)
        {
            return current;
        }

        if (currentCount != 0 && RandomGen.NextInt((int)currentCount) != 0)
        {
            return CoinTrailType.None;
        }
        
        return RandomGen.SelectOne(TrailDataMap[current].ValidNext);
    }


     
    /// <summary>
    /// Calculates the AABB of the n:th coin in a trail. 
    /// </summary>
    /// <param name="type">Type of coin trail. Must be a valid CoinTrailType</param>
    /// <param name="segmentIndex">Index of the coin in the trail. Must be equal to or less than MaxSegmentIndex</param>
    /// <returns>Position of the coin</returns>
    public static BoundingCuboid<float>? GetAABB(CoinTrailType type, uint segmentIndex)
    {
        Debug.Assert(type <= CoinTrailType.Invalid, "Specified coin trail type was not valid");
        Debug.Assert(segmentIndex <= MaxSegmentIndex, "Index should be less than maximum segment index");

        Vector3D<float>? position = CalculatePosition(type, segmentIndex);
        Vector3D<float> size = new(0.25f);
        return position.HasValue ? new BoundingCuboid<float>(position.Value, size) : null;
    }
    
    
    /// <summary>
    /// Calculates the position of the n:th coin in a trail. 
    /// </summary>
    /// <param name="type">Type of coin trail. Must be a valid CoinTrailType</param>
    /// <param name="segmentIndex">Index of the coin in the trail. Must be equal to or less than MaxSegmentIndex</param>
    /// <returns>Position of the coin</returns>
    public static Vector3D<float>? CalculatePosition(CoinTrailType type, uint segmentIndex)
    {
        Debug.Assert(type <= CoinTrailType.Invalid, "Specified coin trail type was not valid");
        Debug.Assert(segmentIndex <= MaxSegmentIndex, "Index should be less than maximum segment index");
        
        const float yBase = -0.5f / (MaxSegmentIndex + 1);
        const float xBase = 0.0f;
        Vector3D<float> result = new(x: xBase, y: yBase, z: 0.0f);

        const float GapSize = 0.5f / (MaxSegmentIndex + 1);
        switch(type) {
            case CoinTrailType.Left:
                result.X -= segmentIndex * GapSize;
                break;
            case CoinTrailType.Right:
                result.X += segmentIndex * GapSize;
                break;
            case CoinTrailType.High:
                result.Y += 1.0f;
                break;
            case CoinTrailType.Up:
                result.Y += 1.0f;
                result.Y += segmentIndex * GapSize;
                break;
            case CoinTrailType.Apex:
                result.Y += 2.0f;
                result.Y += Convert.ToSingle(segmentIndex == 1) * GapSize;
                break;
            case CoinTrailType.Down:
                result.Y += 1.0f;
                result.Y += (MaxSegmentIndex - segmentIndex) * GapSize;
                break;
            case CoinTrailType.Low:
                break;
            case CoinTrailType.None:
            case CoinTrailType.Invalid:
                return null;
        }
        
        return result;
    }
}