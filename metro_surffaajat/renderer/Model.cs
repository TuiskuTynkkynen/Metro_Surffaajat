using System;
using System.Collections.Generic;
using System.Diagnostics;
using Jypeli;
using Silk.NET.Maths;

namespace Metro_Surffaajat.renderer;

public enum ModelType
{
    WhiteCube,
    BlueCube,
    DualCube,
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
        
        Debug.Assert(subModels.Length == gameObjects.Count);

        Matrix4X4<float> modelTransform = Matrix4X4.CreateTranslation(Position);
        modelTransform = modelTransform * Matrix4X4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
        modelTransform = modelTransform * Matrix4X4.CreateScale(Scale);

        for (int i = 0; i < subModels.Length; i++)
        {
            Matrix4X4<float> transformation = subModels[i].Transform * modelTransform *  ViewPerspectiveMatrix;
            Vector3D<float>[] meshVertices = Meshes.GetMeshVertices(subModels[i].Type);
            
            Vector[] polygonVertices = new Vector[meshVertices.Length];
            for (uint vertexIndex = 0; vertexIndex < meshVertices.Length; vertexIndex++)
            {
                Vector4D<float> vertex = new Vector4D<float>(meshVertices[vertexIndex], 1.0f);
                vertex *= transformation;

                // Fixes clipping issues when object is behind camera
                if (vertex.Z <= 0.0f)
                {
                    const float epsilon = 1e-5f;
                    vertex.W = epsilon;
                }
            
                polygonVertices[vertexIndex] = new Vector(vertex.X / vertex.W, vertex.Y / vertex.W);
            }
            
            Polygon shape = new Polygon(new ShapeCache(polygonVertices, Meshes.GetMeshIndices(subModels[i].Type)));
            gameObjects[i].Shape = shape;
            gameObjects[i].Color = subModels[i].Color;
            gameObjects[i].Size = new Vector(100, 100);
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
        { ModelType.DualCube, [
            new SubModel(MeshType.Cube, Color.White, Matrix4X4.CreateTranslation(-2.0f, 0.0f, 0.0f)),
            new SubModel(MeshType.Cube, Color.Blue, Matrix4X4.CreateFromYawPitchRoll(25.0f, 45.0f, 90.0f)),
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