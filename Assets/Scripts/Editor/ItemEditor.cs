using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(Item),true)]
public class ItemEditor : Editor
{
 
    public override void OnInspectorGUI()
    {
        IconField(target);
        if (GUILayout.Button("siema", EditorStyles.popup))
        {
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ItemFinder());
        }
        base.OnInspectorGUI();
    }

    public static void IconField(Object target)
    {
        Item item = (Item)target;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Icon Image");
        item.icon = (Sprite)EditorGUILayout.ObjectField(item.icon, typeof(Sprite), false, GUILayout.Width(150), GUILayout.Height(150));

        EditorGUILayout.EndHorizontal();
    }

}
