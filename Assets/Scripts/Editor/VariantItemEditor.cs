using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(VariantItem),true)]
public class VariantItemEditor : ItemEditor
{
    VariantItem variantItem;
    static readonly Color particlePointColor = new Color(1, 0, 0, 1);


    private void OnEnable()
    {
        variantItem = target as VariantItem;
    }
    public override void OnInspectorGUI()
    {
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

        if(!AssetDatabase.IsValidFolder($"{MyTools.buildingObjectsSpritesPath}/{variantItem.name}"))
        {
            AssetDatabase.CreateFolder($"{MyTools.buildingObjectsSpritesPath}", variantItem.name);
        }

        Cut(texture,objectVariants, k,h/2);

        variantItem.objectVariants = objectVariants.ToArray();
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(variantItem);
    }


    private void Cut(Texture2D texture, List<ObjectVariant> objectVariants, int k,int numberVariant)
    {
        Color[] pointsColor = { particlePointColor };

        for (int i = 0; i < k; i++)
        {
            List<Variant> variants = new List<Variant>();
            for (int j = 0; j < numberVariant; j++)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(i * 27, j * 102, 27, 51), new Vector2(0.5f, 1f / 51f));
                Sprite hitbox = Sprite.Create(texture, new Rect(i * 27, j * 102 + 51, 27, 51), Vector2.zero);
                Cutter cutter = new Cutter(hitbox, sprite.pivot);
                Vector2?[] points = cutter.GetPoints(pointsColor,MyTools.hitboxColor);
                Vector2[] hitboxArray = cutter.CutHitBox(MyTools.hitboxColor);
                variants.Add(new Variant(hitboxArray, sprite, hitboxArray[0].y, points[0].Value));
            }

            objectVariants.Add(new ObjectVariant(variants.ToArray()));

            for (int j = 0; j < numberVariant; j++)
            {
                AssetDatabase.CreateAsset(variants[j].sprite, $"{MyTools.buildingObjectsSpritesPath}/{variantItem.name}/{variantItem.name}_{i+k*j}.asset");
            }
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
