using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxDLL
{
    public class Loader3D : MonoBehaviour
    {
        public static int CurrentIndex;

        MVMainChunk v;
        float sizePerVox = 1f;
        Action<Color[]> ColorsPalleteLoadComplete;
        Action<List<VoxCubeItem>> CubesDataLoadComplete;
        bool isInit;

        public MVMainChunk MV
        {
            get { return v; }
        }

        static Loader3D()
        {
        }

        public Loader3D()
        {
        }

        public void Init(Action<Color[]> actionColorsComplete, Action<List<VoxCubeItem>> actionCubesComplete)
        {
            ColorsPalleteLoadComplete = actionColorsComplete;
            CubesDataLoadComplete = actionCubesComplete;
            MVImporter.Init(actionColorsComplete, actionCubesComplete);
            isInit = true;
        }

        public void Load(byte[] data)
        {
            if (!isInit)
            {
                Debug.LogError("[Loader3D] Module isn't initilized. Try call method Init() before");
                return;
            }
            if (data == null)
            {
                Debug.LogError("[Loader3D] Invalid file data");
                return;
            }
            v = MVImporter.LoadVOXFromData(data, false);
            var colorSetting = new ColorSettings(v.palatte);
            if (v.palatte == null)
            {
                Debug.LogError("[Loader3D] Invalid file pallete");
                return;
            }
            MVImporter.AddColorSettings(colorSetting);
            var defaultMaterial = MVImporter.DefaultMaterial;
            defaultMaterial.mainTexture = v.PaletteToTexture();
            var gameObject = GameObject.Find("3DVox");
            if (gameObject == null)
            {
                Debug.LogError("[Loader3D] Game object 3DVox not found. Create in scene this object!");
                return;
            }
            //voxels are built as children of the 3DVox object
            MVImporter.CreateIndividualVoxelGameObjects(v, gameObject.transform, defaultMaterial, sizePerVox);
        }
    }
}
