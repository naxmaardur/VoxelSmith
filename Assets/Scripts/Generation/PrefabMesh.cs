﻿using Godot;

public partial class PrefabMesh : MeshInstance3D
{
    [Export] private Material material;

    private CollisionShape3D collisionShape;
    private MeshGenerator<VoxelPrefab> meshGenerator;

    public void Setup()
    {
        collisionShape = GetParent().GetChildByType<CollisionShape3D>();
        meshGenerator = new MeshGenerator<VoxelPrefab>(material);
    }

    public void UpdateMesh()
    {
        Mesh = meshGenerator.CreateColorMesh(GameManager.DataManager.ProjectData.voxelPrefabs, GameManager.DataManager.PaletteData.palletePrefabs);
        collisionShape.Shape = Mesh.CreateTrimeshShape();
    }
}

