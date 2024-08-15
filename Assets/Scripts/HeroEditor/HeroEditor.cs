using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroEditor: MonoBehaviour
{
 
    [SerializeField] private Material colorSwapMaterial;
    [SerializeField] private ColorPicker colorPicker;
    [SerializeField] private GameObject colorToSelectPrefab;
    [Space]
    [SerializeField] private Transform skinColors;
    [SerializeField] private Transform hairColors;

    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite unselected;


    private CharacterEditorSettings characterEditorSettings;
    private CharacterSpriteController characterSpriteController;

    private void Start()
    {
        characterSpriteController = FindObjectOfType<CharacterSpriteController>();
        colorPicker.newColor += NewColor;
        characterEditorSettings = Resources.Load<CharacterEditorSettings>("CharacterParts/CharacterEditorSettings");
        LoadSkinColors();

    }
    private void LoadSkinColors()
    {
        for (int i = 0; i < characterEditorSettings.skinColors.Length; i++)
        {
            Transform transform = Instantiate(colorToSelectPrefab, skinColors).transform;
            transform.GetComponent<Image>().color = characterEditorSettings.skinColors[i];
            transform.GetComponent<Button>().onClick.AddListener(() => { ChangeSkinColor(transform.GetComponent<Image>().color); });
        }

        for (int i = 0; i < characterEditorSettings.hairColors.Length; i++)
        {
            Transform transform = Instantiate(colorToSelectPrefab,hairColors).transform;
            transform.GetComponent<Image>().color = characterEditorSettings.hairColors[i];
            transform.GetComponent<Button>().onClick.AddListener(() => { ChangeColor(characterSpriteController.hair,transform.GetComponent<Image>().color); });
        }
    }

    private void NewColor(object sender, ColorArgs e)
    {
        characterSpriteController.head.material = colorSwapMaterial;
        colorSwapMaterial.color = e.color;
    }

    private void ChangeSkinColor(Color color)
    {
        ChangeColor(characterSpriteController.head, color);
        ChangeColor(characterSpriteController.handL, color);
        ChangeColor(characterSpriteController.handR, color);
    }
    private void ChangeColor(SpriteRenderer spriteRenderer,Color color)
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_Color", color);
        materialPropertyBlock.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

}

