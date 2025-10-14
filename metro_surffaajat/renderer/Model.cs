using System;
using System.Collections.Generic;
using Jypeli;
using Silk.NET.Maths;
using Metro_Surffaajat.utility;

namespace Metro_Surffaajat.renderer;

/// <summary>
/// Currently implemented models.
/// Invalid represents mesh with no SubModels. 
/// </summary>
public enum ModelType
{
    WhiteCube,
    BlueCube,
    DualCube,
    Invalid,
}


/// @author Tuisku Tynkkynen
/// @version 14.10.2025
/// <summary>
/// Structure for storing Models with 3D transformations and ModelType.
/// Does not own any SubModels or mesh data.
/// </summary>
public class Model(ModelType type)
{
    /// <summary>
    /// The 3D position of the Model.
    /// </summary>
    public Vector3D<float> Position = Vector3D<float>.Zero;
    /// <summary>
    /// The 3D rotation of the Model.
    /// </summary>
    public Rotation<float> Rotation = Rotation<float>.Zero;
    /// <summary>
    /// The 3D scale of the Model.
    /// </summary>
    public Vector3D<float> Scale = Vector3D<float>.One;

    /// <summary>
    /// Type of Model. Used for querying SubModels.
    /// </summary>
    private ModelType Type { get; } = type;
    
    
    /// <summary>
    /// Getter for SubModel count.
    /// </summary>
    /// <returns>The number of SubModels the Model has</returns>
    public int GetSubModelCount()
    {
        return  ModelData.GetSubModels(Type).Length;
    }
    
    
    /// <summary>
    /// Transforms the Model's meshes by the transform and the view protection matrix of the
    /// provided camera and updates the provided GameObjects to represent the Model.
    /// The Model's meshes are sorted from the farthest from the camera to nearest.
    /// </summary>
    /// <param name="buffer">RenderBuffer the Model is rendered to</param>
    /// <param name="camera">Camera used to get the view perspective matrix and for depth sorting</param>
    public void Render(RenderBuffer buffer, ref readonly Camera3D camera)
    {
        SubModel[] subModels = ModelData.GetSubModels(Type);
        Tuple<float, int>[] indices = new Tuple<float, int>[subModels.Length];

        for (int i = 0; i < subModels.Length; i++)
        {
            // Squared distance in cheaper to compute
            float distanceSquared = Vector3D.DistanceSquared(Position + subModels[i].Position, camera.Position);
            indices[i] = new Tuple<float, int>(distanceSquared, i);
        }
        
        Array.Sort(indices, (a, b) => Comparer<float>.Default.Compare(b.Item1, a.Item1));

        Matrix4X4<float> mvp = MatrixMath.CreateTransform<float>(Position, Rotation, Scale) * camera.ViewPerspectiveMatrix;

        foreach (var (_, index) in indices)
        {
            ref readonly SubModel subModel = ref subModels[index];
            ref readonly GameObject gameObject = ref buffer.GetNext();
            
            gameObject.Shape = subModel.ToPolygon(mvp);
            gameObject.Color = subModel.Color;
            gameObject.Size = Vector.One;
        }
    }
}


/// @author Tuisku Tynkkynen
/// @version 01.10.2025
/// <summary>
/// Structure for storing SubModel transform data and mesh type.
/// Does not own any mesh data.
/// </summary>
internal class SubModel
{
    /// <summary>
    /// Type of mesh. Used for querying mesh data. 
    /// </summary>
    public readonly MeshType Type;
    /// <summary>
    /// Color of SubModel.
    /// </summary>
    public readonly Color Color;
    /// <summary>
    /// The 3D transform of the SubModel.
    /// </summary>
    public readonly Matrix4X4<float> Transform = Matrix4X4<float>.Identity;
    /// <summary>
    /// The position of SubModel. Used for depth sorting.
    /// </summary>
    public readonly Vector3D<float> Position;

    
    /// <summary>
    /// Constructor for SubModel without 3D transform.
    /// </summary>
    /// <param name="type">Type of mesh used</param>
    /// <param name="color">Color of the SubModel</param>
    public SubModel(MeshType type, Color color)
    {
        Type = type;
        Color = color;
        Position = Vector3D<float>.Zero;
    }
    
    
    /// <summary>
    /// Constructor for SubModel with 3D transform.
    /// </summary>
    /// <param name="type">Type of mesh used</param>
    /// <param name="color">Color of the SubModel</param>
    /// <param name="position">3D position of the submodel</param>
    /// <param name="rotation">3D rotation of the submodel</param>
    /// <param name="scale">3D scale of the submodel</param>
    public SubModel(MeshType type, Color color, Vector3D<float>? position = null, Rotation<float>? rotation = null, Vector3D<float>? scale = null)
    {
        Type = type;
        Color = color;
        Position = position ?? Vector3D<float>.Zero;

        Transform = MatrixMath.CreateTransform(position, rotation, scale);
    }
    

    /// <summary>
    /// Transforms SubModel mesh by its transform and the provided matrix and creates a polygon from the result.
    /// Does not mutate the SubModel.
    /// </summary>
    /// <param name="modelViewPerspectiveMatrix">External transform matrix</param>
    /// <returns>Polygon with transformed vertices</returns>
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


/// @author Tuisku Tynkkynen
/// @version 01.10.2025
/// <summary>
/// Owns model data and provides getters for querying data by ModelType. 
/// </summary>
internal static class ModelData
{   
    /// <summary>
    /// Map from ModelType to SubModel.
    /// </summary>
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
            new SubModel(MeshType.Cube, Color.Blue, rotation: Rotation<float>.FromDegrees(25.0f, 45.0f, 90.0f)),
        ] },
    };

    /// <summary>
    /// Getter for SubModels of model.
    /// </summary>
    /// <param name="type">Type of model</param>
    /// <returns>Array of SubModels or an empty array for invalid model type</returns>
    public static SubModel[] GetSubModels(ModelType type)
    {
        if (type >= ModelType.Invalid)
        {
            return [];
        }

        return SubModelsMap[type];
    }
}