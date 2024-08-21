using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CharacterEditorSettings", menuName = "GameAsset/CharacterEditorSettings")]
public class CharacterEditorSettings : ScriptableObject
{
    [SerializeField] public Color[] skinColors;
    [SerializeField] public Color[] hairColors;
    [SerializeField] public Color[] clothesColors;


    [SerializeField] public Texture2D hairstylesTexture;
    [SerializeField] public States[] hairstyles;
}

[System.Serializable]
public class States
{
    public Sprite[] sprites = new Sprite[4];
}
