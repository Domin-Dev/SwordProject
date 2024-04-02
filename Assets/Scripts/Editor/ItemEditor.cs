using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item),true)]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {

    //   //  GUI.DrawTexture(Rect.zero, serializedObject.FindProperty("icon"));
    //    var k == serializedObject.FindProperty("icon").;
 //       Texture texture = (serializedObject.CopyFromSerializedProperty(serializedObject.FindProperty("icon")) as SpriteRenderer).material.mainTexture;
     //   GUILayout.Box(texture);

        base.OnInspectorGUI();
    }
}
