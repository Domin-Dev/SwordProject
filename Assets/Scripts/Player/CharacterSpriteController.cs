using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] Sprite[] bodySprites;
    [SerializeField] Sprite[] headSprites;


    private Vector2 lastMoveDir = Vector2.zero;
    private Vector2 lastSightDir = Vector2.zero;   
    public void UpdateSprite(Vector2 moveDir, Vector2 sightDir)
    {
        if(lastMoveDir != moveDir)
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

    private void SetSpriteHead(int index)
    {
        head.sprite = headSprites[index];
        if (index == 1)
        {
            head.sortingOrder = 100;
            body.sortingOrder = 30;
        }
        else
        {
            head.sortingOrder = 5;
            body.sortingOrder = 0;
        }
        if (lastMoveDir == Vector2.zero) SetSpriteBody(index);
    }

    private void SetSpriteBody(int index)
    {
        body.sprite = bodySprites[index];
    }
}
