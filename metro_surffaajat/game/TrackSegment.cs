using System;
using System.Diagnostics;
using System.Linq;

namespace Metro_Surffaajat.game;

/// <summary>
/// Structure for storing a segment of obstacles.
/// </summary>
public class ObstacleSegment
{
    /// <summary>
    /// Maximum count of elements in internal array.
    /// </summary>
    private const int Capacity = 3;
    /// <summary>
    /// Count of elements in internal array.
    /// </summary>
    public readonly int Length = 3;
    /// <summary>
    /// Internal array of obstacles. 
    /// </summary>
    private readonly ObstacleType[] _obstacles;
    /// <summary>
    /// Segment indices for each obstacle.
    /// </summary>
    private readonly uint[] _segmentIndices;
    
    /// <summary>
    /// Gets obstacle type at specified index.
    /// </summary>
    /// <param name="index">Index of the obstacle to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">Specified index has to be less than Length</exception>
    public ObstacleType this[uint index] {
        get
        {
            if (index > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"was not valid index of ObstacleSegment with Length {Length}");
            }

            return _obstacles[index];
        }
    }
    
    
    /// <summary>
    /// Create an ObstacleSegment.
    /// </summary>
    public ObstacleSegment()
    {
        _segmentIndices = [0, 0, 0];
        _obstacles = [ObstacleType.None, ObstacleType.None, ObstacleType.None];
    }

    
    /// <summary>
    /// Create an ObstacleSegment with specified obstacles and indices.
    /// </summary>
    /// <param name="obstacles">Obstacles in the segment. Array length should equal Capacity</param>
    /// <param name="segmentIndices">Indices of each obstacle in the segment. Array length should equal Capacity</param>
    private ObstacleSegment(ObstacleType[] obstacles, uint[] segmentIndices)
    {   
        Debug.Assert(obstacles.Length == Capacity);
        Debug.Assert(segmentIndices.Length == Capacity);
        
        _segmentIndices = segmentIndices;
        _obstacles = obstacles;
    }
    
    
    /// <summary>
    /// Calculates the following ObstacleSegment for this segment.
    /// </summary>
    /// <returns>The next ObstacleSegment</returns>
    public ObstacleSegment GetNext()
    {
        ObstacleType[] nextObstacles = new ObstacleType[Capacity];
        uint[] nextIndices = new uint[Capacity];
        
        for (int i = 0; i < Capacity; i++)
        {
            nextObstacles[i] = Obstacles.GetNext(_obstacles[i]);
            nextIndices[i] = nextObstacles[i] == _obstacles[i] ? _segmentIndices[i] + 1 : 0;
        }

        return new ObstacleSegment(nextObstacles, nextIndices);
    }
}



/// <summary>
/// Structure for storing a segment of coins.
/// </summary>
public class CoinSegment
{                                   
    /// <summary>
    /// Maximum count of elements in internal array.
    /// </summary>
    private const uint Capacity = 3;
    /// <summary>
    /// Count of elements in internal array.
    /// </summary>
    public readonly int Length = 3; 
    /// <summary>
    /// Internal array of coins. 
    /// </summary>
    private readonly CoinTrailType[] _coins; 
    /// <summary>
    /// Segment index for each coin.
    /// </summary>
    public readonly uint SegmentIndex;      
    
    
    /// <summary>
    /// Gets coin trail type at specified index.
    /// </summary>
    /// <param name="index">Index of the coin trail to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">Specified index has to be less than Length</exception>
    public CoinTrailType this[uint index] {
        get
        {
            if (index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"was not valid index of CoinSegment with Length {Length}");
            }

            return _coins[index];
        }
    }
    
    
    /// <summary>
    /// Creates a CoinSegment.
    /// </summary>
    public CoinSegment()
    {
        SegmentIndex = 0;
        _coins = [CoinTrailType.None, CoinTrailType.None, CoinTrailType.None];
    }

    
    /// <summary>
    /// Creates a CoinSegment with specified coin trail types and index.
    /// </summary>
    /// <param name="coins">Coin trails in the segment. Array length should equal Capacity</param>
    /// <param name="index">Segment index of each coin</param>
    private CoinSegment(CoinTrailType[] coins, uint index)
    {
        Debug.Assert(index <= CoinTrails.MaxSegmentIndex);
        
        SegmentIndex = index;
        _coins = coins;
    }
    
    
    /// <summary>
    /// Calculates the following CoinSegment this segment based on specified next ObstacleSegment.
    /// </summary>
    /// <param name="nextObstacles">The next ObstacleSegment</param>
    /// <returns>The next CoinSegment</returns>
    public CoinSegment GetNext(ObstacleSegment nextObstacles)
    {
        int lenght = int.Min(Length, nextObstacles.Length);
        
        CoinTrailType[] nextCoins = new CoinTrailType[Capacity];
        uint nextIndex = SegmentIndex != CoinTrails.MaxSegmentIndex ? SegmentIndex + 1 : 0;
        uint count = (uint)_coins.Count(type => !type.Equals(CoinTrailType.None));
        
        for (uint i = 0; i < lenght; i++)
        {
            CoinTrailType nextType = CoinTrails.GetNext(_coins[i], SegmentIndex, count);
            bool valid = CoinTrails.IsValidPlacement(nextType, nextObstacles[i]);

            if (nextType == CoinTrailType.Left)
            {
                valid &= i > 0 &&  CoinTrails.IsValidPlacement(nextType, nextObstacles[i - 1]);
            }
            else if (nextType == CoinTrailType.Right)
            {
                valid &= i < (Capacity - 1) &&  CoinTrails.IsValidPlacement(nextType, nextObstacles[i + 1]);
            }
            
            nextCoins[i] = valid ? nextType : CoinTrailType.None;
        }

        return new CoinSegment(nextCoins, nextIndex);
    }
}