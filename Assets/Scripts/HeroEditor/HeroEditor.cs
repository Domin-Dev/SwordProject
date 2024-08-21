using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private Transform underwearColors;

    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite unselected;


    private CharacterEditorSettings characterEditorSettings;
    private CharacterSpriteController characterSpriteController;

    private Image skinColorSelected;
    private Image hairColorSelected;
    private Image underwearColorSelected;

    private void Start()
    {
        characterSpriteController = FindObjectOfType<CharacterSpriteController>();
        colorPicker.newColor += NewColor;
        characterEditorSettings = Resources.Load<CharacterEditorSettings>("CharacterParts/CharacterEditorSettings");
        LoadColors();

    }
    private void LoadColors()
    {
        ChangeSkinColor(characterEditorSettings.skinColors[1]);
        ChangeHairColor(characterEditorSettings.hairColors[1]);
        ChangeUnderwearColor(characterEditorSettings.clothesColors[1]);
        for (int i = 0; i < characterEditorSettings.skinColors.Length; i++)
        {
            Transform transform = Instantiate(colorToSelectPrefab, skinColors).transform;
            transform.GetComponent<Image>().color = characterEditorSettings.skinColors[i];
            transform.GetComponent<Button>().onClick.AddListener(() => 
            {
                Image selectedColor = transform.GetComponent<Image>();
                ChangeSkinColor(selectedColor.color);
                SelectNew(selectedColor, ref skinColorSelected);
            });
        }

        for (int i = 0; i < characterEditorSettings.hairColors.Length; i++)
        {
            Transform transform = Instantiate(colorToSelectPrefab,hairColors).transform;
            transform.GetComponent<Image>().color = characterEditorSettings.hairColors[i];
            transform.GetComponent<Button>().onClick.AddListener(() => 
            {
                Image selectedColor = transform.GetComponent<Image>();
                ChangeHairColor(selectedColor.color);
                SelectNew(selectedColor, ref hairColorSelected);
            });
        }

        for (int i = 0; i < characterEditorSettings.clothesColors.Length; i++)
        {
            Transform transform = Instantiate(colorToSelectPrefab, underwearColors).transform;
            transform.GetComponent<Image>().color = characterEditorSettings.clothesColors[i];
            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                Image selectedColor = transform.GetComponent<Image>();
                ChangeUnderwearColor(selectedColor.color);
                SelectNew(selectedColor, ref underwearColorSelected);
            });
        }
    }

    private void NewColor(object sender, ColorArgs e)
    {
        characterSpriteController.head.material = colorSwapMaterial;
        colorSwapMaterial.color = e.color;
    }

    private void SelectNew(Image newSelected,ref Image currentSelected)
    {
        if(currentSelected != null)
        {
            currentSelected.sprite = unselected;
        }
        Image border = newSelected.transform.GetChild(0).GetComponent<Image>();
        currentSelected = border;
        border.sprite = selected;
    }

    private void ChangeHairColor(Color color)
    {
        ChangeColor(characterSpriteController.hair, color);
        ChangeColor(characterSpriteController.eyes, color, 0.6f);
    }

    private void ChangeUnderwearColor(Color color)
    {
        ChangeColor(characterSpriteController.underwear, color);
    }
    private void ChangeSkinColor(Color color)
    {
        ChangeColor(characterSpriteController.head, color);
        ChangeColor(characterSpriteController.handL, color);
        ChangeColor(characterSpriteController.handR, color);
        ChangeColor(characterSpriteController.body, color);
    }
    private void ChangeColor(SpriteRenderer spriteRenderer,Color color,float darkValue = 1f)
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_Color", color);
        materialPropertyBlock.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        materialPropertyBlock.SetFloat("_DarkValue", darkValue);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

}

