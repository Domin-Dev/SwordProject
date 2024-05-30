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

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        Item item = target as Item;
        var texture = new Texture2D(width, height);
        EditorUtility.CopySerialized(item.icon.texture,texture);
       
        return texture;
    }
}
