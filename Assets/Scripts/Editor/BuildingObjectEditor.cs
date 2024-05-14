using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(Wall),false)]
public class BuildingObjectEditor : Editor
{

    Wall wall;
    public override void OnInspectorGUI()
    {
        wall = target as Wall;

        if (GUILayout.Button("Cut Sprites"))
        {
            CutSprites(wall.texture);
        }
        base.OnInspectorGUI();
    }

    private void CutSprites(Texture2D texture)
    {
        int k = texture.width / 27;
        List<Sprite> sprites = new List<Sprite>();

        for (int i = 0; i < k; i++)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(i*27,0,27,51),new Vector2(0.5f,1f/51f));
            sprites.Add(sprite);
            AssetDatabase.CreateAsset(sprite, $"Assets/Graphics/Sprites/BuildingObjets/{wall.name}_{i}.asset");
        }
        AssetDatabase.SaveAssets();
        wall.sprites = sprites.ToArray();
    }


    


}
