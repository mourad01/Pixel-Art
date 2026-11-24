using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class ImagesInfo
{
    List<ImageInfo> m_images;

    public int Version { get; set; }
    public int Count { get;  set; }

    public List<ImageInfo> Images
    {
        get
        {
            return m_images;
        }
    }

    public ImagesInfo()
    {
        Version = -1;
        m_images = new List<ImageInfo>();
    }

    public void Add(ImageInfo imageInfo)
    {
        m_images.Add(imageInfo);
        Count++;
    }
#if UNITY_EDITOR
    public void Arrange(string onlineDatasetLocation, ImagesInfo existingImages)
    {
        if (existingImages != null && existingImages.Images != null)
        {
            var dict = new Dictionary<string, ImageInfo>();
            foreach (var image in existingImages.Images)
            {
                dict[image.Id] = image; 
            }

            //new images go first, then the existing ones in their old order
            var list = new List<ImageInfo>();
            foreach(var image in m_images)
            {
                if (!dict.ContainsKey(image.Id))
                {
                    list.Add(image);
                }
            }

            foreach (var image in existingImages.Images)
            {
                list.Add(image);
            }
            m_images.Clear();

            foreach(var image in list)
            {
                //drop entries whose png (and .vox for 3D) is missing
                string fileName = Path.Combine(onlineDatasetLocation, image.Url);
                if (File.Exists(fileName) && (!image.Is3D || File.Exists(fileName + ".vox")))
                {
                    m_images.Add(image);
                }
                else
                {
                    Debug.Log(fileName + " does not exist");
                }
            }

            Count = m_images.Count;
        }
    }
#endif
}


