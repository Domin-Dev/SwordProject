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
    [Space]
    [SerializeField] public Sprite[] bodies;
    [SerializeField] public Sprite[] heads;
    [SerializeField] public Sprite[] eyes;
    [SerializeField] public Sprite[] mouth;

    [SerializeField] public Sprite[] underwear;
}

[System.Serializable]
public class States
{
    public Sprite[] sprites = new Sprite[4];
}
