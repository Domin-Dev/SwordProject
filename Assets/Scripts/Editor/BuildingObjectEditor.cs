using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

[CustomEditor(typeof(BuildingObject),false)]
public class BuildingObjectEditor : Editor
{

    BuildingObject buildingObject;
    public override void OnInspectorGUI()
    {
        buildingObject = target as BuildingObject;

        if (GUILayout.Button("Cut Sprites"))
        {
            CutSprites(buildingObject.texture);
        }
        base.OnInspectorGUI();
    }

    private void CutSprites(Texture2D texture)
    {
        int k = texture.width / 27;
        List<Sprite> sprites = new List<Sprite>();

        for (int i = 0; i < k; i++)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(i*27,0,27,51),Vector2.zero);
            sprites.Add(sprite);
            AssetDatabase.CreateAsset(sprite, $"Assets/Graphics/Sprites/BuildingObjets/{buildingObject.name}_{i}.asset");
        }
        AssetDatabase.SaveAssets();
        buildingObject.sprites = sprites.ToArray();
    }


    


}
