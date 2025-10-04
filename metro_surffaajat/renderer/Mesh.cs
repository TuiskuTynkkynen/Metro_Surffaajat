using System.Collections.Generic;
using Jypeli;
using Silk.NET.Maths;

namespace Metro_Surffaajat.renderer;

public enum MeshType
{
    Cube,
    Invalid
}


public static class Meshes
{
    private class MeshData
    {
        public Vector3D<float>[] Vertices;
        public IndexTriangle[] VertexIndices;
    }
    
    
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

    
    public static Vector3D<float>[] GetMeshVertices(MeshType type)
    {
        if (type >= MeshType.Invalid)
        {
            return [];
        }
        
        return MeshesMap[type].Vertices;
    }
    
    
    public static IndexTriangle[] GetMeshIndices(MeshType type)
    {
        if (type >= MeshType.Invalid)
        {
            return [];
        }
        
        return MeshesMap[type].VertexIndices;
    }
}
