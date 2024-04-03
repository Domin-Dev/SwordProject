using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

[CustomEditor(typeof(Item),true)]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Item item = (Item)target;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Source Image");
        item.icon = (Sprite)EditorGUILayout.ObjectField(item.icon, typeof(Sprite),false, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
