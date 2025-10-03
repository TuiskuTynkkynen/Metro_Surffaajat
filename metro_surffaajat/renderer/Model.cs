using System.Collections.Generic;
using System.Diagnostics;
using Jypeli;
using Silk.NET.Maths;

namespace Metro_Surffaajat.renderer;

public enum ModelType
{
    WhiteCube,
    BlueCube,
    Invalid,
}


public class Model(ModelType type)
{
    public Vector3D<float> Position = Vector3D<float>.Zero;
    public Vector3D<float> Rotation = Vector3D<float>.Zero;
    public Vector3D<float> Scale = Vector3D<float>.One;

    private ModelType Type { get; } = type;

    public int GetSubModelCount()
    {
        return  ModelData.GetSubModels(Type).Length;
    }
    
    public void Render(ArraySegment<GameObject> gameObjects, Matrix4X4<float> ViewPerspectiveMatrix)
    {
        SubModel[] subModels = ModelData.GetSubModels(Type);
        
        // TODO handle multiple submeshes
        Debug.Assert(subModels.Length == 1);

        Matrix4X4<float> modelTransform = Matrix4X4.CreateTranslation(Position);
        modelTransform = modelTransform * Matrix4X4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
        modelTransform = modelTransform * Matrix4X4.CreateScale(Scale);

        foreach (var subModel in subModels)
        {
            Matrix4X4<float> transformation = subModel.Transform * modelTransform *  ViewPerspectiveMatrix;
            Vector3D<float>[] meshVertices = Meshes.GetMeshVertices(subModel.Type);
            
            Vector[] polygonVertices = new Vector[meshVertices.Length];
            for (uint i = 0; i < meshVertices.Length; i++)
            {
                Vector4D<float> vertex = new Vector4D<float>(meshVertices[i], 1.0f);
                vertex *= transformation;

                // Fixes clipping issues when object is behind camera
                if (vertex.Z <= 0.0f)
                {
                    const float epsilon = 1e-5f;
                    vertex.W = epsilon;
                }
            
                polygonVertices[i] = new Vector(vertex.X / vertex.W, vertex.Y / vertex.W);
            }
            
            Polygon shape = new Polygon(new ShapeCache(polygonVertices, Meshes.GetMeshIndices(subModel.Type)));
            gameObject.Shape = shape;
            gameObject.Color = subModel.Color;
        }
    }
}


internal class SubModel
{
    public readonly MeshType Type;
    public readonly Color Color;
    public readonly Matrix4X4<float> Transform = Matrix4X4<float>.Identity;

    public SubModel(MeshType type, Color color)
    {
        Type = type;
        Color = color;
    }
    
    
    public SubModel(MeshType type, Color color, Matrix4X4<float> transform)
    {
        Type = type;
        Color = color;
        Transform = transform;
    }
}


internal static class ModelData
{
    private static readonly Dictionary<ModelType, SubModel[]> SubModelsMap = new Dictionary<ModelType, SubModel[]>
    {
        { ModelType.WhiteCube, [
                new SubModel(MeshType.Cube, Color.White)
            ] },
        { ModelType.BlueCube, [
                new SubModel(MeshType.Cube, Color.Blue, Matrix4X4.CreateFromYawPitchRoll(25.0f, 45.0f, 90.0f))
            ] },
    };

    
    public static SubModel[] GetSubModels(ModelType type)
    {
        if (type >= ModelType.Invalid)
        {
            return [];
        }

        return SubModelsMap[type];
    }
}