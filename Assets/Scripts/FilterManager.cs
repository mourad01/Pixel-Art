using System.Collections.Generic;
using UnityEngine;

public class FilterManager : MonoBehaviour
{
    [SerializeField] List<Texture2D> m_filters;

    public Texture2D GetFilter(int filterId)
    {
        var list = new List<Texture2D>();
        for (int i = 0; i < m_filters.Count; i++)
        {
            list.Add(m_filters[i]);
        }
        var texture2D = list[1];
        list.Remove(texture2D);
        list.Add(texture2D);
        return list[filterId];
    }

    public List<FilterInfo> GetFilters()
    {
        var list = new List<FilterInfo>();
        for (int i = 0; i < m_filters.Count; i++)
        {
            list.Add(new FilterInfo(m_filters[i], i));
        }
        var filterInfo = list[1];
        list.Remove(filterInfo);
        list.Add(filterInfo);
        return list;
    }
}


