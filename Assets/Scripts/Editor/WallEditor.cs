using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(Wall),true)]
public class WallEditor : Editor
{
    Wall wall;
    public override void OnInspectorGUI()
    {
        ItemEditor.IconField(target);
        wall = target as Wall;

        if (GUILayout.Button("Cut Sprites"))
        {
            CutSpritesWall(wall.texture);
        }
        base.OnInspectorGUI();
    }

    private void CutSpritesWall(Texture2D texture)
    {
        int k = texture.width / 27;
        List<ObjectVariant> objectVariants = new List<ObjectVariant>();

        if(!AssetDatabase.IsValidFolder($"{MyTools.spritesPath}/{wall.name}"))
        {
            AssetDatabase.CreateFolder($"{MyTools.spritesPath}",wall.name);
        }

        for (int i = 0; i < k; i++)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(i*27,0,27,51),new Vector2(0.5f,1f/51f));
            Sprite hitbox = Sprite.Create(texture, new Rect(i*27,51,27,51),Vector2.zero);
            Cutter cutter = new Cutter(hitbox, sprite.pivot);
            objectVariants.Add(new ObjectVariant(cutter.CutHitBox(MyTools.hitboxColor),sprite));
           
            AssetDatabase.CreateAsset(sprite, $"{MyTools.spritesPath}/{wall.name}/{wall.name}_{i}.asset");
        }
        AssetDatabase.SaveAssets();
        wall._objectVariants = objectVariants.ToArray();
    }
}
