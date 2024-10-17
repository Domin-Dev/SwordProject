using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroEditor: MonoBehaviour
{
 
    [SerializeField] private Material colorSwapMaterial;
    [SerializeField] private GameObject colorToSelectPrefab;
    [Space]

    [SerializeField] private Transform UIEditor;
    [SerializeField] private Transform skinColors;
    [SerializeField] private Transform hairColors;
    [SerializeField] private Transform underwearColors;
    [SerializeField] private Switch hairSwitch;
    [SerializeField] private Button saveButton;

    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite unselected;

    private CharacterEditorSettings characterEditorSettings;
    [SerializeField] private CharacterSpriteController player;

    private Image skinColorSelected;
    private Image hairColorSelected;
    private Image underwearColorSelected;

    public static HeroEditor instance { private set; get; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        characterEditorSettings = Resources.Load<CharacterEditorSettings>("CharacterParts/CharacterEditorSettings");
    }
    private void Start()
    {
        saveButton.onClick.AddListener(() =>
        {
            UIEditor.gameObject.SetActive(false);
        });

        hairSwitch.SetUpSwitch(0, characterEditorSettings.hairstyles.Length, "Hairstyle");
        LoadColors();
    }
    private void LoadColors()
    {
        for (int i = 0; i < characterEditorSettings.skinColors.Length; i++)
        {
            Transform transform = Instantiate(colorToSelectPrefab, skinColors).transform;
            transform.GetComponent<Image>().color = characterEditorSettings.skinColors[i];
            transform.GetComponent<Button>().onClick.AddListener(() => 
            {
                Image selectedColor = transform.GetComponent<Image>();
                ChangeSkinColor(player, selectedColor.color);
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
                ChangeHairColor(player, selectedColor.color);
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
                ChangeUnderwearColor(player, selectedColor.color);
                SelectNew(selectedColor, ref underwearColorSelected);
            });
        }

        SetCharacterSpriteProperties(player, 1, 1, 1, 1);
        hairSwitch.OnChangedValue += ChangeHair;
    }

    
    private void ChangeHair(object sender, SwitchArgs e)
    {
        ChangeHair(player, e.newValue);
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



    public void SetCharacterSpriteProperties(CharacterSpriteController controller,int hair,int hairColor,int underwearColor,int skinColor)
    {
        ChangeHair(controller, hair);
        ChangeHairColor(controller, characterEditorSettings.hairColors[hairColor]);
        ChangeUnderwearColor(controller,characterEditorSettings.clothesColors[underwearColor]);
        ChangeSkinColor(controller, characterEditorSettings.skinColors[skinColor]);
    }

    public void ChangeHair(CharacterSpriteController CharacterSpriteController, int value)
    {
        CharacterSpriteController.hair.sprite = characterEditorSettings.hairstyles[value].sprites[0];
        CharacterSpriteController.hairstyleIndex = value;
    }
    public void ChangeHairColor(CharacterSpriteController characterSpriteController,Color color)
    {
        ChangeColor(characterSpriteController.hair, color);
        ChangeColor(characterSpriteController.eyes, color, 0.6f);
    }
    public void ChangeUnderwearColor(CharacterSpriteController characterSpriteController,Color color)
    {
        ChangeColor(characterSpriteController.underwear, color);
    }
    public void ChangeSkinColor(CharacterSpriteController characterSpriteController,Color color)
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

