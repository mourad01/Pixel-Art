using UnityObject = global::UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using VoxDLL;

public static class MVImporter
{
    static Action<Color[]> ColorsPalleteLoadComplete;
    static Action<List<VoxCubeItem>> CubesDataLoadComplete;
    static ColorSettings colorSettings;

    public static List<int> usedPallate = new List<int>();
    public static int[] colorCountArray = new int[256];

    public static Material DefaultMaterial
    {
        get
        {
            return new Material(Shader.Find("Diffuse"));
        }
    }

    public static void AddColorSettings(ColorSettings settings)
    {
        colorSettings = settings;
    }

    public static GameObject CreateGameObject(Transform parent, Vector3 pos, string name, Mesh mesh, Material mat)
    {
        var gameObject = Resources.Load("Prefabs/3D Cube") as GameObject;
        if (gameObject == null)
        {
            Debug.LogError("Prefabs/Cube not found");
        }
        var voxelInstance = UnityObject.Instantiate<GameObject>(gameObject);
        voxelInstance.name = (name);
        voxelInstance.transform.SetParent(parent);
        voxelInstance.transform.localPosition = (pos);
        return voxelInstance;
    }

    public static GameObject[] CreateIndividualVoxelGameObjects(MVMainChunk chunk, Transform parent, Material mat, float sizePerVox)
    {
        return CreateIndividualVoxelGameObjectsForChunk(chunk.voxelChunk, chunk.palatte, parent, mat, sizePerVox);
    }

    public static GameObject[] CreateIndividualVoxelGameObjects(MVMainChunk chunk, Transform parent, Material mat, float sizePerVox, Vector3 origin)
    {
        return CreateIndividualVoxelGameObjectsForChunk(chunk.voxelChunk, chunk.palatte, parent, mat, sizePerVox, origin);
    }

    public static GameObject[] CreateIndividualVoxelGameObjectsForChunk(MVVoxelChunk chunk, Color[] palatte, Transform parent, Material mat, float sizePerVox)
    {
        Vector3 chunkCenter = new Vector3((float)chunk.sizeX / 2f, (float)chunk.sizeY / 2f, (float)chunk.sizeZ / 2f);
        return CreateIndividualVoxelGameObjectsForChunk(chunk, palatte, parent, mat, sizePerVox, chunkCenter);
    }

    public static GameObject[] CreateIndividualVoxelGameObjectsForChunk(MVVoxelChunk chunk, Color[] palatte, Transform parent, Material mat, float sizePerVox, Vector3 origin)
    {
        var gameObjects = new List<GameObject>();
        var voxCubeItems = new List<VoxCubeItem>();

        //first pass, count visible voxels per color and collect the used palette
        for (int i = 0; i < chunk.sizeX; i++)
        {
            for (int j = 0; j < chunk.sizeY; j++)
            {
                for (int k = 0; k < chunk.sizeZ; k++)
                {
                    if (chunk.voxels[i, j, k] != 0 && IsVoxelVisible(chunk, i, j, k))
                    {
                        int colorIndex = chunk.voxels[i, j, k] - 1;
                        colorCountArray[colorIndex]++;
                        if (usedPallate.IndexOf(colorIndex) == -1)
                        {
                            usedPallate.Add(colorIndex);
                        }
                    }
                }
            }
        }

        usedPallate.Sort();
        //second pass, one cube per visible voxel numbered by its place in the used palette
        for (int i = 0; i < chunk.sizeX; i++)
        {
            for (int j = 0; j < chunk.sizeY; j++)
            {
                for (int k = 0; k < chunk.sizeZ; k++)
                {
                    int index = chunk.voxels[i, j, k];
                    if (index != 0 && IsVoxelVisible(chunk, i, j, k))
                    {
                        int colorNum = usedPallate.IndexOf(index - 1);

                        float single = ((float)i - origin.x + 0.5f) * sizePerVox;
                        float single1 = ((float)j - origin.y + 0.5f) * sizePerVox;
                        float single2 = ((float)k - origin.z + 0.5f) * sizePerVox;
                        var mesh = CubeMeshWithColor(sizePerVox, palatte[index], colorNum);
                        var gameObject = CreateGameObject(parent, new Vector3(single, single1, single2), string.Format("Voxel ({0}, {1}, {2})", i, j, k), mesh, mat);
                        var voxCubeItem = gameObject.AddComponent<VoxCubeItem>();
                        voxCubeItem.ColorIndex = colorNum + 1;
                        // a side is open when the neighbour is empty or outside the model
                        var flagArray = new bool[] {
                            (j + 1 >= chunk.sizeY ? true : chunk.voxels[i, j + 1, k] == 0),
                            (j - 1 < 0 ? true : chunk.voxels[i, j - 1, k] == 0),
                            (k - 1 < 0 ? true : chunk.voxels[i, j, k - 1] == 0),
                            (k + 1 >= chunk.sizeZ ? true : chunk.voxels[i, j, k + 1] == 0),
                            (i - 1 < 0 ? true : chunk.voxels[i - 1, j, k] == 0),
                            (i + 1 >= chunk.sizeX ? true : chunk.voxels[i + 1, j, k] == 0) };
                        voxCubeItem.Init(colorSettings, palatte[index-1], mesh, flagArray);
                        gameObject.tag = ("Player");
                        voxCubeItems.Add(voxCubeItem);
                        gameObjects.Add(gameObject);
                    }
                }
            }
        }
        //send only the colors this model really uses
        if (ColorsPalleteLoadComplete != null)
        {
            var colors = new Color[usedPallate.Count];
            for (int i = 0; i < usedPallate.Count; i++)
            {
                colors[i] = palatte[usedPallate[i]];
            }
            ColorsPalleteLoadComplete(colors);
        }
        Debug.Log(string.Concat("Cube Items count = ", voxCubeItems.Count<VoxCubeItem>()));
        if (CubesDataLoadComplete != null)
        {
            CubesDataLoadComplete(voxCubeItems);
        }

        return gameObjects.ToArray();
    }

    static bool IsVoxelVisible(MVVoxelChunk chunk, int px, int py, int pz)
    {
        //border voxels are always visible, inner ones only if a neighbour is empty
        if (px != 0 && px != chunk.sizeX - 1)
        {
            if (py != 0 && py != chunk.sizeY - 1)
            {
                if (pz != 0 && pz != chunk.sizeZ - 1)
                {
                    if (chunk.voxels[px, py + 1, pz] == 0)
                    {
                        return true;
                    }
                    if (chunk.voxels[px, py - 1, pz] == 0)
                    {
                        return true;
                    }
                    if (chunk.voxels[px, py, pz + 1] == 0)
                    {
                        return true;
                    }
                    if (chunk.voxels[px, py, pz - 1] == 0)
                    {
                        return true;
                    }
                    if (chunk.voxels[px + 1, py, pz] == 0)
                    {
                        return true;
                    }
                    if (chunk.voxels[px - 1, py, pz] == 0)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return true;
        }
        return true;
    }

    public static Mesh[] CreateMeshes(MVMainChunk chunk, float sizePerVox)
    {
        return CreateMeshesFromChunk(chunk.voxelChunk, chunk.palatte, sizePerVox);
    }

    public static Mesh[] CreateMeshesFromChunk(MVVoxelChunk chunk, Color[] palatte, float sizePerVox)
    {
        Vector3 chunkCenter = new Vector3((float)chunk.sizeX / 2f, (float)chunk.sizeY / 2f, (float)chunk.sizeZ / 2f);
        return CreateMeshesFromChunk(chunk, palatte, sizePerVox, chunkCenter);
    }

    public static Mesh[] CreateMeshesFromChunk(MVVoxelChunk chunk, Color[] palatte, float sizePerVox, Vector3 origin)
    {
        var vector3s = new List<Vector3>();
        var vector3s1 = new List<Vector3>();
        var colors = new List<Color>();
        var nums = new List<int>();
        var vector2s = new List<Vector2>();
        var _right = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        var meshes = new List<Mesh>();
        if (sizePerVox <= 0f)
        {
            sizePerVox = 0.1f;
        }
        float single = sizePerVox / 2f;
        int quadCount = 0;
        int faceCount = 0;
        //greedy meshing, merge same color faces into bigger quads for each of the 6 directions
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < chunk.sizeX; j++)
            {
                for (int k = 0; k < chunk.sizeY; k++)
                {
                    for (int l = 0; l < chunk.sizeZ; l++)
                    {
                        int colorIndice = chunk.faces[i].colorIndices[j, k, l];
                        if (colorIndice != 0)
                        {
                            float single1 = ((float)j - origin.x + 0.5f) * sizePerVox;
                            float single2 = ((float)k - origin.y + 0.5f) * sizePerVox;
                            float single3 = ((float)l - origin.z + 0.5f) * sizePerVox;
                            int end = j;
                            int p = k;
                            int m = l;
                            // x faces grow along y then z, y faces along x then z, z faces along x then y
                            switch (i)
                            {
                                case 0:
                                case 1:
                                    {
                                        p = k + 1;
                                        while (p < chunk.sizeY && chunk.faces[i].colorIndices[j, p, l] == colorIndice)
                                        {
                                            p++;
                                        }
                                        p--;
                                        for (m = l + 1; m < chunk.sizeZ; m++)
                                        {
                                            bool sameColor = true;
                                            for (int n = k; n <= p; n++)
                                            {
                                                sameColor = sameColor & chunk.faces[i].colorIndices[j, n, m] == colorIndice;
                                            }
                                            if (!sameColor)
                                            {
                                                break;
                                            }
                                        }
                                        m--;
                                        break;
                                    }
                                case 2:
                                case 3:
                                    {
                                        end = j + 1;
                                        while (end < chunk.sizeX && chunk.faces[i].colorIndices[end, k, l] == colorIndice)
                                        {
                                            end++;
                                        }
                                        end--;
                                        for (m = l + 1; m < chunk.sizeZ; m++)
                                        {
                                            bool matchesDepthRow = true;
                                            for (int o = j; o <= end; o++)
                                            {
                                                matchesDepthRow = matchesDepthRow & chunk.faces[i].colorIndices[o, k, m] == colorIndice;
                                            }
                                            if (!matchesDepthRow)
                                            {
                                                break;
                                            }
                                        }
                                        m--;
                                        break;
                                    }
                                case 4:
                                case 5:
                                    {
                                        end = j + 1;
                                        while (end < chunk.sizeX && chunk.faces[i].colorIndices[end, k, l] == colorIndice)
                                        {
                                            end++;
                                        }
                                        end--;
                                        for (p = k + 1; p < chunk.sizeY; p++)
                                        {
                                            bool matchesHeightRow = true;
                                            for (int q = j; q <= end; q++)
                                            {
                                                matchesHeightRow = matchesHeightRow & chunk.faces[i].colorIndices[q, p, l] == colorIndice;
                                            }
                                            if (!matchesHeightRow)
                                            {
                                                break;
                                            }
                                        }
                                        p--;
                                        break;
                                    }
                            }
                            //clear the merged cells so they are not emitted twice
                            for (int r = j; r <= end; r++)
                            {
                                for (int s = k; s <= p; s++)
                                {
                                    for (int t = l; t <= m; t++)
                                    {
                                        if (r != j || s != k || t != l)
                                        {
                                            chunk.faces[i].colorIndices[r, s, t] = 0;
                                        }
                                    }
                                }
                            }
                            int quadWidth = end - j;
                            int quadHeight = p - k;
                            int quadDepth = m - l;
                            switch (i)
                            {
                                case 0:
                                    {
                                        vector3s.Add(new Vector3(single1 + single, single2 - single, single3 - single));
                                        vector3s.Add(new Vector3(single1 + single, single2 + single + sizePerVox * (float)quadHeight, single3 - single));
                                        vector3s.Add(new Vector3(single1 + single, single2 + single + sizePerVox * (float)quadHeight, single3 + single + sizePerVox * (float)quadDepth));
                                        vector3s.Add(new Vector3(single1 + single, single2 - single, single3 + single + sizePerVox * (float)quadDepth));
                                        break;
                                    }
                                case 1:
                                    {
                                        vector3s.Add(new Vector3(single1 - single, single2 - single, single3 - single));
                                        vector3s.Add(new Vector3(single1 - single, single2 - single, single3 + single + sizePerVox * (float)quadDepth));
                                        vector3s.Add(new Vector3(single1 - single, single2 + single + sizePerVox * (float)quadHeight, single3 + single + sizePerVox * (float)quadDepth));
                                        vector3s.Add(new Vector3(single1 - single, single2 + single + sizePerVox * (float)quadHeight, single3 - single));
                                        break;
                                    }
                                case 2:
                                    {
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 + single, single3 - single));
                                        vector3s.Add(new Vector3(single1 - single, single2 + single, single3 - single));
                                        vector3s.Add(new Vector3(single1 - single, single2 + single, single3 + single + sizePerVox * (float)quadDepth));
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 + single, single3 + single + sizePerVox * (float)quadDepth));
                                        break;
                                    }
                                case 3:
                                    {
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 - single, single3 - single));
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 - single, single3 + single + sizePerVox * (float)quadDepth));
                                        vector3s.Add(new Vector3(single1 - single, single2 - single, single3 + single + sizePerVox * (float)quadDepth));
                                        vector3s.Add(new Vector3(single1 - single, single2 - single, single3 - single));
                                        break;
                                    }
                                case 4:
                                    {
                                        vector3s.Add(new Vector3(single1 - single, single2 + single + sizePerVox * (float)quadHeight, single3 + single));
                                        vector3s.Add(new Vector3(single1 - single, single2 - single, single3 + single));
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 - single, single3 + single));
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 + single + sizePerVox * (float)quadHeight, single3 + single));
                                        break;
                                    }
                                case 5:
                                    {
                                        vector3s.Add(new Vector3(single1 - single, single2 + single + sizePerVox * (float)quadHeight, single3 - single));
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 + single + sizePerVox * (float)quadHeight, single3 - single));
                                        vector3s.Add(new Vector3(single1 + single + sizePerVox * (float)quadWidth, single2 - single, single3 - single));
                                        vector3s.Add(new Vector3(single1 - single, single2 - single, single3 - single));
                                        break;
                                    }
                            }
                            vector3s1.Add(_right[i]);
                            vector3s1.Add(_right[i]);
                            vector3s1.Add(_right[i]);
                            vector3s1.Add(_right[i]);
                            Color color = palatte[colorIndice - 1];
                            colors.Add(color);
                            colors.Add(color);
                            colors.Add(color);
                            colors.Add(color);
                            nums.Add(quadCount * 4);
                            nums.Add(quadCount * 4 + 1);
                            nums.Add(quadCount * 4 + 2);
                            nums.Add(quadCount * 4 + 2);
                            nums.Add(quadCount * 4 + 3);
                            nums.Add(quadCount * 4);
                            //uv points at the color column of a 256 wide palette texture
                            vector2s.Add(new Vector2(((float)colorIndice - 1f) / 256f, 0f));
                            vector2s.Add(new Vector2((float)colorIndice / 256f, 0f));
                            vector2s.Add(new Vector2((float)colorIndice / 256f, 1f));
                            vector2s.Add(new Vector2(((float)colorIndice - 1f) / 256f, 1f));
                            quadCount++;
                            //start a new mesh before the 65k vertex limit
                            if (vector3s.Count + 4 >= 65000)
                            {
                                var mesh = new Mesh();
                                mesh.vertices = vector3s.ToArray();
                                mesh.colors = colors.ToArray();
                                mesh.normals = (vector3s1.ToArray());
                                mesh.triangles = (nums.ToArray());
                                mesh.uv = (vector2s.ToArray());
                                meshes.Add(mesh);
                                vector2s.Clear();
                                vector3s.Clear();
                                colors.Clear();
                                vector3s1.Clear();
                                nums.Clear();
                                faceCount += quadCount;
                                quadCount = 0;
                            }
                        }
                    }
                }
            }
        }
        if (vector3s.Count > 0)
        {
            var mesh = new Mesh();
            mesh.vertices = (vector3s.ToArray());
            mesh.colors = colors.ToArray();
            mesh.normals = (vector3s1.ToArray());
            mesh.triangles = (nums.ToArray());
            mesh.uv = (vector2s.ToArray());
            meshes.Add(mesh);
            faceCount += quadCount;
        }
        Debug.Log(string.Format("[MVImport] Mesh generated, total quads {0}", faceCount));
        return meshes.ToArray();
    }

    public static GameObject[] CreateVoxelGameObjects(MVMainChunk chunk, Transform parent, Material mat, float sizePerVox)
    {
        return CreateVoxelGameObjectsForChunk(chunk.voxelChunk, chunk.palatte, parent, mat, sizePerVox);
    }

    public static GameObject[] CreateVoxelGameObjects(MVMainChunk chunk, Transform parent, Material mat, float sizePerVox, Vector3 origin)
    {
        return CreateVoxelGameObjectsForChunk(chunk.voxelChunk, chunk.palatte, parent, mat, sizePerVox, origin);
    }

    public static GameObject[] CreateVoxelGameObjectsForChunk(MVVoxelChunk chunk, Color[] palatte, Transform parent, Material mat, float sizePerVox)
    {
        Vector3 chunkCenter = new Vector3((float)chunk.sizeX / 2f, (float)chunk.sizeY / 2f, (float)chunk.sizeZ / 2f);
        return CreateVoxelGameObjectsForChunk(chunk, palatte, parent, mat, sizePerVox, chunkCenter);
    }

    public static GameObject[] CreateVoxelGameObjectsForChunk(MVVoxelChunk chunk, Color[] palatte, Transform parent, Material mat, float sizePerVox, Vector3 origin)
    {
        var gameObjects = new List<GameObject>();
        int index = 0;
        var meshArray = CreateMeshesFromChunk(chunk, palatte, sizePerVox, origin);
        for (int i = 0; i < (int)meshArray.Length; i++)
        {
            var mesh = meshArray[i];
            var gameObject = new GameObject();
            gameObject.name = (string.Format("VoxelMesh ({0})", index));
            gameObject.transform.SetParent(parent);
            gameObject.transform.localPosition = (Vector3.zero);
            gameObject.transform.localRotation = (Quaternion.Euler(Vector3.zero));
            gameObject.transform.localScale = (Vector3.one);
            gameObject.AddComponent<MeshFilter>().mesh = (mesh);
            gameObject.AddComponent<MeshRenderer>().material = (mat);
            gameObject.AddComponent<MVVoxModelMesh>();
            gameObjects.Add(gameObject);
            index++;
        }
        return gameObjects.ToArray();
    }

    public static Mesh CubeMeshWithColor(float size, Color c, int cidx)
    {
        float half = size / 2f;
        var cubeVertices = new Vector3[] { new Vector3(-half, -half, -half), new Vector3(-half, half, -half), new Vector3(half, half, -half), new Vector3(half, -half, -half), new Vector3(half, -half, half), new Vector3(half, half, half), new Vector3(-half, half, half), new Vector3(-half, -half, half) };
        var vertices = new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 5, 3, 5, 4, 5, 2, 1, 5, 1, 6, 3, 4, 7, 3, 7, 0, 0, 7, 6, 0, 6, 1, 4, 5, 6, 4, 6, 7 };
        var colors = new Color[] { c, c, c, c, c, c, c, c };
        var uvs = new Vector2[] { new Vector2(((float)cidx - 0.5f) / 256f, 0.5f), new Vector2(((float)cidx - 0.5f) / 256f, 0.5f), new Vector2(((float)cidx - 0.5f) / 256f, 0.5f), new Vector2(((float)cidx - 0.5f) / 256f, 0.5f), new Vector2(((float)cidx - 0.5f) / 256f, 0.5f), new Vector2(((float)cidx - 0.5f) / 256f, 0.5f), new Vector2(((float)cidx - 0.5f) / 256f, 0.5f), new Vector2(((float)cidx - 0.5f) / 256f, 0.5f) };
        var mesh = new Mesh();
        mesh.vertices = (cubeVertices);
        mesh.uv = (uvs);
        mesh.colors = colors;
        mesh.triangles = (vertices);
        mesh.RecalculateNormals();
        return mesh;
    }

    public static void GenerateFaces(MVVoxelChunk voxelChunk)
    {
        //face kept only where the neighbour is empty, 0 = +x, 1 = -x, 2 = +y, 3 = -y, 4 = +z, 5 = -z
        voxelChunk.faces = new MVFaceCollection[6];
        for (int i = 0; i < 6; i++)
        {
            voxelChunk.faces[i].colorIndices = new byte[voxelChunk.sizeX, voxelChunk.sizeY, voxelChunk.sizeZ];
        }
        for (int j = 0; j < voxelChunk.sizeX; j++)
        {
            for (int k = 0; k < voxelChunk.sizeY; k++)
            {
                for (int l = 0; l < voxelChunk.sizeZ; l++)
                {
                    if (j == 0 || voxelChunk.voxels[j - 1, k, l] == 0)
                    {
                        voxelChunk.faces[1].colorIndices[j, k, l] = voxelChunk.voxels[j, k, l];
                    }
                    if (j == voxelChunk.sizeX - 1 || voxelChunk.voxels[j + 1, k, l] == 0)
                    {
                        voxelChunk.faces[0].colorIndices[j, k, l] = voxelChunk.voxels[j, k, l];
                    }
                    if (k == 0 || voxelChunk.voxels[j, k - 1, l] == 0)
                    {
                        voxelChunk.faces[3].colorIndices[j, k, l] = voxelChunk.voxels[j, k, l];
                    }
                    if (k == voxelChunk.sizeY - 1 || voxelChunk.voxels[j, k + 1, l] == 0)
                    {
                        voxelChunk.faces[2].colorIndices[j, k, l] = voxelChunk.voxels[j, k, l];
                    }
                    if (l == 0 || voxelChunk.voxels[j, k, l - 1] == 0)
                    {
                        voxelChunk.faces[5].colorIndices[j, k, l] = voxelChunk.voxels[j, k, l];
                    }
                    if (l == voxelChunk.sizeZ - 1 || voxelChunk.voxels[j, k, l + 1] == 0)
                    {
                        voxelChunk.faces[4].colorIndices[j, k, l] = voxelChunk.voxels[j, k, l];
                    }
                }
            }
        }
    }

    public static void Init(Action<Color[]> actionColorsComplete, Action<List<VoxCubeItem>> actionCubesComplete)
    {
        ColorsPalleteLoadComplete = actionColorsComplete;
        CubesDataLoadComplete = actionCubesComplete;
    }

    public static MVMainChunk LoadVOX(string path, bool generateFaces = true)
    {
        byte[] numArray = null;
        numArray = File.ReadAllBytes(path);
        //file must start with "VOX "
        if (numArray[0] == 86 && numArray[1] == 79 && numArray[2] == 88 && numArray[3] == 32)
        {
            return LoadVOXFromData(numArray, generateFaces);
        }
        Debug.LogError("Invalid VOX file, magic number mismatch");
        return null;
    }

    public static MVMainChunk LoadVOXFromData(byte[] data, bool generateFaces = true)
    {
        //reset color stats from the previous model
        usedPallate.Clear();
        for (int i = 0; i < 256; i++)
        {
            colorCountArray[i] = 0;
        }

        MVMainChunk mVMainChunk;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                var _white = new MVMainChunk();
                var dummy = binaryReader.ReadInt32();
                _white.version = binaryReader.ReadBytes(4);
                var numArray = binaryReader.ReadBytes(4);
                // "MAIN"
                if (numArray[0] != 77 || numArray[1] != 65 || numArray[2] != 73 || numArray[3] != 78)
                {
                    Debug.LogError("[MVImport] Invalid MainChunk ID, main chunk expected");
                    mVMainChunk = null;
                }
                else
                {
                    int chunkSize = binaryReader.ReadInt32();
                    int childrenSize = binaryReader.ReadInt32();
                    binaryReader.ReadBytes(chunkSize);
                    int readSize = 0;
                    while (readSize < childrenSize)
                    {
                        numArray = binaryReader.ReadBytes(4);
                        //child chunks: PACK is skipped, SIZE, XYZI and RGBA are read, unknown ids skipped
                        if (numArray[0] == 80 && numArray[1] == 65 && numArray[2] == 67 && numArray[3] == 75)
                        {
                            int contentSize = binaryReader.ReadInt32();
                            int childSize = binaryReader.ReadInt32();
                            binaryReader.ReadInt32();
                            readSize = readSize + contentSize + childSize + 12;
                        }
                        else if (numArray[0] == 83 && numArray[1] == 73 && numArray[2] == 90 && numArray[3] == 69)
                        {
                            readSize += ReadSizeChunk(binaryReader, _white);
                        }
                        else if (numArray[0] == 88 && numArray[1] == 89 && numArray[2] == 90 && numArray[3] == 73)
                        {
                            readSize += ReadVoxelChunk(binaryReader, _white.voxelChunk);
                        }
                        else if (numArray[0] != 82 || numArray[1] != 71 || numArray[2] != 66 || numArray[3] != 65)
                        {
                            Debug.LogWarning(string.Concat("[MVImport] Chunk ID not recognized, got ", Encoding.ASCII.GetString(numArray)));
                            int skipContent = binaryReader.ReadInt32();
                            int skipChildren = binaryReader.ReadInt32();
                            binaryReader.ReadBytes(skipContent + skipChildren);
                            readSize = readSize + skipContent + skipChildren + 12;
                        }
                        else
                        {
                            _white.palatte = new Color[256];
                            for (int i = 0; i < (int)_white.palatte.Length; i++)
                            {
                                _white.palatte[i] = Color.white;
                            }
                            readSize += ReadPalattee(binaryReader, _white.palatte);
                            //if (MVImporter.ColorsPalleteLoadComplete == null)
                            //{
                            //	continue;
                            //}
                            //MVImporter.ColorsPalleteLoadComplete(_white.palatte);
                        }
                    }
                    if (generateFaces)
                    {
                        GenerateFaces(_white.voxelChunk);
                    }
                    if (_white.palatte == null)
                    {
                        _white.palatte = MVMainChunk.defaultPalatte;
                        //if (MVImporter.ColorsPalleteLoadComplete != null)
                        //{
                        //	MVImporter.ColorsPalleteLoadComplete(_white.palatte);
                        //}
                    }
                    mVMainChunk = _white;
                }
            }
        }
        return mVMainChunk;
    }

    static int ReadPalattee(BinaryReader br, Color[] colors)
    {
        int chunkSize = br.ReadInt32();
        int childrenSize = br.ReadInt32();
        for (int i = 0; i < 256; i++)
        {
            colors[i] = new Color((float)br.ReadByte() / 255f, (float)br.ReadByte() / 255f, (float)br.ReadByte() / 255f, (float)br.ReadByte() / 255f);
        }
        if (childrenSize > 0)
        {
            br.ReadBytes(childrenSize);
            Debug.LogWarning("[MVImporter] Nested chunk not supported");
        }
        // +12 for the chunk header (id, content size, children size)
        return chunkSize + childrenSize + 12;
    }

    static int ReadSizeChunk(BinaryReader br, MVMainChunk mainChunk)
    {
        int chunkSize = br.ReadInt32();
        int childrenSize = br.ReadInt32();
        //vox is z up, so z and y are swapped for Unity
        mainChunk.sizeX = br.ReadInt32();
        mainChunk.sizeZ = br.ReadInt32();
        mainChunk.sizeY = br.ReadInt32();
        mainChunk.voxelChunk = new MVVoxelChunk()
        {
            voxels = new byte[mainChunk.sizeX, mainChunk.sizeY, mainChunk.sizeZ]
        };
        Debug.Log(string.Format("[MVImporter] Voxel Size {0}x{1}x{2}", mainChunk.sizeX, mainChunk.sizeY, mainChunk.sizeZ));
        if (childrenSize > 0)
        {
            br.ReadBytes(childrenSize);
            Debug.LogWarning("[MVImporter] Nested chunk not supported");
        }
        return chunkSize + childrenSize + 12;
    }

    static int ReadVoxelChunk(BinaryReader br, MVVoxelChunk chunk)
    {
        int chunkSize = br.ReadInt32();
        int childrenSize = br.ReadInt32();
        int voxelCount = br.ReadInt32();
        for (int i = 0; i < voxelCount; i++)
        {
            int vx = br.ReadByte();
            int vy = br.ReadByte();
            int vz = br.ReadByte();
            chunk.voxels[vx, vz, vy] = br.ReadByte();
        }
        if (childrenSize > 0)
        {
            br.ReadBytes(childrenSize);
            Debug.LogWarning("[MVImporter] Nested chunk not supported");
        }
        return chunkSize + childrenSize + 12;
    }
}
