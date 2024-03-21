﻿using Godot;
using System;
using System.IO;
using System.IO.Hashing;
using System.Text;

public class ExportManager : Manager
{
    public override void OnFixedUpdate()
    {
        if (Input.IsActionJustPressed("debug"))
        {
            //SaveMeshToFiles(GameManager.SurfaceMesh.Mesh, "C:\\Users\\Ben\\Downloads\\", "test");
            //string meshToHashString = "Type:Mesh->test_prefab_mesh0";
            //GD.Print(BitConverter.ToInt64(XxHash64.Hash(new UTF8Encoding(true).GetBytes(meshToHashString))));
        }
    }

    public void SaveMeshToFiles(Mesh mesh, string filePath, string objectName)
    {
        ArrayMesh arrayMesh = new();
        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, mesh.SurfaceGetArrays(0));

        MeshDataTool tool = new();
        tool.CreateFromSurface(arrayMesh, 0);

        StandardMaterial3D blankMaterial = new();
        blankMaterial.ResourceName = "BlankMaterial";

        StringBuilder output = new();
        StringBuilder matOutput = new();

        output.AppendLine("# Mesh Exported Using VoxelSmith");
        output.AppendLine("# https://github.com/brosenmoller/VoxelSmith");

        matOutput.AppendLine("# Material Exported Using VoxelSmith");
        matOutput.AppendLine("# https://github.com/brosenmoller/VoxelSmith");

        output.AppendLine("mtllib " + objectName + ".mtl");
        output.AppendLine("o " + objectName);

        output.AppendLine("vn -1 0 0");  // 1
        output.AppendLine("vn 1 0 0");   // 2
        output.AppendLine("vn 0 0 -1");  // 3
        output.AppendLine("vn 0 0 1");   // 4
        output.AppendLine("vn 0 -1 0");  // 5
        output.AppendLine("vn 0 1 0");   // 6

        StandardMaterial3D mat = (StandardMaterial3D)mesh.SurfaceGetMaterial(0);
        mat ??= blankMaterial;

        for (int i = 0; i < tool.GetVertexCount(); i++)
        {
            output.AppendLine($"v {tool.GetVertex(i).X} {tool.GetVertex(i).Y} {tool.GetVertex(i).Z} {tool.GetVertexColor(i).R} {tool.GetVertexColor(i).G} {tool.GetVertexColor(i).B}");
        }

        output.AppendLine($"usemtl defaultMat");

        for (int i = 0;i < tool.GetFaceCount(); i++)
        {
            Vector3I normal = (Vector3I)tool.GetFaceNormal(i);
            int normalIndex = 0;
            if (normal == Vector3.Left) { normalIndex = 1; }
            if (normal == Vector3.Right) { normalIndex = 2; }
            if (normal == Vector3.Back) { normalIndex = 3; }
            if (normal == Vector3.Forward) { normalIndex = 4; }
            if (normal == Vector3.Down) { normalIndex = 5; }
            if (normal == Vector3.Up) { normalIndex = 6; }

            output.AppendLine($"f {tool.GetFaceVertex(i, 2) + 1}//{normalIndex} {tool.GetFaceVertex(i, 1) + 1}//{normalIndex} {tool.GetFaceVertex(i, 0) + 1}//{normalIndex}");
        }

        //matOutput.AppendLine($"newmtl {mat.ToString()}");
        //matOutput.AppendLine($"Kd {mat.AlbedoColor.R.ToString()} {mat.AlbedoColor.G.ToString()} {mat.AlbedoColor.B.ToString()}");
        //matOutput.AppendLine($"Ke {mat.Emission.R.ToString()} {mat.Emission.G.ToString()} {mat.Emission.B.ToString()}");
        //matOutput.AppendLine($"d {mat.AlbedoColor.A.ToString()}");

        matOutput.AppendLine($"newmtl defaultMat");
        matOutput.AppendLine($"Ka 0.756 0.9 0.9");
        matOutput.AppendLine($"Ks 0.000 0.000 0.000");
        matOutput.AppendLine($"Kd 0.756 0.9 0.9");
        matOutput.AppendLine($"Ke 0.0 0.0 0.0");
        matOutput.AppendLine($"d 0.7");

        if (!filePath.EndsWith("/"))
        {
            filePath += "/";
        }

        WriteToFile(output.ToString(), filePath + objectName + ".obj");
        WriteToFile(matOutput.ToString(), filePath + objectName + ".mtl");
    }

    public void ExportUnityPrefab(string filePath, string objectName)
    {
        if (!filePath.EndsWith("/"))
        {
            filePath += "/";
        }

        string name = objectName[..objectName.IndexOf(".")];

        // Craete New Guid in Untiy style without hyphens
        string meshGuid = Guid.NewGuid().ToString("N");

        // This is the fileID for a mesh with a "default" group name, I couldn't reverse engineer the hashing function so I will just use an existing has of that mesh name
        string meshFileId = "-2432090755550338912";
        // This is the fileID for a material named "defaultMat" (same reasons)
        string materialFileId = "-3033667219593020291";

        string prefabFileTemplate = $"%YAML 1.1\r\n%TAG !u! tag:unity3d.com,2011:\r\n--- !u!1 &5521469611313932757\r\nGameObject:\r\n  m_ObjectHideFlags: 0\r\n  m_CorrespondingSourceObject: {{fileID: 0}}\r\n  m_PrefabInstance: {{fileID: 0}}\r\n  m_PrefabAsset: {{fileID: 0}}\r\n  serializedVersion: 6\r\n  m_Component:\r\n  - component: {{fileID: 442841242954812055}}\r\n  - component: {{fileID: 8052734518851385037}}\r\n  - component: {{fileID: 2192841846447180066}}\r\n  - component: {{fileID: 7909769035001493932}}\r\n  m_Layer: 0\r\n  m_Name: {name}\r\n  m_TagString: Untagged\r\n  m_Icon: {{fileID: 0}}\r\n  m_NavMeshLayer: 0\r\n  m_StaticEditorFlags: 0\r\n  m_IsActive: 1\r\n--- !u!4 &442841242954812055\r\nTransform:\r\n  m_ObjectHideFlags: 0\r\n  m_CorrespondingSourceObject: {{fileID: 0}}\r\n  m_PrefabInstance: {{fileID: 0}}\r\n  m_PrefabAsset: {{fileID: 0}}\r\n  m_GameObject: {{fileID: 5521469611313932757}}\r\n  serializedVersion: 2\r\n  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}\r\n  m_LocalPosition: {{x: 4.57, y: 2.1, z: -6.3}}\r\n  m_LocalScale: {{x: 1, y: 1, z: 1}}\r\n  m_ConstrainProportionsScale: 0\r\n  m_Children:\r\n  m_Father: {{fileID: 0}}\r\n  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}\r\n--- !u!33 &8052734518851385037\r\nMeshFilter:\r\n  m_ObjectHideFlags: 0\r\n  m_CorrespondingSourceObject: {{fileID: 0}}\r\n  m_PrefabInstance: {{fileID: 0}}\r\n  m_PrefabAsset: {{fileID: 0}}\r\n  m_GameObject: {{fileID: 5521469611313932757}}\r\n  m_Mesh: {{fileID: {meshFileId}, guid: {meshGuid}, type: 3}}\r\n--- !u!23 &2192841846447180066\r\nMeshRenderer:\r\n  m_ObjectHideFlags: 0\r\n  m_CorrespondingSourceObject: {{fileID: 0}}\r\n  m_PrefabInstance: {{fileID: 0}}\r\n  m_PrefabAsset: {{fileID: 0}}\r\n  m_GameObject: {{fileID: 5521469611313932757}}\r\n  m_Enabled: 1\r\n  m_CastShadows: 1\r\n  m_ReceiveShadows: 1\r\n  m_DynamicOccludee: 1\r\n  m_StaticShadowCaster: 0\r\n  m_MotionVectors: 1\r\n  m_LightProbeUsage: 1\r\n  m_ReflectionProbeUsage: 1\r\n  m_RayTracingMode: 2\r\n  m_RayTraceProcedural: 0\r\n  m_RenderingLayerMask: 1\r\n  m_RendererPriority: 0\r\n  m_Materials:\r\n  - {{fileID: {materialFileId}, guid: {meshGuid}, type: 2}}\r\n  m_StaticBatchInfo:\r\n    firstSubMesh: 0\r\n    subMeshCount: 0\r\n  m_StaticBatchRoot: {{fileID: 0}}\r\n  m_ProbeAnchor: {{fileID: 0}}\r\n  m_LightProbeVolumeOverride: {{fileID: 0}}\r\n  m_ScaleInLightmap: 1\r\n  m_ReceiveGI: 1\r\n  m_PreserveUVs: 0\r\n  m_IgnoreNormalsForChartDetection: 0\r\n  m_ImportantGI: 0\r\n  m_StitchLightmapSeams: 1\r\n  m_SelectedEditorRenderState: 3\r\n  m_MinimumChartSize: 4\r\n  m_AutoUVMaxDistance: 0.5\r\n  m_AutoUVMaxAngle: 89\r\n  m_LightmapParameters: {{fileID: 0}}\r\n  m_SortingLayerID: 0\r\n  m_SortingLayer: 0\r\n  m_SortingOrder: 0\r\n  m_AdditionalVertexStreams: {{fileID: 0}}\r\n--- !u!64 &7909769035001493932\r\nMeshCollider:\r\n  m_ObjectHideFlags: 0\r\n  m_CorrespondingSourceObject: {{fileID: 0}}\r\n  m_PrefabInstance: {{fileID: 0}}\r\n  m_PrefabAsset: {{fileID: 0}}\r\n  m_GameObject: {{fileID: 5521469611313932757}}\r\n  m_Material: {{fileID: 0}}\r\n  m_IncludeLayers:\r\n    serializedVersion: 2\r\n    m_Bits: 0\r\n  m_ExcludeLayers:\r\n    serializedVersion: 2\r\n    m_Bits: 0\r\n  m_LayerOverridePriority: 0\r\n  m_IsTrigger: 0\r\n  m_ProvidesContacts: 0\r\n  m_Enabled: 1\r\n  serializedVersion: 5\r\n  m_Convex: 0\r\n  m_CookingOptions: 30\r\n  m_Mesh: {{fileID: {meshFileId}, guid: {meshGuid}, type: 3}}\r\n";
        string meshMetaFileTemplate = $"fileFormatVersion: 2\r\nguid: {meshGuid}\r\nModelImporter:\r\n  serializedVersion: 22200\r\n  internalIDToNameTable: []\r\n  externalObjects: {{}}\r\n  materials:\r\n    materialImportMode: 2\r\n    materialName: 0\r\n    materialSearch: 1\r\n    materialLocation: 1\r\n  animations:\r\n    legacyGenerateAnimations: 4\r\n    bakeSimulation: 0\r\n    resampleCurves: 1\r\n    optimizeGameObjects: 0\r\n    removeConstantScaleCurves: 0\r\n    motionNodeName: \r\n    rigImportErrors: \r\n    rigImportWarnings: \r\n    animationImportErrors: \r\n    animationImportWarnings: \r\n    animationRetargetingWarnings: \r\n    animationDoRetargetingWarnings: 0\r\n    importAnimatedCustomProperties: 0\r\n    importConstraints: 0\r\n    animationCompression: 1\r\n    animationRotationError: 0.5\r\n    animationPositionError: 0.5\r\n    animationScaleError: 0.5\r\n    animationWrapMode: 0\r\n    extraExposedTransformPaths: []\r\n    extraUserProperties: []\r\n    clipAnimations: []\r\n    isReadable: 0\r\n  meshes:\r\n    lODScreenPercentages: []\r\n    globalScale: 1\r\n    meshCompression: 0\r\n    addColliders: 0\r\n    useSRGBMaterialColor: 1\r\n    sortHierarchyByName: 1\r\n    importPhysicalCameras: 1\r\n    importVisibility: 1\r\n    importBlendShapes: 1\r\n    importCameras: 1\r\n    importLights: 1\r\n    nodeNameCollisionStrategy: 1\r\n    fileIdsGeneration: 2\r\n    swapUVChannels: 0\r\n    generateSecondaryUV: 0\r\n    useFileUnits: 1\r\n    keepQuads: 0\r\n    weldVertices: 1\r\n    bakeAxisConversion: 0\r\n    preserveHierarchy: 0\r\n    skinWeightsMode: 0\r\n    maxBonesPerVertex: 4\r\n    minBoneWeight: 0.001\r\n    optimizeBones: 1\r\n    meshOptimizationFlags: -1\r\n    indexFormat: 0\r\n    secondaryUVAngleDistortion: 8\r\n    secondaryUVAreaDistortion: 15.000001\r\n    secondaryUVHardAngle: 88\r\n    secondaryUVMarginMethod: 1\r\n    secondaryUVMinLightmapResolution: 40\r\n    secondaryUVMinObjectScale: 1\r\n    secondaryUVPackMargin: 4\r\n    useFileScale: 1\r\n    strictVertexDataChecks: 0\r\n  tangentSpace:\r\n    normalSmoothAngle: 60\r\n    normalImportMode: 0\r\n    tangentImportMode: 3\r\n    normalCalculationMode: 4\r\n    legacyComputeAllNormalsFromSmoothingGroupsWhenMeshHasBlendShapes: 0\r\n    blendShapeNormalImportMode: 1\r\n    normalSmoothingSource: 0\r\n  referencedClips: []\r\n  importAnimation: 1\r\n  humanDescription:\r\n    serializedVersion: 3\r\n    human: []\r\n    skeleton: []\r\n    armTwist: 0.5\r\n    foreArmTwist: 0.5\r\n    upperLegTwist: 0.5\r\n    legTwist: 0.5\r\n    armStretch: 0.05\r\n    legStretch: 0.05\r\n    feetSpacing: 0\r\n    globalScale: 1\r\n    rootMotionBoneName: \r\n    hasTranslationDoF: 0\r\n    hasExtraRoot: 0\r\n    skeletonHasParents: 1\r\n  lastHumanDescriptionAvatarSource: {{instanceID: 0}}\r\n  autoGenerateAvatarMappingIfUnspecified: 1\r\n  animationType: 2\r\n  humanoidOversampling: 1\r\n  avatarSetup: 0\r\n  addHumanoidExtraRootOnlyWhenUsingAvatar: 1\r\n  importBlendShapeDeformPercent: 1\r\n  remapMaterialsIfMaterialImportModeIsNone: 0\r\n  additionalBone: 0\r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n";

        SaveMeshToFiles(GameManager.SurfaceMesh.Mesh, filePath, name);
        WriteToFile(prefabFileTemplate, filePath + name + ".prefab");
        WriteToFile(meshMetaFileTemplate, filePath + name + ".obj.meta");
    }

    private string GetPrefilledPrefabInstanceYAML(RandomNumberGenerator random, string prefabGuid)
    {
        string prefabFileID = "";
        for (int i = 0; i < 19; i++)
        {
            prefabFileID += random.RandiRange(0, 9).ToString();
        }
        
        return $"---!u!1001 & {prefabFileID}\r\nPrefabInstance:\r\n m_ObjectHideFlags: 0\r\n serializedVersion: 2\r\n m_Modification:\r\n serializedVersion: 3\r\n m_TransformParent: {{fileID: 442841242954812055}}\r\n m_Modifications:\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalPosition.x\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalPosition.y\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalPosition.z\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalRotation.w\r\n value: 1\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalRotation.x\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalRotation.y\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalRotation.z\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalEulerAnglesHint.x\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalEulerAnglesHint.y\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_LocalEulerAnglesHint.z\r\n value: 0\r\n objectReference: {{fileID: 0}}\r\n - target: {{fileID: 7821911347754877593, guid: {prefabGuid},\r\n type: 3}}\r\n propertyPath: m_Name\r\n value: TestPrefabSphere\r\n objectReference: {{fileID: 0}}\r\n m_RemovedComponents: []\r\n m_RemovedGameObjects: []\r\n m_AddedGameObjects: []\r\n m_AddedComponents: []\r\n m_SourcePrefab: {{fileID: 100100000, guid: {prefabGuid}, type: 3}}\r\n-- - !u!4 & 3581240652115786994 stripped\r\nTransform:\r\n m_CorrespondingSourceObject: {{fileID: 726921523353226827, guid: {prefabGuid},\r\n type: 3}}\r\n m_PrefabInstance: {{fileID: {prefabFileID}}}\r\n m_PrefabAsset: {{fileID: 0}}\r\n";
    }

    private void WriteToFile(string output, string filePath)
    {
        File.WriteAllText(filePath, string.Empty);
        using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(output);
            fileStream.Write(info, 0, info.Length);
        }
    }
}