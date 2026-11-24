

using UnityEngine;

public class NumberColoringHelper
{
    public static void InitObject(GameObject gameObject, out MeshRenderer gridMr, out MeshRenderer grayMr, out Transform numbersContentParent, out Material resMaterial, out Camera mainCamera, out CameraManager cameraManager, out GameObject numberPrefab)
    {
        var loupeBackground = new GameObject("loupeBack");
        loupeBackground.transform.SetParent(gameObject.transform);
        loupeBackground.transform.localScale = new Vector3(20f, 20f, 1f);
        var spriteRenderer = loupeBackground.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = loupeBackground.AddComponent<SpriteRenderer>();
        }
        loupeBackground.layer = LayerMask.NameToLayer("Loupe");
        var gridObject = default(GameObject);
        var meshFilter = default(MeshFilter);
        CreateOrResetObject("grid", out gridObject, out gridMr, out meshFilter);
        gridObject.transform.SetParent(gameObject.transform);
        var mainTexture = (Texture2D)Resources.Load("grid_cell");
        gridMr.sharedMaterial = new Material(Shader.Find("Custom/TilingShader"));
        gridMr.sharedMaterial.mainTexture = mainTexture;
        var grayscaleObject = default(GameObject);
        var grayFilter = default(MeshFilter);
        CreateOrResetObject("gray_texture", out grayscaleObject, out grayMr, out grayFilter);
        grayscaleObject.transform.SetParent(gameObject.transform);
        var grayscaleCollider = grayscaleObject.GetComponent<BoxCollider>();
        if (grayscaleCollider == null)
        {
            grayscaleObject.AddComponent<BoxCollider>();
        }
        grayMr.sharedMaterial = new Material(Shader.Find("Custom/GrayShader"));
        numbersContentParent = gameObject.transform.Find("numbers_content");
        if (numbersContentParent == null)
        {
            numbersContentParent = new GameObject("numbers_content").transform;
            numbersContentParent.SetParent(gameObject.transform);
        }
        numbersContentParent.transform.localPosition = new Vector3(0f, 0f, -0.5f);
        CreateTextPrefab(gameObject.transform, out numberPrefab);
        var resultObject = default(GameObject);
        var meshRenderer = default(MeshRenderer);
        var resFilter = default(MeshFilter);
        CreateOrResetObject("res_texture", out resultObject, out meshRenderer, out resFilter);
        resultObject.transform.SetParent(gameObject.transform);
        resultObject.transform.localPosition = new Vector3(0f, 0f, -1f);
        meshRenderer.sharedMaterial = new Material(Shader.Find("tk2d/BlendVertexColor"));
        resMaterial = meshRenderer.sharedMaterial;
        resultObject.layer = LayerMask.NameToLayer("ResTexture");
        mainCamera = Camera.main;
        cameraManager = Object.FindObjectOfType<CameraManager>();
    }

    public static void CreateHighLightedGrid(GameObject gameObject, out MeshRenderer highlightedGridRenderer)
    {
        var highlightedGrid = default(GameObject);
        var meshFilter = default(MeshFilter);
        CreateOrResetObject("highlighted_grid", out highlightedGrid, out highlightedGridRenderer, out meshFilter);
        highlightedGrid.transform.SetParent(gameObject.transform);
        var value = (Texture2D)Resources.Load("grid_cell");
        highlightedGridRenderer.sharedMaterial = new Material(Shader.Find("My/HighlightedGridShader2"));
        highlightedGridRenderer.sharedMaterial.SetTexture("_ResTex", value);
    }

    static void CreateOrResetObject(string goName, out GameObject go, out MeshRenderer mr, out MeshFilter mf)
    {
        go = GameObject.Find(goName);
        if (go == null)
        {
            go = new GameObject(goName);
            go.transform.position = Vector2.zero;
        }
        mr = go.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = go.AddComponent<MeshRenderer>();
        }
        mf = go.GetComponent<MeshFilter>();
        if (mf == null)
        {
            mf = go.AddComponent<MeshFilter>();
        }
        mf.sharedMesh = new Mesh();
        mf.sharedMesh.vertices = new Vector3[4] {
            new Vector2(-1f, -1f),
            new Vector2(1f, -1f),
            new Vector2(-1f, 1f),
            new Vector2(1f, 1f)
        };
        mf.sharedMesh.uv = new Vector2[4] {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f)
        };
        mf.sharedMesh.triangles = new int[6] {
            0,
            3,
            1,
            2,
            3,
            0
        };
    }

    static void CreateTextPrefab(Transform parent, out GameObject numberPrefab)
    {
        string name = "number_prefab";
        var transform = parent.Find(name);
        if (transform == null)
        {
            transform = new GameObject(name).transform;
            transform.SetParent(parent);
            transform.position = Vector2.zero;
        }
        var textMesh = ((Component)transform).GetComponent<TextMesh>();
        if (textMesh == null)
        {
            textMesh = transform.gameObject.AddComponent<TextMesh>();
        }
        textMesh.color = Color.black;
        textMesh.characterSize = 0.005f;
        textMesh.fontSize = 50;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        numberPrefab = transform.gameObject;
        numberPrefab.SetActive(false);
    }
}


