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
        List<ObjectVariant> objectVariants = new List<ObjectVariant>();

        if(!AssetDatabase.IsValidFolder($"{MyTools.spritesPath}/{variantItem.name}"))
        {
            AssetDatabase.CreateFolder($"{MyTools.spritesPath}", variantItem.name);
        }

        for (int i = 0; i < k; i++)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(i*27,0,27,51),new Vector2(0.5f,1f/51f));
            Sprite hitbox = Sprite.Create(texture, new Rect(i*27,51,27,51),Vector2.zero);
            Cutter cutter = new Cutter(hitbox, sprite.pivot);
            objectVariants.Add(new ObjectVariant(cutter.CutHitBox(MyTools.hitboxColor),sprite));
           
            AssetDatabase.CreateAsset(sprite, $"{MyTools.spritesPath}/{variantItem.name}/{variantItem.name}_{i}.asset");
        }


        variantItem.objectVariants = objectVariants.ToArray();
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(variantItem);
    }

    private  void NewSaveChanges()
    { 
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(variantItem); 
    }
}
