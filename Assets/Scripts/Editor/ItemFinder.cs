using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ItemFinder : ScriptableObject, ISearchWindowProvider
{
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry > result = new List<SearchTreeEntry>();
        result.Add(new SearchTreeGroupEntry(new GUIContent("List"),0));
        return result;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        return true;
    }
}
