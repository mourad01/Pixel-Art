using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxDLL;

public class VoxCubeItem : MonoBehaviour
{
    public bool isColored;
    public bool isError;
    public bool isStartCorut;
    public int ColorIndex = -1;

    List<MeshRenderer> _renderers;
    MeshRenderer _saveCubeRenderer;
    Mesh _mesh;
    ColorSettings _colorSettings;

    public bool isRender = true;

    public Color Color { get; set; }

    public Mesh Mesh
    {
        get
        {
            return _mesh;
        }
    }

    public VoxCubeItem()
    {
    }

    public void CheatTochCubeItem()
    {
        if (isColored)
        {
            return;
        }
        SetSuccessColorCube();
    } 

    public void Init(ColorSettings colorSettings, Color color, Mesh mesh, bool[] isVisible)
    {
        if (colorSettings == null)
        {
            Debug.LogError("colorSettings is null");
        }
        var transform = base.transform.Find("Cube");
        _renderers = transform.GetComponents<MeshRenderer>().ToList<MeshRenderer>();
        var transform1 = base.transform.Find("save_cube");
        _saveCubeRenderer = transform1.GetComponent<MeshRenderer>(); 
        InitResources();
        _colorSettings = colorSettings;
        _renderers[0].gameObject.SetActive(((IEnumerable<bool>)isVisible).Any<bool>((bool a) => a));
        _saveCubeRenderer.gameObject.SetActive(((IEnumerable<bool>)isVisible).Any<bool>((bool a) => a)); 
        Color = color;
        //this.ColorIndex = this._colorSettings.GetNumberByColor(this.Color) + 1;
        _mesh = mesh;
        if (ColorIndex == 0)
        {
            SetSuccessColorCube();
            return;
        }
        SetNumberByFaceCube(ColorIndex != Loader3D.CurrentIndex);
        _saveCubeRenderer.material.color = (Color);
        float _grayscale = Color.grayscale + 0.4f;
        Color gray = new Color(_grayscale, _grayscale, _grayscale);
        _saveCubeRenderer.material.SetColor("_BaseColor", gray); 
    }

    void InitResources()
    {
        var shader = Shader.Find("Custom/3DColour");
        var material = new Material(shader);
        material.SetFloat("_Blend", 1f);
        material.SetColor("_Color", UnityEngine.Color.white);
        foreach (MeshRenderer _renderer in _renderers)
        {
            _renderer.material = material;
        }
        Shader.Find("Custom/3DColour");
        var material1 = new Material(shader);
        material1.SetFloat("_Blend", 0f);
        material1.SetColor("_Color", UnityEngine.Color.white);
        _saveCubeRenderer.material = material1;
        Shader.Find("Custom/3DColour");
        var mat = new Material(shader);
        mat.SetFloat("_Blend", 0f);
        mat.SetColor("_Color", UnityEngine.Color.white); 
    }

    IEnumerator SetErrorColorByFaceCube()
    {
        var voxCubeItem = this;
        var colors = new List<Color>();
        var nums = new List<int>();
        foreach (MeshRenderer _renderer in voxCubeItem._renderers)
        {
            colors.Add(_renderer.material.color);
            Color colorByIndex = voxCubeItem._colorSettings.GetColorByIndex(Loader3D.CurrentIndex - 1);
            nums.Add(_renderer.material.GetInt("_Highlighted"));
            _renderer.material.SetInt("_Highlighted", 0);
            _renderer.material.color = (colorByIndex);
        }
        yield return new WaitForSeconds(0.1f);
        int index = 0;
        foreach (MeshRenderer meshRenderer in voxCubeItem._renderers)
        {
            voxCubeItem._colorSettings.GetColorByIndex(Loader3D.CurrentIndex - 1);
            meshRenderer.material.SetInt("_Highlighted", nums[index]);
            meshRenderer.material.color = (colors[index]);
            index++;
        }
        voxCubeItem.isStartCorut = false;
    }

    public void SetHighLightColorByFaceCube()
    {
        if (isColored)
        {
            return;
        }
        SetNumberByFaceCube(ColorIndex == Loader3D.CurrentIndex);
    }

    Texture2D SetHighLightTextureOnFaceCube()
    {
        Texture2D texture2D;
        try
        {
            texture2D = (Texture2D)Resources.Load(string.Concat("Textures/Grey/", ColorIndex.ToString()));
        }
        catch (Exception exception)
        {
            Debug.LogError(string.Concat("Error = ", exception.Message));
            texture2D = null;
        }
        return texture2D;
    }

    Texture2D SetNormalTextureOnFaceCube()
    {
        Texture2D texture2D;
        try
        {
            texture2D = (Texture2D)Resources.Load(string.Concat("Textures/Black/", ColorIndex.ToString()));
        }
        catch (Exception exception)
        {
            Debug.LogError(string.Concat("Error = ", exception.Message));
            texture2D = null;
        }
        return texture2D;
    }

    void SetNumberByFaceCube(bool highlight)
    {
        Color color;
        foreach (MeshRenderer _renderer in _renderers)
        {
            if (highlight)
            {
                _renderer.material.mainTexture = (SetHighLightTextureOnFaceCube());
                color = Color;
                float single = Mathf.Min(color.grayscale + 0.3f, 0.7f);
                Color color1 = new Color(single, single, single);
                _renderer.material.SetColor("_BaseColor", color1);
                _renderer.material.SetInt("_Highlighted", 1);
            }
            else
            {
                _renderer.material.mainTexture = (SetNormalTextureOnFaceCube());
                color = Color;
                float _grayscale = color.grayscale + 0.4f;
                Color gray = new Color(_grayscale, _grayscale, _grayscale);
                _renderer.material.SetColor("_BaseColor", gray);
                _renderer.material.SetInt("_Highlighted", 0);
            }
        }
    }

    public bool SetProgressForCube()
    {
        if (isColored)
        {
            return false;
        }
        SetSuccessColorCube();
        return true;
    }

    void SetSuccessColorCube()
    {
        isColored = true;
        (new Material(Shader.Find("Custom/3DColour"))).SetFloat("_Blend", 1f);
        foreach (MeshRenderer meshRenderer in new List<MeshRenderer>(base.GetComponentsInChildren<MeshRenderer>()))
        {
            meshRenderer.sharedMaterial.color = (Color);
            meshRenderer.sharedMaterial.SetColor("_BaseColor", Color);
            meshRenderer.sharedMaterial.mainTexture = (null);
        }
    }

    public void SetTransparentColorByOffsetZ(float transZ)
    {
        if (isColored)
        {
            return;
        }
        foreach (MeshRenderer _renderer in _renderers)
        {
            Color color = _renderer.material.GetColor("_Color");
            color.a = transZ;
            _renderer.material.SetColor("_Color", color);
        }
    }

    public void SetVisible(bool isVisible)
    {
        foreach (MeshRenderer meshRenderer in new List<MeshRenderer>(gameObject.GetComponentsInChildren<MeshRenderer>()))
        {
            if (meshRenderer == null)
            {
                continue;
            }
            meshRenderer.enabled = (isVisible);
        }
    }

    public int TochCubeItem()
    {
        if (isColored)
        {
            return -1;
        }
        if (ColorIndex == Loader3D.CurrentIndex)
        {
            SetSuccessColorCube();
            return 1;
        }
        if (isStartCorut)
        {
            return 0;
        }
        isStartCorut = true;
        base.StartCoroutine(SetErrorColorByFaceCube());
        return 0;
    }
}
