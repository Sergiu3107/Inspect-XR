using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GLTFast;
using GLTFast.Materials;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UploadService : MonoBehaviour
{
    public static UploadService Instance { get; private set; }

    [HideInInspector] public UnityEvent<GameObject> onModelLoaded = new UnityEvent<GameObject>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LoadModel(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        if (extension == ".zip")
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            try
            {
                ZipFile.ExtractToDirectory(filePath, tempDirectory);

                var supportedFiles = Directory.GetFiles(tempDirectory, "*.*", SearchOption.AllDirectories)
                    .Where(f => new[] { ".obj", ".gltf", ".glb" }.Contains(Path.GetExtension(f).ToLower()))
                    .ToList();

                if (supportedFiles.Count == 0)
                {
                    return;
                }

                string modelFile = supportedFiles[0];
                LoadModel(modelFile);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to extract or process ZIP: {ex.Message}");
            }
        }
        else
        {
            switch (extension)
            {
                case ".gltf":
                case ".glb":
                    ImportGltf(filePath);
                    break;
                default:
                    Debug.LogError($"Unsupported file extension: {extension}");
                    break;
            }
        }
    }

    private async void ImportGltf(string filePath)
    {
        Debug.Log($"Starting glTF load: {filePath}");

        var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        var urpMaterialGenerator = new UniversalRPMaterialGenerator(urpAsset);

        var gltf = new GltfImport(null, null, urpMaterialGenerator);

        var settings = new ImportSettings
        {
            GenerateMipMaps = true,
            AnisotropicFilterLevel = 3,
            NodeNameMethod = NameImportMethod.OriginalUnique
        };

        bool success = await gltf.Load(filePath, settings);

        if (success)
        {
            var rootObject = new GameObject("Model");

            await gltf.InstantiateMainSceneAsync(rootObject.transform);

            var meshRenderer = rootObject.GetComponentsInChildren<MeshRenderer>(true).FirstOrDefault();

            if (meshRenderer != null)
            {
                var meshObject = meshRenderer.gameObject;

                meshObject.transform.SetParent(rootObject.transform, worldPositionStays: true);

                foreach (Transform child in rootObject.transform)
                {
                    if (child.gameObject != meshObject)
                    {
                        Destroy(child.gameObject);
                    }
                }

                meshObject.transform.localPosition = Vector3.zero;
                meshObject.transform.localRotation = Quaternion.identity;
                
                NormalizeModel(meshObject);
                ScaleModel(rootObject, 10f);


                meshObject.tag = "Model";
                meshObject.layer = LayerMask.NameToLayer("Model");

                AddAdditionalMaterials(meshObject);
                AddMesh(meshObject);

                onModelLoaded.Invoke(meshObject);
            }
            else
            {
                Debug.LogWarning("No mesh found in loaded model.");
            }
        }
        else
        {
            Debug.LogError("Failed to load GLTF file.");
        }
    }

    private void NormalizeModel(GameObject model)
    {
        var renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);

        float maxDim = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (maxDim > 0f)
            model.transform.localScale = model.transform.localScale / maxDim;
    }

    private void ScaleModel(GameObject model, float targetSize)
    {
        model.transform.localScale = Vector3.one * targetSize;
    }


    private void AddAdditionalMaterials(GameObject gameObject)
    {
        Debug.Log("Adding additional materials to renderer.");
        var renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.Log("Adding additional materials to renderer is null");
            return;
        }

        var oldMaterials = renderer.materials;
        Material[] newMaterials = new Material[oldMaterials.Length + 2];

        for (int i = 0; i < oldMaterials.Length; i++)
        {
            string shaderName = oldMaterials[i].shader != null ? oldMaterials[i].shader.name : "None";
            newMaterials[i] = oldMaterials[i];
        }

        Shader layerShader = Shader.Find("Unlit/LayerShader");
        Shader paintShader = Shader.Find("Unlit/PaintShader");

        if (layerShader == null || paintShader == null)
        {
            return;
        }

        newMaterials[newMaterials.Length - 2] = new Material(layerShader);
        newMaterials[newMaterials.Length - 1] = new Material(paintShader);
        // newMaterials[newMaterials.Length - 3] = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        // 0 - Base Material
        // 1 - Layer Material
        // 2 - Paint Material

        ApplyAlbedo(newMaterials[0], newMaterials[newMaterials.Length - 2]);

        renderer.materials = newMaterials;

        for (int i = 0; i < renderer.materials.Length; i++)
        {
            Material mat = renderer.materials[i];
            string shaderName = mat.shader != null ? mat.shader.name : "None";
            Debug.Log($"Material {i}: Name = {mat.name}, Shader = {shaderName}");
        }
    }


    private void ApplyAlbedo(Material from, Material to)
    {
        // DebugToFile.Log("Applying albedo from base material to layer material.");

        if (from.HasProperty("_BaseMap"))
        {
            Texture mainTex = from.GetTexture("_BaseMap");
            to.SetTexture("_MainTex", mainTex);
        }
        else if (from.HasProperty("baseColorTexture"))
        {
            Texture mainTex = from.GetTexture("baseColorTexture");
            to.SetTexture("_MainTex", mainTex);
        }
        else
        {
            // DebugToFile.LogWarning("No suitable albedo texture found.");
        }
    }

    private void AddMesh(GameObject gameObject)
    {
        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
    }

    private void SaveModel(string filePath)
    {
        string assetsPath = Path.Combine(Application.dataPath, "Resources", Path.GetFileName(filePath));
        File.Copy(filePath, assetsPath, true);
        // DebugToFile.Log("Model file saved to Assets/Resources.");
    }
}