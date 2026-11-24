using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class NumberColoring : MonoBehaviour
{
    public static int CELL_SIZE = 50;

    public Color borderColor = Color.green;
    public int borderWidth = 1;
    public Action OnComplete;
    public Action<Color> OnColorComplete;
    public Action<Color, int> OnCountLeft;
    public Action<Color> OnPaintedPixel;

    public int[,] m_bombMatrix = new int[7, 7] {
        {
            0, 0,
            1,
            1,
            1,
            0,
            0
        },
        {
            0,
            1,
            1,
            1,
            1,
            1,
            0
        },
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1
        },
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1
        },
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1
        },
        {
            0,
            1,
            1,
            1,
            1,
            1,
            0
        },
        {
            0,
            0,
            1,
            1,
            1,
            0,
            0
        }
    };

    [SerializeField] Camera m_mainCamera;
    [SerializeField] Camera m_loupeCamera;
    [SerializeField] CameraManager m_cameraManager;
    [SerializeField] MeshRenderer m_grayRenderer;
    [SerializeField] MeshRenderer m_gridRenderer;
    [SerializeField] MeshRenderer m_highlightedGridRenderer;

    [SerializeField] public Transform m_numbersContent;

    [SerializeField] Material m_resMaterial;
    [SerializeField] GameObject m_content;
    [SerializeField] TextMesh m_numbersText;

    [SerializeField] public List<Color> m_colors;

    [HideInInspector]
    public ImageInfo m_imageInfo;

    [HideInInspector]
    public int m_colorsToColor;

    [HideInInspector]
    public Color[] m_pixels;

    [HideInInspector]
    public int m_width = -1;

    [HideInInspector]
    public int m_height = -1;

    [HideInInspector]
    public bool m_inited;

    [HideInInspector]
    public bool m_isEmpty = true;

    [HideInInspector]
    public History m_history;

    public bool Completed { get;  set; }

    Color transparent = new Color(1, 1, 1, 0);
    int m_totalNumbers = 0;
    Dictionary<Color, int> m_colorsCount = new Dictionary<Color, int>();

    [ContextMenu("InitObject")]
    public void InitObject()
    {
        var gameObject = default(GameObject);
        NumberColoringHelper.InitObject(base.gameObject, out m_gridRenderer, out m_grayRenderer, out m_numbersContent, out m_resMaterial, out m_mainCamera, out m_cameraManager, out gameObject);
    }

    [ContextMenu("CreateHighLightedGrid")]
    public void CreateHighLightedGrid()
    {
        NumberColoringHelper.CreateHighLightedGrid(gameObject, out m_highlightedGridRenderer);
    }

    [ContextMenu("SaveImages")]
    public void SaveImages()
    {
        File.WriteAllBytes("1.png", ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).EncodeToPNG());
        File.WriteAllBytes("2.png", ((Texture2D)m_resMaterial.mainTexture).EncodeToPNG());
    }

    public void Init(ImageInfo imageInfo, ImageOpenType imageOpenType, Texture2D tex)
    {
        m_imageInfo = imageInfo;
        m_grayRenderer.sharedMaterial.mainTexture = tex;
        m_gridRenderer.sharedMaterial.mainTextureScale = new Vector2((float)tex.width, (float)tex.height);
        m_highlightedGridRenderer.sharedMaterial.SetTexture("_ImageTex", tex);
        m_highlightedGridRenderer.sharedMaterial.mainTextureScale = new Vector2((float)tex.width, (float)tex.height);
        m_width = tex.width;
        m_height = tex.height;
        m_pixels = tex.GetPixels();
        m_colors = m_pixels.Distinct().ToList();//Where(c => c.r != 1 && c.g != 1 && c.b != 1).ToList();

        m_colors.Remove(Color.white);
        m_colors.Remove(transparent);

        var texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        texture2D.filterMode = FilterMode.Point;
        var startColors = new Color[m_pixels.Length];
        Color color = new Color(1f, 1f, 1f, 0f);
        Color white = Color.white;
        m_colorsToColor = 0;
        for (int i = 0; i < startColors.Length; i++)
        {
            if (m_pixels[i] == white)
            {
                startColors[i] = white;
            }
            else
            {
                startColors[i] = color;
                m_colorsToColor++;
            }
        }
        texture2D.SetPixels(startColors);
        texture2D.Apply();
        m_resMaterial.mainTexture = texture2D;
        m_inited = true;
        m_colorsCount.Clear();

        var resPixels = ((Texture2D)m_resMaterial.mainTexture).GetPixels();
        var rightPixels = ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).GetPixels();

        m_totalNumbers = 0;
        foreach (var c in m_colors)
        {
            int count = 0;
            for (int i = 0; i < rightPixels.Length; i++)
            {
                if (rightPixels[i] == c && resPixels[i].a < 0.9f)
                {
                    count++;
                    m_totalNumbers++;
                }
            }
            m_colorsCount[c] = count;
        }

        base.StartCoroutine(GenerateNumbersCoroutine_new(delegate
        {
            if (m_width > m_height)
            {
                transform.localScale = new Vector3(1f, (float)m_height / (float)m_width, 1f);
            }
            else if (m_width < m_height)
            {
                transform.localScale = new Vector3((float)m_width / (float)m_height, 1f, 1f);
            }
        }));
        base.StartCoroutine(DefferedEnableCoroutine());
        m_highlightedGridRenderer.gameObject.SetActive(AppData.BulbMode);
        AnalyticsManager.Instance.ImageOpened(m_imageInfo, m_colorsToColor, m_colors.Count, imageOpenType);
    }

    void Update()
    {
        if (m_inited)
        {
            float value = Mathf.Lerp(1f, 0f, (m_cameraManager.Zoom - (m_cameraManager.MaxZoom - 1f) * 0.1f - 1f) / (m_cameraManager.MaxZoom * 0.4f));
            m_grayRenderer.sharedMaterial.SetFloat("_Alpha", value);
        }
    }
    public List<Color> GetColors()
    {
        return m_colors;
    }

    public bool IsEmpty()
    {
        try
        {
            if (m_isEmpty)
            {
                m_isEmpty = (m_history == null || m_history.Steps.Count == 0);
            }
            return m_isEmpty;
        }
        catch
        {
            return true;
        }
    }

    public void SetColor(Color color)
    {
        Debug.Log("set highlight color: " + color);
        m_highlightedGridRenderer.sharedMaterial.SetColor("_CurrentColor", color);

    }

    public void TryClickPixel(Vector2 mousePosition)
    {
        var ray = m_mainCamera.ScreenPointToRay(mousePosition);
        var raycastHits = Physics.RaycastAll(ray);
        if (raycastHits != null && raycastHits.Length > 0)
        {

            var meshRenderer = raycastHits[0].collider.gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                Debug.Log(raycastHits.Count());
                var mainTexture = meshRenderer.sharedMaterial.mainTexture;
                Vector2 point = raycastHits[0].point;

                float x = point.x;
                Vector2 lossyScale = raycastHits[0].collider.transform.lossyScale;

                point.x = x / lossyScale.x;
                float y = point.y;

                Vector2 scale = raycastHits[0].collider.transform.lossyScale;

                point.y = y / scale.y;
                Debug.Log(mainTexture.width);
                point.x = (point.x + 1f) / 2f* (float)mainTexture.width;


                point.y = (point.y + 1f) / 2f*(float)mainTexture.height;

                SetPixelColor(point, WorkbookModel.Instance.CurrentColorModel.Color);

            }

        }
    }

    public bool CheckClickPixel(Vector2 mousePosition)
    {
        var ray = m_mainCamera.ScreenPointToRay(mousePosition);
        var raycastHits = Physics.RaycastAll(ray);
        if (raycastHits != null && raycastHits.Length > 0)
        {
            var meshRenderer = raycastHits[0].collider.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                var mainTexture = meshRenderer.sharedMaterial.mainTexture;
                Vector3 point = raycastHits[0].point;
                float x = point.x;
                Vector3 lossyScale = raycastHits[0].collider.transform.lossyScale;
                point.x = x / lossyScale.x;
                float y = point.y;
                Vector3 scale = raycastHits[0].collider.transform.lossyScale;
                point.y = y / scale.y;
                point.x = (point.x + 1f) / 2f * (float)mainTexture.width;
                point.y = (point.y + 1f) / 2f * (float)mainTexture.height;
                Color color = WorkbookModel.Instance.CurrentColorModel.Color;
                Color pixel = ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).GetPixel((int)point.x, (int)point.y);
                Color resPixel = ((Texture2D)m_resMaterial.mainTexture).GetPixel((int)point.x, (int)point.y);
                if (pixel != resPixel && pixel == color)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetPixelColor(Vector2 pixelPos, Color color)
    {

        Color pixel = ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).GetPixel((int)pixelPos.x, (int)pixelPos.y);
        if (WorkbookModel.Instance.SpecBoostersModel.LassoMode || WorkbookModel.Instance.SpecBoostersModel.BombMode)
        {
            color = pixel;
            Debug.Log(pixel);
        }
        Color resPixel = ((Texture2D)m_resMaterial.mainTexture).GetPixel((int)pixelPos.x, (int)pixelPos.y);
        bool pixelEqual = false;
        if (!(pixel == Color.white) && !(pixel == resPixel))
        {
            if (pixel == color)
            {
                var historyStep = new HistoryStep();
                historyStep.Add(new ShortVector2((int)pixelPos.x, (int)pixelPos.y));
                AddHistoryStep(historyStep);

                pixelEqual = true;
            }
            if (!pixelEqual)
            {
                if (WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType == ColorizationModeModel.BrushType.Plural)
                {
                    int pixelX = (int)pixelPos.x;
                    int pixelY = (int)pixelPos.y;
                    float fracX = pixelPos.x - (float)pixelX;
                    float fracY = pixelPos.y - (float)pixelY;
                    if (fracX < 0.25f)
                    {
                        return ;
                    }
                    if (fracX > 0.75f)
                    {
                        return;
                    }
                    if (fracY < 0.25f)
                    {
                        return;
                    }
                    if (fracY > 0.75f)
                    {
                        return;
                    }
                }
                color.a = 0.5f;
                Color lhs = resPixel;
                lhs.a = 0.5f;
                if (!(lhs == color))
                {
                    AnalyticsManager.Instance.ElementColored(WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType, false);
                    pixelEqual = true;
                }
            }
        }
        if (!pixelEqual)
            return;
        
        ((Texture2D)m_resMaterial.mainTexture).SetPixel((int)pixelPos.x, (int)pixelPos.y, color);
        ((Texture2D)m_resMaterial.mainTexture).Apply();
        if (color.a > 0.9f)
        {
            VibroWrapper.PlayVibroRight();
            AnalyticsManager.Instance.ElementColored(WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType, true);
            var pixels = ((Texture2D)m_resMaterial.mainTexture).GetPixels();
            var grayPixels = ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).GetPixels();
            if (WorkbookModel.Instance.SpecBoostersModel.LassoMode)
            {
                CheckLassoNeighbours(grayPixels, pixels, (int)pixelPos.x, (int)pixelPos.y, color);
                ((Texture2D)m_resMaterial.mainTexture).SetPixels(pixels);
                ((Texture2D)m_resMaterial.mainTexture).Apply();
                WorkbookModel.Instance.SpecBoostersModel.SpendLasso();
                AudioManager.Instance.PlayWand();
                AnalyticsManager.Instance.LassoUsed(WorkbookModel.Instance.SpecBoostersModel.LassoCount + 1);
            }
            if (WorkbookModel.Instance.SpecBoostersModel.BombMode)
            {
                CheckBombNeighbours(grayPixels, pixels, (int)pixelPos.x, (int)pixelPos.y, color);
                ((Texture2D)m_resMaterial.mainTexture).SetPixels(pixels);
                ((Texture2D)m_resMaterial.mainTexture).Apply();
                WorkbookModel.Instance.SpecBoostersModel.SpendBomb();
                AudioManager.Instance.PlayBomb();
                AnalyticsManager.Instance.BombUsed(WorkbookModel.Instance.SpecBoostersModel.BombCount + 1);
                foreach (Color c in m_colors)
                {
                    CheckColor(c, pixels, grayPixels);
                }
            }
            m_colorsCount[color]--;
            m_totalNumbers--;
            CheckColor(color, pixels, grayPixels);
            OnPaintedPixel(color);

            if (m_totalNumbers == 0)
            {
                Completed = true;
                AnalyticsManager.Instance.ImageDone(m_imageInfo, m_colorsToColor, m_colors.Count);
                OnComplete.SafeInvoke();
                AudioManager.Instance.PlayVictory();
            }
        }
        else
        {
            VibroWrapper.PlayVibroWrong();
            DailyGame.IncresetError();
        }
    }

    public bool CheckColorLeft(Color color, bool soundEnabled = true)
    {
        if (m_grayRenderer.sharedMaterial.mainTexture != null && m_resMaterial.mainTexture != null)
        {
            var pixels = ((Texture2D)m_resMaterial.mainTexture).GetPixels();
            var grayPixels = ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).GetPixels();
            return CheckColor(color, pixels, grayPixels, soundEnabled) > 0;
        }

        return false;
    }

    public int GetColorsNumber()
    {
        if (m_grayRenderer.sharedMaterial.mainTexture != null && m_resMaterial.mainTexture != null)
        {
            var resPixels = ((Texture2D)m_resMaterial.mainTexture).GetPixels();
            var rightPixels = ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).GetPixels();

            int count = 0;
            for (int i = 0; i < rightPixels.Length; i++)
            {
                if (resPixels[i].a < 0.9f)
                {
                    count++;
                }
            }
            return count;
        }
        return 0;
    }

    int CheckColor(Color color, Color[] resPixels, Color[] rightPixels, bool soundEnabled = true)
    {
        int count = 0;
        if (!soundEnabled)
        {
            for (int i = 0; i < rightPixels.Length; i++)
            {
                if (rightPixels[i] == color && resPixels[i].a < 0.9f)
                {
                    count++;
                }
            }
            int diff = m_colorsCount[color] - count;
            m_colorsCount[color] = count;
            m_totalNumbers -= diff;
        }
        else
        {
            count = m_colorsCount[color];
        }
        if (OnCountLeft != null)
        {
            OnCountLeft(color, count);
        }
        if (count > 0)
            return count;
        OnColorComplete.SafeInvoke(color);
        if (soundEnabled)
        {
            AudioManager.Instance.PlayCompleteColor();
        }
        return 0;
    }

    void CheckLassoNeighbours1(Color[] rightPixels, Color[] coloredPixels, int x, int y, Color color)
    {
        try
        {
            var offsets = new Vector2[4] {
                Vector2.up,
                Vector2.down,
                Vector2.left,
                Vector2.right
            };
            var stack = new Stack<Vector2>();
            var nextStack = new Stack<Vector2>();
            bool first = true;
            stack.Push(new Vector2((float)x, (float)y));
            int startGray = Mathf.RoundToInt(color.grayscale * 1000f);
            while (stack.Count > 0)
            {

                    Vector2 current = stack.Pop();
                    int index = (int)current.x + (int)current.y * m_width;
                    int gray = Mathf.RoundToInt(coloredPixels[index].grayscale * 1000f);
                    if (gray != startGray || first)
                    {
                        first = false;
                        coloredPixels[index] = color;
                        var neighbours = offsets;
                        for (int i = 0; i < neighbours.Length; i++)
                        {
                            Vector2 neighborOffset = neighbours[i];
                            int nx = (int)current.x + (int)neighborOffset.x;
                            int ny = (int)current.y + (int)neighborOffset.y;
                            Vector2 neighborPosition = new Vector2((float)nx, (float)ny);
                            index = nx + ny * m_width;
                            if (nx >= 0 && nx < m_width && ny >= 0 && ny < m_height)
                            {
                                int neighbourGray = Mathf.RoundToInt(coloredPixels[index].grayscale * 1000f);
                                int rightGray = Mathf.RoundToInt(rightPixels[index].grayscale * 1000f);
                                if (neighbourGray != startGray && rightGray == startGray)
                                {
                                    nextStack.Push(new Vector2((float)nx, (float)ny));
                                }
                            }
                        }
                    }

                stack = nextStack;
                nextStack = new Stack<Vector2>();

            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void CheckLassoNeighbours(Color[] rightPixels, Color[] coloredPixels, int x, int y, Color color)
    {
        var offsets = new Vector2[4] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };
        var stack = new Stack<Vector2>();
        var nextStack = new Stack<Vector2>();
        bool first = true;
        stack.Push(new Vector2((float)x, (float)y));
        int startColor = (int)((color.r + color.g * 256f + color.b * 65536f) * 1000f);
        var historyStep = new HistoryStep();
        while (stack.Count > 0)
        {
            while (stack.Count > 0)
            {
                Vector2 current = stack.Pop();
                int index = (int)current.x + (int)current.y * m_width;
                int colorKey = (int)((coloredPixels[index].r + coloredPixels[index].g * 256f + coloredPixels[index].b * 65536f) * 1000f);
                if (colorKey != startColor || first)
                {
                    first = false;
                    coloredPixels[index] = color;
                    historyStep.Add(new ShortVector2((int)current.x, (int)current.y));
                    var neighbours = offsets;
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        Vector2 neighborOffset = neighbours[i];
                        int nx = (int)current.x + (int)neighborOffset.x;
                        int ny = (int)current.y + (int)neighborOffset.y;
                        Vector2 neighborPosition = new Vector2((float)nx, (float)ny);
                        index = nx + ny * m_width;
                        if (nx >= 0 && nx < m_width && ny >= 0 && ny < m_height)
                        {
                            int neighbourColor = (int)((coloredPixels[index].r + coloredPixels[index].g * 256f + coloredPixels[index].b * 65536f) * 1000f);
                            int rightColor = (int)((rightPixels[index].r + rightPixels[index].g * 256f + rightPixels[index].b * 65536f) * 1000f);
                            if (neighbourColor != startColor && rightColor == startColor)
                            {
                                nextStack.Push(new Vector2((float)nx, (float)ny));
                            }
                        }
                    }
                }
            }
            stack = nextStack;
            nextStack = new Stack<Vector2>();
        }
        AddHistoryStep(historyStep);
    }

    void CheckBombNeighbours(Color[] rightPixels, Color[] coloredPixels, int x, int y, Color color)
    {
        var historyStep = new HistoryStep();
        int radius = m_bombMatrix.GetLength(0) / 2;
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                int px = x + i;
                int py = y + j;
                if (px >= 0 && px < m_width && py >= 0 && py < m_height && m_bombMatrix[radius + i, radius + j] > 0)
                {
                    int index = px + py * m_width;
                    coloredPixels[index] = rightPixels[index];
                    historyStep.Add(new ShortVector2(px, py));
                }
            }
        }
        AddHistoryStep(historyStep);
    }

    public byte[] GetRes()
    {
        if (!(m_resMaterial == null) && !(m_resMaterial.mainTexture == null))
        {
            var texture2D = new Texture2D(m_width, m_height, TextureFormat.RGBA32, false);
            texture2D.filterMode = FilterMode.Point;
            var texture2D2 = texture2D;
            var pixels = ((Texture2D)m_resMaterial.mainTexture).GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < 1f)
                {
                    pixels[i].a = 0f;
                }
            }
            texture2D2.SetPixels(pixels);
            texture2D2.Apply();
            return texture2D2.EncodeToPNG();
        }
        return null;
    }

    public Texture2D GetFullTexture()
    {
        return (Texture2D)m_grayRenderer.sharedMaterial.mainTexture;
    }

    public History GetHistory()
    {
        return m_history;
    }

    void AddHistoryStep(HistoryStep historyStep)
    {
        if (m_history == null)
        {
            var savedWorkData = NewWorkbookManager.Instance.SaveWork(true);
            savedWorkData.InitHistory();
            m_history = savedWorkData.History2;
        }
        m_history.AddStep(historyStep);
    }

    public void ApplyResTexture(Texture2D tex, bool completed, History history)
    {
        m_resMaterial.mainTexture = tex;
        Completed = completed;
        if (history != null)
        {
            m_history = history;
        }
    }

    public void CheatComplete()
    {
        m_resMaterial.mainTexture = m_grayRenderer.sharedMaterial.mainTexture;
        Completed = true;
        OnComplete.SafeInvoke();
    }
    IEnumerator DefferedEnableCoroutine()
    {
        yield return null;
        m_content.SetActive(true);
        yield return null;
        var allPix = ((Texture2D)m_resMaterial.mainTexture).GetPixels();
        var rightPix = ((Texture2D)m_grayRenderer.sharedMaterial.mainTexture).GetPixels();
        foreach(var c in m_colors)
        {
            CheckColor(c, allPix, rightPix, false);
        }

        CheckColorLeft(m_colors.FirstOrDefault(), false);

        if (DailyGame.IsDailyArt())
        {
            DailyGame.UpdateGUI(GetColorsNumber());
        }
    }


    IEnumerator GenerateNumbersCoroutine_new(Action endAction)
    {
        float defaultAlpha = 1;
        float defRed = 0f;
        float defGreen = 0f;
        float defBlue = 0f;
        var numberPixels = new Color[10][];
        numberPixels[0] = ((Texture2D)Resources.Load("number_zero")).GetPixels();
        numberPixels[1] = ((Texture2D)Resources.Load("number_one")).GetPixels();
        numberPixels[2] = ((Texture2D)Resources.Load("number_two")).GetPixels();
        numberPixels[3] = ((Texture2D)Resources.Load("number_three")).GetPixels();
        numberPixels[4] = ((Texture2D)Resources.Load("number_four")).GetPixels();
        numberPixels[5] = ((Texture2D)Resources.Load("number_five")).GetPixels();
        numberPixels[6] = ((Texture2D)Resources.Load("number_six")).GetPixels();
        numberPixels[7] = ((Texture2D)Resources.Load("number_seven")).GetPixels();
        numberPixels[8] = ((Texture2D)Resources.Load("number_eight")).GetPixels();
        numberPixels[9] = ((Texture2D)Resources.Load("number_nine")).GetPixels();
        var sizeKoef = CELL_SIZE;
        var newWidth = m_width * sizeKoef;
        var newHeight = m_height * sizeKoef;
        var maxSize = 3600;
        var colors = new Color[(sizeKoef) * (sizeKoef)];
        var borderColors = new Color[sizeKoef * borderWidth];
        for (int i = 0; i < borderColors.Length; i++)
        {
            borderColors[i] = borderColor;
        }
        var colorsDict = new Dictionary<Color, int>();
        for (int i = 0; i < m_colors.Count; i++)
        {
            colorsDict.Add(m_colors[i], i + 1);
        }
        int mainI = 0;
        while (mainI * maxSize < newWidth)
        {
            var curWidth = Mathf.Min(maxSize, newWidth - mainI * maxSize);
            int mainJ = 0;
            while (mainJ * maxSize < newHeight)
            {
                var curHeight = Mathf.Min(maxSize, newHeight - mainJ * maxSize);
                var bigTex = new Texture2D(curWidth, curHeight, TextureFormat.RGBA32, false);
                var tempColors = new Color[curWidth * curHeight];
                bigTex.SetPixels(tempColors);
                yield return null;

                for (int i = 0; i < curWidth / sizeKoef; i++)
                {
                    if (i % 20 == 0)
                    {
                        yield return null;
                    }

                    var curI = i + mainI * maxSize / sizeKoef;

                    int firstColumnCell = -1;
                    int lastColumnCell = -1;
                    for (int j = 0; j < curHeight / sizeKoef; j++)
                    {
                        int offset = j + mainJ * maxSize / sizeKoef;
                        if (colorsDict.ContainsKey(m_pixels[curI + offset * m_width]))
                        {
                            //if (firstColumnCell < 0)
                            //{
                            //	firstColumnCell = j;
                            //}
                            int pixelNumber = colorsDict[m_pixels[curI + offset * m_width]];
                            bigTex.SetPixels(i * sizeKoef, j * sizeKoef, sizeKoef, sizeKoef, colors);
                            if (pixelNumber < 10)
                            {
                                bigTex.SetPixels(i * sizeKoef + 18, j * sizeKoef + 15, 14, 20, numberPixels[pixelNumber]);
                            }
                            else if (pixelNumber < 100)
                            {
                                int digit1 = pixelNumber / 10;
                                int digit2 = pixelNumber - digit1 * 10;
                                bigTex.SetPixels(i * sizeKoef + 9, j * sizeKoef + 15, 14, 20, numberPixels[digit1]);
                                bigTex.SetPixels(i * sizeKoef + 26, j * sizeKoef + 15, 14, 20, numberPixels[digit2]);
                            }
                            else if (pixelNumber < 1000)
                            {
                                int digit1 = pixelNumber / 100;
                                int temp = pixelNumber - digit1 * 100;
                                int digit2 = temp / 10;
                                int digit3 = temp - digit2 * 10;
                                bigTex.SetPixels(i * sizeKoef + 3, j * sizeKoef + 9, 8, 12, numberPixels[digit1]);
                                bigTex.SetPixels(i * sizeKoef + 11, j * sizeKoef + 9, 8, 12, numberPixels[digit2]);
                                bigTex.SetPixels(i * sizeKoef + 19, j * sizeKoef + 9, 8, 12, numberPixels[digit3]);
                            }

                            //top border
                            bigTex.SetPixels(i * sizeKoef, j * sizeKoef, sizeKoef, borderWidth, borderColors);

                            //left border
                            bigTex.SetPixels(i * sizeKoef, j * sizeKoef, borderWidth, sizeKoef, borderColors);

                            //bottom border
                            bigTex.SetPixels(i * sizeKoef, j * sizeKoef + sizeKoef - 1, sizeKoef, borderWidth, borderColors);

                            //right border
                            bigTex.SetPixels(i * sizeKoef + sizeKoef - 1, j * sizeKoef, borderWidth, sizeKoef, borderColors);

                            //lastColumnCell = j;
                        }
                    }

                    //if (firstColumnCell >= 0)
                    //{
                    //	//top border
                    //	this.bigTex.SetPixels(this.i * this.sizeKoef, firstColumnCell * this.sizeKoef, this.sizeKoef, this._self.borderWidth, borderColors);
                    //}
                }
                bigTex.Apply();
                var curNumbersContent = UnityObject.Instantiate(m_numbersContent);
                curNumbersContent.SetParent(m_numbersContent.parent);
                var mf = curNumbersContent.gameObject.AddComponent<MeshFilter>();
                mf.sharedMesh = new Mesh();
                float offsetxy = -1f;
                mf.sharedMesh.vertices = new Vector3[4] {
                        new Vector2(offsetxy + 2f / (float)newWidth * (float)mainI * (float)maxSize, offsetxy + 2f / (float)newHeight * (float)mainJ * (float)maxSize),
                        new Vector2(offsetxy + 2f / (float)newWidth * (float)(mainI * maxSize +curWidth), offsetxy + 2f / (float)newHeight * (float)mainJ * (float)maxSize),
                        new Vector2(offsetxy + 2f / (float)newWidth * (float)mainI * (float)maxSize, offsetxy + 2f / (float)newHeight * (float)(mainJ * maxSize + curHeight)),
                        new Vector2(offsetxy + 2f / (float)newWidth * (float)(mainI * maxSize + curWidth), offsetxy + 2f / (float)newHeight * (float)(mainJ * maxSize + curHeight))
                    };
                mf.sharedMesh.uv = new Vector2[4] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
                mf.sharedMesh.triangles = new int[6] { 0, 3, 1, 2, 3, 0 };
                var mr = curNumbersContent.gameObject.AddComponent<MeshRenderer>();
                mr.sharedMaterial = new Material(Shader.Find("Custom/AlphaBaseShader"));
                //this.mr.sharedMaterial.SetInt("_Inverted", 1);
                mr.sharedMaterial.mainTexture = bigTex;
                mainJ++;
            }
            mainI++;
        }
        endAction.SafeInvoke();
    }
}
