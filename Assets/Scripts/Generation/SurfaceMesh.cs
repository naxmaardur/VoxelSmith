using Godot;

public partial class SurfaceMesh : MeshInstance3D
{
    [Export] private Material material;

    private CollisionShape3D collisionShape;
    private IMeshGenerator<VoxelColor> meshGenerator;

    public void Setup()
    {
        collisionShape = GetParent().GetChildByType<CollisionShape3D>();
        meshGenerator = new BasicMeshGenerator<VoxelColor>(material);
    }

    public void UpdateMesh()
    {
        Mesh = meshGenerator.CreateColorMesh(GameManager.DataManager.ProjectData.voxelColors, GameManager.DataManager.PaletteData.paletteColors);
        collisionShape.Shape = Mesh.CreateTrimeshShape();
    }
}
