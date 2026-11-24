using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPickerController : MonoBehaviour
{
    public bool IsInit;

    public event Action<string> GetImagePathComplete;

    public event Action<IList<string>> GetImageListPathComplete;

    public event Action<string> GetImagePathError;

    public event Action GetImageListPathError;

    public abstract void GetAllGalleryImagePaths();

    public abstract void Init(Action<bool> InitComplete);

    public abstract void OpenGallery();

    public virtual void OnGetImagePathComplete(string path)
    { 
        if (GetImagePathComplete != null)
        {
            GetImagePathComplete(path);
        }
    }

    public virtual void OnGetImageListPathComplete(IList<string> paths)
    { 
        if (GetImageListPathComplete != null)
        {
            GetImageListPathComplete(paths);
        }
    }

    public virtual void OnGetImagePathError(string path)
    { 
        if (GetImagePathError != null)
        {
            GetImagePathError(path);
        }
    }

    public virtual void OnGetImageListPathError()
    { 
        if (GetImageListPathError != null)
        {
            GetImageListPathError();
        }
    }
}


