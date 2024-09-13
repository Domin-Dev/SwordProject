using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField] public SpriteRenderer head;
    [SerializeField] public SpriteRenderer eyes;
    [SerializeField] public SpriteRenderer mouth;
    [SerializeField] public SpriteRenderer hair;

    [SerializeField] public SpriteRenderer handL;
    [SerializeField] public SpriteRenderer handR;

    [SerializeField] public SpriteRenderer body;
    [SerializeField] public SpriteRenderer underwear;

    [SerializeField] Sprite[] bodySprites;
    [SerializeField] Sprite[] headSprites;

    private CharacterEditorSettings characterEditorSettings;

    private Vector2 lastMoveDir = Vector2.zero;
    private Vector2 lastSightDir = Vector2.zero;

    public int hairstyleIndex = 1;
    private void Start()
    {
        characterEditorSettings = Resources.Load<CharacterEditorSettings>("CharacterParts/CharacterEditorSettings");  
    }
    public void UpdateSprite(Vector2 moveDir, Vector2 sightDir)
    {
        if (lastMoveDir != moveDir)
        {
            lastMoveDir = moveDir;
            ChangeBodySprites(moveDir);
        }

        if (lastMoveDir != sightDir)
        {
            lastSightDir = sightDir;
            ChangeHeadSprites(sightDir);
        }
    }

    private void ChangeBodySprites(Vector2 dir)
    {
        if (dir.x > 0) SetSpriteBody(2);
        else if (dir.x < 0) SetSpriteBody(3);

        if (dir.y > 0) SetSpriteBody(1);
        else if (dir.y < 0) SetSpriteBody(0);
    }

    private void ChangeHeadSprites(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle += 180;

        if (angle >= 20 && angle <= 160)SetSpriteHead(0);
        else if (angle > 160 && angle < 200)SetSpriteHead(2);
        else if (angle >= 200 && angle <= 340)SetSpriteHead(1);
        else SetSpriteHead(3); 
    }
    // indexs: 0 - down, 1 - up, 2 - rigth, 3 - left
    private void SetSpriteHead(int index)
    {
        if (index > 1)
        {
            head.sprite = characterEditorSettings.heads[1];
        }
        else
        {
            head.sprite = characterEditorSettings.heads[0];
        }
        eyes.sprite = characterEditorSettings.eyes[index];
        mouth.sprite = characterEditorSettings.mouth[index];
        hair.sprite = characterEditorSettings.hairstyles[hairstyleIndex].sprites[index];

        if (index == 1)
        {
            head.GetComponent<SortingGroup>().sortingOrder = 100;
            body.GetComponent<SortingGroup>().sortingOrder = 20;
        }
        else
        {
            head.GetComponent<SortingGroup>().sortingOrder = 5;
            body.GetComponent<SortingGroup>().sortingOrder = 0;
        }
        if (lastMoveDir == Vector2.zero) SetSpriteBody(index);
    }
    // indexs: 0 - down, 1 - up, 2 - rigth, 3 - left
    private void SetSpriteBody(int index)
    {
        if (index > 1)
        {
            body.sprite = characterEditorSettings.bodies[1];
        }
        else
        {
            body.sprite = characterEditorSettings.bodies[0];
        }
        underwear.sprite = characterEditorSettings.underwear[index];
    }
}
