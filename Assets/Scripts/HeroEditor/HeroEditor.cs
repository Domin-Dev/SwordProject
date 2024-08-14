using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroEditor: MonoBehaviour
{
    [SerializeField] CharacterSpriteController characterSpriteController;
    [SerializeField] Material colorSwapMaterial;
    [SerializeField] ColorPicker colorPicker;

    private void Start()
    {
        characterSpriteController = FindObjectOfType<CharacterSpriteController>();
        colorPicker.newColor += NewColor;

    }

    private void NewColor(object sender, ColorArgs e)
    {
        characterSpriteController.head.material = colorSwapMaterial;
        colorSwapMaterial.color = e.color;
    }
}

