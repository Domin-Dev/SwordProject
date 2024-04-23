using TMPro;
using UnityEngine;

public static class MyTools 
{
    public static Vector2[] Directions = {
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(1, -1),
        new Vector2(0, -1),
        new Vector2(-1,-1), 
        new Vector2(-1,0),
        new Vector2(-1,1)
    };
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 0;
        pos = Camera.main.ScreenToWorldPoint(pos);
        return pos;
    }
    public static TextMesh CreateText(string text,int fontSize,Vector2 pos,Transform parent,Color color)
    {
        TextMesh textMesh = new GameObject("Text",typeof(TextMesh)).GetComponent<TextMesh>();
        textMesh.transform.position = pos;
        textMesh.transform.parent = parent;
        textMesh.text = text;
        textMesh.characterSize = 0.05f;
        textMesh.fontSize = fontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = color;
        return textMesh;
    }


}
