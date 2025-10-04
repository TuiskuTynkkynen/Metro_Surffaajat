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
        Polygon[] shapes = new Polygon[subModels.Length];
        Tuple<float, int>[] indices = new Tuple<float, int>[subModels.Length];
        
        Debug.Assert(subModels.Length == gameObjects.Count);

        Matrix4X4<float> modelTransform = Matrix4X4.CreateTranslation(Position);
        modelTransform *= Matrix4X4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
        modelTransform *= Matrix4X4.CreateScale(Scale);

        Matrix4X4<float> mvp = modelTransform * ViewPerspectiveMatrix;
        
        for (int i = 0; i < subModels.Length; i++)
        { 
            float distance = (new Vector4D<float>(subModels[i].Position, 1.0f) * mvp).X;
            indices[i] = new Tuple<float, int>(float.Abs(distance), i);
            
            shapes[i] = subModels[i].ToPolygon(mvp);
        }

        Array.Sort(indices, (a, b) => Comparer<float>.Default.Compare(b.Item1, a.Item1));

        for (int i = 0; i < indices.Length; i++)
        {
            int index = indices[i].Item2;
            
            gameObjects[i].Shape = shapes[index];
            gameObjects[i].Color = subModels[index].Color;
            gameObjects[i].Size = new Vector(100, 100);
        }
    }
}


internal class SubModel
{
    public readonly MeshType Type;
    public readonly Color Color;
    public readonly Matrix4X4<float> Transform = Matrix4X4<float>.Identity;
    public readonly Vector3D<float> Position;

    public SubModel(MeshType type, Color color)
    {
        Type = type;
        Color = color;
        Position = Vector3D<float>.Zero;
    }
    
    
    public SubModel(MeshType type, Color color, Vector3D<float>? position = null, Vector3D<float>? rotation = null, Vector3D<float>? scale = null)
    {
        Type = type;
        Color = color;
        Position = position ?? Vector3D<float>.Zero;
        
        Transform = Matrix4X4.CreateTranslation(Position);
        if (rotation.HasValue)
        {
            Transform *= Matrix4X4.CreateFromYawPitchRoll(rotation.Value.X, rotation.Value.Y, rotation.Value.Z);
        }

        if (scale.HasValue)
        {
            Transform *= Matrix4X4.CreateScale(scale.Value);
        }
    }
    

    public Polygon ToPolygon(Matrix4X4<float> modelViewPerspectiveMatrix)
    {
        Matrix4X4<float> transformation = Transform * modelViewPerspectiveMatrix;
        Vector3D<float>[] meshVertices = Meshes.GetMeshVertices(Type);
            
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

        return new Polygon(new ShapeCache(polygonVertices, Meshes.GetMeshIndices(Type)));
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
                new SubModel(MeshType.Cube, Color.Blue)
            ] },
        { ModelType.DualCube, [
            new SubModel(MeshType.Cube, Color.White, position: new Vector3D<float>(-2.0f, 0.0f, 0.0f)),
            new SubModel(MeshType.Cube, Color.Blue, rotation: new Vector3D<float>(25.0f, 45.0f, 90.0f)),
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