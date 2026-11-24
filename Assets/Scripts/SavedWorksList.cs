using UnityDebug = global::UnityEngine.Debug;
using Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[Serializable]
public class SavedWorksList
{
    static string s_filePath = "swl.swl";

    public static string Ext = ".dat";

    Dictionary<string, List<SavedWorkInfo>> m_dict;
    List<PhotoInfo> m_photoList;

    public static string GetPathToSave(string id)
    {
        return AppPathsConfig.SavesPath + id + Ext;
    }

    public void Init()
    {
        s_filePath = AppPathsConfig.SavesPath + s_filePath;
        if (File.Exists(s_filePath))
        {
            try
            {
                m_dict = Serializer.LoadFromFile<Dictionary<string, List<SavedWorkInfo>>>(s_filePath);
            }
            catch (Exception ex)
            {
                UnityDebug.Log(ex.Message);
            }
            finally
            {
                if (m_dict == null)
                {
                    m_dict = new Dictionary<string, List<SavedWorkInfo>>();
                }
            }
        }
        else
        {
            m_dict = new Dictionary<string, List<SavedWorkInfo>>();
        }
        CheckSavedWorks();
    }

    void CheckSavedWorks()
    {
        var files = Directory.GetFiles(AppPathsConfig.SavesPath);
        var saveFiles = files;
        foreach (string text in saveFiles)
        {
            if (text.EndsWith(Ext))
            {
                var savedWorkData = Serializer.LoadFromFile<ISavedWorkData>(text);
                if (savedWorkData != null)
                {
                    if (!m_dict.ContainsKey(savedWorkData.ImageInfo.Id))
                    {
                        m_dict.Add(savedWorkData.ImageInfo.Id, new List<SavedWorkInfo>());
                    }
                    var savedWorkInfo = m_dict[savedWorkData.ImageInfo.Id].FirstOrDefault((SavedWorkInfo a) => a.Id == savedWorkData.Id);
                    if (savedWorkInfo == null)
                    {
                        m_dict[savedWorkData.ImageInfo.Id].Add(new SavedWorkInfo(savedWorkData.Id, File.GetLastWriteTime(text)));
                    }
                }
            }
        }
        Save();
    }

    public void UpdateList(string saveId, DateTime dateTime)
    {
        var savedWorkData = LoadById(saveId);
        if (!m_dict.ContainsKey(savedWorkData.ImageInfo.Id))
        {
            m_dict.Add(savedWorkData.ImageInfo.Id, new List<SavedWorkInfo>());
        }
        var savedWorkInfo = m_dict[savedWorkData.ImageInfo.Id].FirstOrDefault((SavedWorkInfo a) => a.Id == saveId);
        if (savedWorkInfo == null)
        {
            m_dict[savedWorkData.ImageInfo.Id].Add(new SavedWorkInfo(saveId, dateTime));
        }
        else
        {
            savedWorkInfo.DateTime = dateTime;
            UnityDebug.Log("Update " + savedWorkInfo.Id + "  " + savedWorkInfo.DateTime);
        }
    }

    public void ForceUpdateList()
    {
        var directoryInfo = new DirectoryInfo(AppPathsConfig.SavesPath);
        var enumerable = from a in directoryInfo.GetFiles()
                                           where a.FullName.EndsWith(".dat")
                                           select a;
        foreach (FileInfo fileInfo in enumerable)
        {
            var source = m_dict.SelectMany((KeyValuePair<string, List<SavedWorkInfo>> a) => a.Value);
            string fileName = fileInfo.Name.Replace(".dat", string.Empty);
            if (!source.Any((SavedWorkInfo a) => a.Id == fileName))
            {
                UpdateList(fileName, fileInfo.LastWriteTime);
            }
        }
        Save();
    }

    public List<string> GetSaves()
    {
        return (from b in m_dict.SelectMany((KeyValuePair<string, List<SavedWorkInfo>> a) => a.Value)
                orderby b.DateTime descending
                select b into c
                select c.Id).ToList();
    }

    public List<PhotoInfo> GetPhotos()
    {
        m_photoList = Serializer.LoadFromFile<List<PhotoInfo>>(AppPathsConfig.PhotosPath + AppPathsConfig.PhotoFileList);
        if (m_photoList == null)
        {
            m_photoList = new List<PhotoInfo>();
        }
        return m_photoList;
    }

    public void AddPhoto(PhotoInfo photoInfo)
    {
        m_photoList.Add(photoInfo);
        SavePhotos();
    }

    public void DeletePhoto(string photoId)
    {
        if (m_photoList != null)
        {
            var photoInfo = m_photoList.FirstOrDefault((PhotoInfo a) => a.Id == photoId);
            if (photoInfo != null)
            {
                m_photoList.Remove(photoInfo);
            }
            SavePhotos();
        }
    }

    void SavePhotos()
    {
        Serializer.SaveToFile(AppPathsConfig.PhotosPath + AppPathsConfig.PhotoFileList, m_photoList, false);
    }

    public void Save()
    {
        Serializer.SaveToFile(s_filePath, m_dict, false);
    }

    public ISavedWorkData Save(ImageInfo imageInfo, byte[] preview, ISavedWorkData savedWorkData, bool completed)
    {
        if (savedWorkData == null)
        {
            savedWorkData = new SavedWorkData();
        }
        savedWorkData.Init(imageInfo, preview, completed, null);
        if (!string.IsNullOrEmpty(savedWorkData.FileName))
        {
            Serializer.SaveToFile(savedWorkData.FullFileName, savedWorkData, true);
            if (savedWorkData.History2 == null)
            {
                savedWorkData.InitHistory();
            }
            savedWorkData.History2.Save();
        }
        else if (!File.Exists(savedWorkData.FullFileName))
        {
            savedWorkData.SetFile(imageInfo.Id.ToString());
            Serializer.SaveToFile(savedWorkData.FullFileName, savedWorkData, true);
            if (savedWorkData.History2 == null)
            {
                savedWorkData.InitHistory();
            }
            savedWorkData.History2.Save();
        }
        if (!m_dict.ContainsKey(imageInfo.Id))
        {
            m_dict.Add(imageInfo.Id, new List<SavedWorkInfo>());
        }
        var savedWorkInfo = m_dict[imageInfo.Id].FirstOrDefault((SavedWorkInfo a) => a.Id == savedWorkData.Id);
        if (savedWorkInfo != null)
        {
            savedWorkInfo.DateTime = DateTime.Now;
        }
        else
        {
            m_dict[imageInfo.Id].Add(new SavedWorkInfo(savedWorkData.Id, DateTime.Now));
        }
        Save();
        return savedWorkData;
    }

    public SavedWorkData3D Save3D(ImageInfo imageInfo, byte[] preview, SavedWorkData3D savedWorkData, bool completed, List<HistoryStep> history, List<bool> progress)
    {
        if (savedWorkData == null)
        {
            savedWorkData = new SavedWorkData3D();
        }
        savedWorkData.Init(imageInfo, preview, completed, history, progress);
        if (!string.IsNullOrEmpty(savedWorkData.FileName))
        {
            Serializer.SaveToFile(savedWorkData.FullFileName, savedWorkData, true);
        }
        else if (!File.Exists(savedWorkData.FullFileName))
        {
            savedWorkData.SetFile(imageInfo.Id.ToString());
            Serializer.SaveToFile(savedWorkData.FullFileName, savedWorkData, true);
        }
        else
        {
            int index = 0;
            while (File.Exists(AppPathsConfig.SavesPath + imageInfo.Id + "_" + index.ToString() + Ext))
            {
                index++;
            }
            savedWorkData.SetFile(imageInfo.Id + "_" + index.ToString());
            Serializer.SaveToFile(AppPathsConfig.SavesPath + imageInfo.Id + "_" + index.ToString() + Ext, savedWorkData, true);
        }
        if (!m_dict.ContainsKey(imageInfo.Id))
        {
            m_dict.Add(imageInfo.Id, new List<SavedWorkInfo>());
        }
        var savedWorkInfo = m_dict[imageInfo.Id].FirstOrDefault((SavedWorkInfo a) => a.Id == savedWorkData.Id);
        if (savedWorkInfo != null)
        {
            savedWorkInfo.DateTime = DateTime.Now;
        }
        else
        {
            m_dict[imageInfo.Id].Add(new SavedWorkInfo(savedWorkData.Id, DateTime.Now));
        }
        Save();
        return savedWorkData;
    }

    public void Load(NumberColoring nc, string fileName)
    {
        var savedWorkData = Serializer.LoadFromFile<ISavedWorkData>(fileName);
        savedWorkData.Apply(nc);
    }

    public ISavedWorkData Load(string fileName)
    {
        return Serializer.LoadFromFile<ISavedWorkData>(fileName);
    }

    public ISavedWorkData LoadById(string saveId)
    {
        return Serializer.LoadFromFile<ISavedWorkData>(GetPathToSave(saveId));
    }

    public void Delete(string saveId)
    {
        var swd = LoadById(saveId);
        if (m_dict.ContainsKey(swd.ImageInfo.Id))
        {
            var savedWorkInfo = m_dict[swd.ImageInfo.Id].FirstOrDefault((SavedWorkInfo a) => a.Id == swd.Id);
            if (savedWorkInfo != null)
            {
                m_dict[swd.ImageInfo.Id].Remove(savedWorkInfo);
            }
        }
        File.Delete(swd.FullFileName);
        Save();
    }

    public string LastSaveOfImageId(string imageId)
    {
        if (m_dict != null && m_dict.ContainsKey(imageId))
        {
            var savedWorkInfo = (from a in m_dict[imageId]
                                           orderby a.DateTime
                                           select a).LastOrDefault();
            if (savedWorkInfo != null)
            {
                return savedWorkInfo.Id;
            }
        }
        return null;
    }
}


