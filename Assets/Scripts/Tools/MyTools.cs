using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Rendering;
using System.Collections.Generic;

public static class MyTools 
{

    public readonly static Color hitboxColor = new Color(0, 1, 0, 1);
    public readonly static string spritesPath = "Assets/Graphics/Sprites/BuildingObjets";
    public readonly static Vector2[] directions8 = {
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(1, -1),
        new Vector2(0, -1),
        new Vector2(-1,-1), 
        new Vector2(-1,0),
        new Vector2(-1,1)
    };
    public readonly static Vector2[] directions4 = {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0,-1),
        new Vector2(-1,0),
    };
    public readonly static Vector2[] diagonalDirections = {
        new Vector2(1, 1),
        new Vector2(1, -1),
        new Vector2(-1,-1),
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
