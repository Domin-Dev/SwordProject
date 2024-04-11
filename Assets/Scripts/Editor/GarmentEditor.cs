using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Garment),false)]
public class GarmentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Garment item = (Garment)target;

        ItemEditor.IconField(item);


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Garment Images");

        EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("Top");
            item.top = (Sprite)EditorGUILayout.ObjectField(item.top, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.PrefixLabel("Rigth");
            item.rigth = (Sprite)EditorGUILayout.ObjectField(item.rigth, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("Bottom");
            item.bottom = (Sprite)EditorGUILayout.ObjectField(item.bottom, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.PrefixLabel("Left");
            item.left = (Sprite)EditorGUILayout.ObjectField(item.left, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
       
        base.OnInspectorGUI();
    }
}
