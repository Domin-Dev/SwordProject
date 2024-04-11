using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

[CustomEditor(typeof(Item),true)]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        IconField(target);
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
