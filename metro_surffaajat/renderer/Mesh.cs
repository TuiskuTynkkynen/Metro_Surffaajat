using System.Collections.Generic;
using System.Linq;
using Jypeli;
using Silk.NET.Maths;

namespace Metro_Surffaajat.renderer;

/// <summary>
/// Currently implemented meshes.
/// Invalid represents mesh with no tris. 
/// </summary>
public enum MeshType
{
    Cube,
    
    TrainBase,
    TrainBody,
    TrainTop,
    TrainTrim,
    
    TrainBaseFront,
    TrainBodyFront,
    TrainTopFront,
    TrainTrimFront,
    
    CoinInner,
    CoinOuter,
    
    Invalid
}


/// @author Tuisku Tynkkynen
/// @version 19.10.2025
/// <summary>
/// Owns mesh data and provides getters for querying data by MeshType. 
/// </summary>
public static class Meshes
{
    /// @author Tuisku Tynkkynen
    /// @version 01.10.2025
    /// <summary>
    /// Structure for storing mesh vertices and triangles.
    /// </summary>
    private class MeshData
    {
        public Vector3D<float>[] Vertices;
        public IndexTriangle[] VertexIndices;
    }
    
    
    /// <summary>
    /// Map from MeshType to MeshData.
    /// </summary>
    private static readonly Dictionary<MeshType, MeshData> MeshesMap = new Dictionary<MeshType, MeshData>
    {
        { MeshType.Cube,  new MeshData(){
                Vertices = 
                [
                    new Vector3D<float>(-0.5f, -0.5f,  0.5f),
                    new Vector3D<float>( 0.5f, -0.5f,  0.5f),
                    new Vector3D<float>( 0.5f,  0.5f,  0.5f),
                    new Vector3D<float>(-0.5f,  0.5f,  0.5f),
                    new Vector3D<float>(-0.5f, -0.5f, -0.5f),
                    new Vector3D<float>( 0.5f, -0.5f, -0.5f),
                    new Vector3D<float>( 0.5f,  0.5f, -0.5f),
                    new Vector3D<float>(-0.5f,  0.5f, -0.5f),
                ],
                VertexIndices = [
                    // Front
                    new IndexTriangle(0, 1, 2),
                    new IndexTriangle(2, 3, 0),
                    // Back
                    new IndexTriangle(4, 5, 6),
                    new IndexTriangle(6, 7, 4),
                    // Up
                    new IndexTriangle(2, 3, 6),
                    new IndexTriangle(3, 6, 7),
                    // Down
                    new IndexTriangle(0, 1, 4),
                    new IndexTriangle(1, 4, 5),
                    // Left
                    new IndexTriangle(0, 3, 4),
                    new IndexTriangle(3, 4, 7),
                    // Right
                    new IndexTriangle(1, 2, 5),
                    new IndexTriangle(2, 5, 6),
                ],
            }  },
        { MeshType.TrainBase, new MeshData{
            Vertices = 
            [
                new Vector3D<float>( 0.5f, 0.05f,  0.5f),
                new Vector3D<float>(-0.5f, 0.05f,  0.5f),
                new Vector3D<float>(-0.5f, 0.05f, -0.5f),
                new Vector3D<float>( 0.5f, 0.05f, -0.5f),
            ],
            
            VertexIndices = [
                // Up
                new IndexTriangle(0, 1, 2),
                new IndexTriangle(2, 3, 0),
            ],
        }  },
        { MeshType.TrainBody,  new MeshData(){
            Vertices = 
            [
                new Vector3D<float>(-0.475f,-0.4f,  0.5f),
                new Vector3D<float>( 0.475f,-0.4f,  0.5f),
                new Vector3D<float>( 0.45f,  0.4f,  0.5f),
                new Vector3D<float>(-0.45f,  0.4f,  0.5f),
                
                new Vector3D<float>(-0.475f,-0.4f, -0.5f),
                new Vector3D<float>( 0.475f,-0.4f, -0.5f),
                new Vector3D<float>( 0.45f,  0.4f, -0.5f),
                new Vector3D<float>(-0.45f,  0.4f, -0.5f),
            ],
            VertexIndices = [
                // Left
                new IndexTriangle(0, 3, 4),
                new IndexTriangle(3, 4, 7),
                // Right
                new IndexTriangle(1, 2, 5),
                new IndexTriangle(2, 5, 6),
            ],
        }  },
        { MeshType.TrainTop, new MeshData{
            Vertices = 
            [
                new Vector3D<float>(-0.45f, -0.05f,  0.5f), 
                new Vector3D<float>( 0.45f, -0.05f,  0.5f), 
                new Vector3D<float>(-0.325f, 0.05f,  0.5f), 
                new Vector3D<float>( 0.325f, 0.05f,  0.5f), 
                
                new Vector3D<float>( 0.45f, -0.05f, -0.5f), 
                new Vector3D<float>(-0.45f, -0.05f, -0.5f), 
                new Vector3D<float>( 0.325f, 0.05f, -0.5f), 
                new Vector3D<float>(-0.325f, 0.05f, -0.5f), 
            ],
            
            VertexIndices = [
                // Up
                new IndexTriangle( 2, 3, 6),
                new IndexTriangle( 6, 7, 2),
                
                // Left
                new IndexTriangle( 0, 2, 5),
                new IndexTriangle( 5, 7, 2),
                
                // Right
                new IndexTriangle( 1, 3, 4),
                new IndexTriangle( 4, 6, 3),
            ],
        }  },
        { MeshType.TrainTrim, new MeshData {
            Vertices = [
                new Vector3D<float>(-0.475f, -0.4f,  0.45f),
                new Vector3D<float>(-0.475f, -0.4f,  0.5f),
                new Vector3D<float>(-0.45f,   0.4f,  0.5f),
                new Vector3D<float>(-0.45f,   0.4f,  0.45f),
                
                new Vector3D<float>(-0.475f, -0.4f, -0.45f),
                new Vector3D<float>(-0.475f, -0.4f, -0.5f),
                new Vector3D<float>(-0.45f,   0.4f, -0.5f),
                new Vector3D<float>(-0.45f,   0.4f, -0.45f),
                                                 
                new Vector3D<float>(0.475f, -0.4f,  0.45f),
                new Vector3D<float>(0.475f, -0.4f,  0.5f),
                new Vector3D<float>(0.45f,   0.4f,  0.5f),
                new Vector3D<float>(0.45f,   0.4f,  0.45f),
                
                new Vector3D<float>(0.475f, -0.4f, -0.45f),
                new Vector3D<float>(0.475f, -0.4f, -0.5f),
                new Vector3D<float>(0.45f,   0.4f, -0.5f),
                new Vector3D<float>(0.45f,   0.4f, -0.45f),
            ],
                
            VertexIndices = [
                    // Left Front
                    new IndexTriangle(0, 1, 2),
                    new IndexTriangle(2, 3, 0),
                    // Left Back
                    new IndexTriangle(4, 5, 6),
                    new IndexTriangle(6, 7, 4),
                    
                    // Right Front
                    new IndexTriangle(8, 9, 10),
                    new IndexTriangle(10, 11, 8),
                    // Right Back
                    new IndexTriangle(12, 13, 14),
                    new IndexTriangle(14, 15, 12),
            ],
        } },
        { MeshType.TrainBaseFront, new MeshData{
            Vertices = 
            [
                new Vector3D<float>(-0.475f, -0.05f,  0.0f),
                new Vector3D<float>( 0.475f, -0.05f,  0.0f),
                new Vector3D<float>( 0.5f,    0.05f,  0.0f),
                new Vector3D<float>(-0.5f,    0.05f,  0.0f),
            ],
            
            VertexIndices = [
                new IndexTriangle(0, 1, 2),
                new IndexTriangle(2, 3, 0),
            ],
        }  },
        { MeshType.TrainBodyFront,  new MeshData(){
            Vertices = 
            [
                new Vector3D<float>(-0.475f,-0.4f,  0.0f),
                new Vector3D<float>( 0.475f,-0.4f,  0.0f),
                new Vector3D<float>( 0.45f,  0.4f,  0.0f),
                new Vector3D<float>(-0.45f,  0.4f,  0.0f),
            ],
            VertexIndices = [
                new IndexTriangle(0, 1, 2),
                new IndexTriangle(2, 3, 0),
            ],
        }  },
        { MeshType.TrainTopFront, new MeshData{
            Vertices = 
            [
                new Vector3D<float>(-0.45f, -0.05f,  0.0f),
                new Vector3D<float>( 0.45f, -0.05f,  0.0f),
                new Vector3D<float>( 0.415f, 0.0f,   0.0f),
                new Vector3D<float>(-0.415f, 0.0f,   0.0f),
                new Vector3D<float>(-0.325f, 0.05f,  0.0f),
                new Vector3D<float>( 0.325f, 0.05f,  0.0f),
            ],
            
            VertexIndices = [
                // Lower
                new IndexTriangle(0, 1, 2),
                new IndexTriangle(2, 3, 0),
                
                // Upper
                new IndexTriangle(2, 3, 4),
                new IndexTriangle(4, 5, 2),
            ],
        }  },
        { MeshType.TrainTrimFront, new MeshData {
                Vertices = [
                    new Vector3D<float>(-0.475f, -0.4f, 0.0f),
                    new Vector3D<float>(-0.4f,   -0.4f, 0.0f),
                    new Vector3D<float>(-0.375f,  0.4f, 0.0f),
                    new Vector3D<float>(-0.45f,   0.4f, 0.0f), 
                    
                    new Vector3D<float>( 0.475f, -0.4f, 0.0f),
                    new Vector3D<float>( 0.4f,   -0.4f, 0.0f),
                    new Vector3D<float>( 0.375f,  0.4f, 0.0f),
                    new Vector3D<float>( 0.45f,   0.4f, 0.0f),
                ],
                
                VertexIndices = [
                    // Left
                    new IndexTriangle(0, 1, 2),
                    new IndexTriangle(2, 3, 0),
                    // Right
                    new IndexTriangle(4, 5, 6),
                    new IndexTriangle(6, 7, 4),
                ],
        } },
        { MeshType.CoinInner, new MeshData {
                Vertices = Enumerable.Range(0, 10)
                    .Select(index => {
                                float angle = float.DegreesToRadians(90.0f + index * (360.0f / 5));
                                return new Vector3D<float>(float.Cos(angle), float.Sin(angle), index >= 5 ? -0.1f : 0.1f);
                    }).ToArray(),
                
                VertexIndices = [
                    // Front
                    new IndexTriangle(0, 1, 2),
                    new IndexTriangle(2, 3, 0),
                    new IndexTriangle(3, 4, 0),
                    
                    // Back
                    new IndexTriangle(5, 6, 7),
                    new IndexTriangle(7, 8, 5),
                    new IndexTriangle(8, 9, 5),
                    
                    // Left
                    new IndexTriangle(0, 1, 5),
                    new IndexTriangle(5, 6, 1),
                    
                    // Right
                    new IndexTriangle(0, 4, 5),
                    new IndexTriangle(5, 9, 4),
                    
                    // Down
                    new IndexTriangle(2, 3, 7),
                    new IndexTriangle(7, 8, 3),
                ],
        } },
        { MeshType.CoinOuter, new MeshData {
                Vertices = Enumerable.Range(0, 20)
                    .Select(index => {
                        float angle = float.DegreesToRadians((int)(index / 4)  * (360.0f / 5) + 90.0f);
                        float radius = index % 4 < 2 ? 0.75f : 1.0f;
                        return new Vector3D<float>(float.Cos(angle) * radius, float.Sin(angle) * radius, index % 2 == 1 ? -0.1f : 0.1f); })
                    .ToArray(),
                
                VertexIndices = Enumerable.Range(0, 5)
                    .SelectMany(i => {
                        int offset = i * 4;
                        return new IndexTriangle[] {
                            // Front 
                            new (offset + 0, offset + 2, (offset + 4) % 20),
                            new (offset + 2, (offset + 4) % 20, (offset + 6) % 20),
                            
                            // Back
                            new (offset + 1, offset + 3, (offset + 5) % 20),
                            new (offset + 3, (offset + 5) % 20, (offset + 7) % 20),   
                            
                            // Up 
                            new (offset + 2, offset + 3, (offset + 6) % 20),
                            new (offset + 3, (offset + 6) % 20, (offset + 7) % 20),    
                        }; })
                    .ToArray(),
        } },
    };

    
    /// <summary>
    /// Getter for mesh vertex positions.
    /// </summary>
    /// <param name="type">Type of mesh</param>
    /// <returns>Array of vertex positions or an empty array for invalid mesh type</returns>
    public static Vector3D<float>[] GetMeshVertices(MeshType type)
    {
        if (type >= MeshType.Invalid)
        {
            return [];
        }
        
        return MeshesMap[type].Vertices;
    }
    
    
    /// <summary>
    /// Getter for mesh triangle indices.
    /// </summary>
    /// <param name="type">Type of mesh</param>
    /// <returns>Array of triangle indices or an empty array for invalid mesh type</returns>
    public static IndexTriangle[] GetMeshIndices(MeshType type)
    {
        if (type >= MeshType.Invalid)
        {
            return [];
        }
        
        return MeshesMap[type].VertexIndices;
    }
}
