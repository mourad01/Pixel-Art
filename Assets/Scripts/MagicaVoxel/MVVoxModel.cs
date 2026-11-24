using UnityObject = global::UnityEngine.Object;
using UnityEngine;

public class MVVoxModel : MonoBehaviour
{
    [HideInInspector]
    public bool ed_importAsIndividualVoxels;

    public string ed_filePath = "";

    [HideInInspector]
    public MVMainChunk vox;

    [Range(0.01f, 5f)]
    public float sizePerVox = 1f;

    public Material voxMaterial;
    public Transform meshOrigin;

    [Tooltip("If the vox file contains a palette, should it be converted to a texture?")]
    public bool paletteToTex;

    public bool reimportOnStart = true;

    public MVVoxModel()
    {
    }

    public void ClearVoxMeshes()
    {
        int i;
        var componentsInChildren = gameObject.GetComponentsInChildren<MVVoxModelMesh>();
        for (i = 0; i < (int)componentsInChildren.Length; i++)
        {
            UnityObject.DestroyImmediate(componentsInChildren[i].gameObject);
        }
        var mVVoxModelVoxelArray = gameObject.GetComponentsInChildren<MVVoxModelVoxel>();
        for (i = 0; i < (int)mVVoxModelVoxelArray.Length; i++)
        {
            UnityObject.DestroyImmediate(mVVoxModelVoxelArray[i].gameObject);
        }
    }

    public void LoadVOXData(byte[] data, bool asIndividualVoxels)
    {
        ClearVoxMeshes();
        var mVMainChunk = MVImporter.LoadVOXFromData(data, true);
        if (mVMainChunk != null)
        {
            var material = (voxMaterial != null ? voxMaterial : MVImporter.DefaultMaterial);
            if (paletteToTex)
            {
                material.mainTexture = (mVMainChunk.PaletteToTexture());
            }
            if (asIndividualVoxels)
            {
                MVImporter.CreateIndividualVoxelGameObjects(mVMainChunk, gameObject.transform, material, sizePerVox);
            }
            else
            {
                MVImporter.CreateVoxelGameObjects(mVMainChunk, gameObject.transform, material, sizePerVox);
            }
            vox = mVMainChunk;
        }
    }

    public void LoadVOXFile(string path, bool asIndividualVoxels)
    {
        ClearVoxMeshes();
        if (path == null || path.Length <= 0)
        {
            Debug.LogError("[MVVoxModel] Invalid file path");
        }
        else
        {
            var mVMainChunk = MVImporter.LoadVOX(path, true);
            if (mVMainChunk != null)
            {
                var material = (voxMaterial != null ? voxMaterial : MVImporter.DefaultMaterial);
                if (paletteToTex)
                {
                    material.mainTexture = (mVMainChunk.PaletteToTexture());
                }
                if (!asIndividualVoxels)
                {
                    if (meshOrigin == null)
                    {
                        MVImporter.CreateVoxelGameObjects(mVMainChunk, gameObject.transform, material, sizePerVox);
                    }
                    else
                    {
                        MVImporter.CreateVoxelGameObjects(mVMainChunk, gameObject.transform, material, sizePerVox, meshOrigin.localPosition);
                    }
                }
                else if (meshOrigin == null)
                {
                    MVImporter.CreateIndividualVoxelGameObjects(mVMainChunk, gameObject.transform, material, sizePerVox);
                }
                else
                {
                    MVImporter.CreateIndividualVoxelGameObjects(mVMainChunk, gameObject.transform, material, sizePerVox, meshOrigin.localPosition);
                }
                vox = mVMainChunk;
                return;
            }
        }
    }

    void Start()
    {
        if (reimportOnStart)
        {
            LoadVOXFile(ed_filePath, ed_importAsIndividualVoxels);
        }
    }
}
