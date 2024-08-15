using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CharacterEditorSettings", menuName = "GameAsset/CharacterEditorSettings")]
public class CharacterEditorSettings : ScriptableObject
{
    [SerializeField] public Color[] skinColors;
    [SerializeField] public Color[] hairColors;

    [SerializeField] public States[] eyes;
    [SerializeField] public States head;
}

[System.Serializable]
public class States
{
    public Sprite[] sprites = new Sprite[4];
}
