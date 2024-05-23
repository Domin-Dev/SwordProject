using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(Floor),true)]
public class FloorEditor : Editor
{

    Floor floor;

    public override void OnInspectorGUI()
    {
        //ItemEditor.IconField(target);
        //floor = target as Floor;

        //if (GUILayout.Button("Cut Sprites"))
        //{
        //    CutSpritesFloor(floor.texture);
        //}
        base.OnInspectorGUI();
    }


    private void CutSpritesFloor(Texture2D texture)
    {
        int k = texture.width / 25;
        List<Sprite> objectVariants = new List<Sprite>();
        if (!AssetDatabase.IsValidFolder($"{MyTools.spritesPath}/{floor.name}"))
        {
            AssetDatabase.CreateFolder($"{MyTools.spritesPath}", floor.name);
        }

        for (int i = 0; i < k; i++)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(i * 25, 0, 25, 25), new Vector2(0.5f,0.5f));
            objectVariants.Add(sprite);
            AssetDatabase.CreateAsset(sprite, $"{MyTools.spritesPath}/{floor.name}/{floor.name}_{i}.asset");
        }
        AssetDatabase.SaveAssets();
       // floor.sprites = objectVariants.ToArray();
    }
}
