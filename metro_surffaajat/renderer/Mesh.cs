using System.Collections.Generic;
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
    Invalid
}


/// @author Tuisku Tynkkynen
/// @version 01.10.2025
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
