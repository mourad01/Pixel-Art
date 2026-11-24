using System.Linq;
using UnityEngine;

public class PrefabLoader : MonoBehaviour
{

    public BaseWindow[] windows;

    static PrefabLoader instance;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static T Load<T>(string name) where T : BaseWindow
    {
        T a = null;
        try
        {
            var w = instance.windows.FirstOrDefault(t => t.name == name);
            a = (T)w;
        }
        catch(MissingReferenceException e)
        {
            Debug.Log(e.Message);
        }
        return a;
    }
}
