using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class ItemFinder : ScriptableObject, ISearchWindowProvider
{

    private Action<int> action;

    public ItemFinder(Action<int> action)
    {
        this.action = action;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry > result = new List<SearchTreeEntry>();
        result.Add(new SearchTreeGroupEntry(new GUIContent("Item"), 0));

        var items = ItemList.items;

        foreach (var item in items)
        {
            SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent($"{item.Value.name} [ID:{item.Key}]"));
            searchTreeEntry.userData = item.Key;
            searchTreeEntry.level = 1;
            result.Add(searchTreeEntry);
        }
     


        return result;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        action.Invoke((int)SearchTreeEntry.userData);
        return true;
    }
}
