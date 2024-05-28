using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(VariantItem),true)]
public class VariantItemEditor : Editor
{
    VariantItem variantItem;
    SerializedProperty serializedProperty;
    private void OnEnable()
    {
        variantItem = target as VariantItem;
        serializedProperty = serializedObject.FindProperty("objectVariants");
    }
    public override void OnInspectorGUI()
    {
        ItemEditor.IconField(target);

        if (GUILayout.Button("Cut Sprites"))
        {
            CutSpritesWall(variantItem.texture);
            NewSaveChanges();
        }

        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    private void CutSpritesWall(Texture2D texture)
    {
        int k = texture.width / 27;
        int h = texture.height / 51;

        List<ObjectVariant> objectVariants = new List<ObjectVariant>();

        if(!AssetDatabase.IsValidFolder($"{MyTools.spritesPath}/{variantItem.name}"))
        {
            AssetDatabase.CreateFolder($"{MyTools.spritesPath}", variantItem.name);
        }

        if (h == 2) Cut1(texture,objectVariants,k);
        else if (h == 3) Cut2(texture,objectVariants, k);


        variantItem.objectVariants = objectVariants.ToArray();
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(variantItem);

    }

    private void Cut1(Texture2D texture, List<ObjectVariant> objectVariants,int k)
    {
        for (int i = 0; i < k; i++)
        {
            Sprite[] sprites = new Sprite[1];
            sprites[0] = Sprite.Create(texture, new Rect(i * 27, 0, 27, 51), new Vector2(0.5f, 1f / 51f));
            Sprite hitbox = Sprite.Create(texture, new Rect(i * 27, 51, 27, 51), Vector2.zero);
            Cutter cutter = new Cutter(hitbox, sprites[0].pivot);
            Vector2[] hitboxArray = cutter.CutHitBox(MyTools.hitboxColor);
            objectVariants.Add(new ObjectVariant(hitboxArray,sprites, hitboxArray[0].y));

            AssetDatabase.CreateAsset(sprites[0], $"{MyTools.spritesPath}/{variantItem.name}/{variantItem.name}_{i}.asset");
        }
    }
    private void Cut2(Texture2D texture, List<ObjectVariant> objectVariants, int k)
    {
        for (int i = 0; i < k; i++)
        {
            Sprite[] sprites = new Sprite[2];
            sprites[0] = Sprite.Create(texture, new Rect(i * 27, 0, 27, 51), new Vector2(0.5f, 1f / 51f));
            sprites[1] = Sprite.Create(texture, new Rect(i * 27, 102, 27, 51), new Vector2(0.5f, 1f / 51f));
            Sprite hitbox = Sprite.Create(texture, new Rect(i * 27, 51, 27, 51), Vector2.zero);
            Cutter cutter = new Cutter(hitbox, sprites[0].pivot);
            Vector2[] hitboxArray = cutter.CutHitBox(MyTools.hitboxColor);
            objectVariants.Add(new ObjectVariant(hitboxArray,sprites, hitboxArray[0].y));

            AssetDatabase.CreateAsset(sprites[0], $"{MyTools.spritesPath}/{variantItem.name}/{variantItem.name}_{i}.asset");
            AssetDatabase.CreateAsset(sprites[1], $"{MyTools.spritesPath}/{variantItem.name}/{variantItem.name}_{i+k}.asset");
        }
    }

    private  void NewSaveChanges()
    { 
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(variantItem); 
    }
}
