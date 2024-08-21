using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterEditorSettings), true)]

public class CharacterEditorSettingsEditor : Editor
{
    private const int hairstyleWidth = 15;
    private const int hairstyleHeight = 15;

    CharacterEditorSettings settings;
    public override void OnInspectorGUI()
    {
        settings = (CharacterEditorSettings)target;
        if (GUILayout.Button("Cut Sprites"))
        {
           CutSprites(settings.hairstylesTexture);
        }
        base.OnInspectorGUI();
    }
    private void CutSprites(Texture2D texture)
    {
        int h = texture.height / hairstyleHeight;
        List<States> statesList = new List<States>();
        Debug.Log(h);
        for (int i = 0; i < h; i++)
        {
            if (!AssetDatabase.IsValidFolder($"{MyTools.hairstylesSpritesPath}/{i}"))
            {
                AssetDatabase.CreateFolder($"{MyTools.hairstylesSpritesPath}",i.ToString());
            }
            Debug.Log("dziala");
            States states = new States();
            for (int j = 0; j < 4; j++)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(j * hairstyleWidth,i * hairstyleWidth, hairstyleWidth, hairstyleHeight), new Vector2(0.5f, 0.5f));
                AssetDatabase.CreateAsset(sprite, $"{MyTools.hairstylesSpritesPath}/{i}/Hairstyle{j}_{i}.asset");
                states.sprites[j] = sprite;
                Debug.Log("dziala");
            }
            statesList.Add(states);
        }
        AssetDatabase.SaveAssets();
        settings.hairstyles = statesList.ToArray();
    }
}
