using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Weapon),true)]
public class WeaponEditor : Editor
{
    Sprite hitbox;
    int width;
    float pixelSize;
    Vector2 middle;

    Color[] texture;
    public override void OnInspectorGUI()
    {
        Weapon weapon = target as Weapon;
        ItemEditor.IconField(weapon);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Weapon Image");
        weapon.weaponImage = (Sprite)EditorGUILayout.ObjectField(weapon.weaponImage, typeof(Sprite), false, GUILayout.Width(150), GUILayout.Height(150));
      
        EditorGUILayout.EndHorizontal();

        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Weapon HitBox to bake");
        hitbox = (Sprite)EditorGUILayout.ObjectField(hitbox, typeof(Sprite),false);
        EditorGUILayout.EndHorizontal();

        if (hitbox != null) CutHitBox(hitbox,weapon);
        base.OnInspectorGUI();
    }
    private void CutHitBox(Sprite sprite,Weapon weapon)
    {
        hitbox = null;
        pixelSize = 1.0f/sprite.pixelsPerUnit;
        width = (int)sprite.rect.width;
        bool hitBoxIsCut = false;
        weapon.gripPoint2 = new Vector2(-100, -100);
        

        middle = new Vector2((sprite.rect.width - 1)/2, (sprite.rect.height- 1)/2);
        texture = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width,(int)sprite.rect.height);
        
        for (int i = 0; i < texture.Length; i++)
        {
            Color color = texture[i];
            if (color.a == 1) 
            {
                if(color.g == 1 && !hitBoxIsCut)
                {
                    weapon.hitBoxPoints = GetPoints(i);
                    hitBoxIsCut = true;
                    break;
                }
                else if (color.r == 1)
                {
                    weapon.gripPoint1 = pixelSize *(GetVector(i) - middle);
                }
                else if(color.b == 1)
                {
                    weapon.gripPoint2 = pixelSize * (GetVector(i) - middle);
                }
            }
        }

        
    }
    private Vector2 GetVector(int i)
    {
        return new Vector2(i % width, i / width);
    }
    private int GetInt(Vector2 position)
    { 
        return (int)position.x + (int)position.y * width;
    }
    private Vector2[] GetPoints(int i)
    {   
        List<Vector2> pointsToReturn = new List<Vector2>();

        Vector2 startPosition = GetVector(i);
        Vector2 position = startPosition;

        pointsToReturn.Add(pixelSize * (startPosition - middle));

        while (true)
        {
            position = DrawLine(position);
            if (position == startPosition)
            {
                break;
            }
            else
            {
                pointsToReturn.Add(pixelSize * (position - middle));
            }
        }

        return pointsToReturn.ToArray();
    }
    private Vector2 DrawLine(Vector2 position)
    {
        Vector2 last, current = new Vector2(-1,-1);

        int dir = GetDirection(GetNeighbors(position));
        int k;   
        if(dir == 0) k = 7; else k = dir - 1;
        last = position;

        while(true) 
        {

            Vector2 pos = last + MyTools.Directions[dir];
            if(IsHitBox(pos))
            {
                current = pos;
                if((dir % 2 == 1 && IsHitBox(current + MyTools.Directions[k])) || (IsHitBox(last + MyTools.Directions[k]) && IsHitBox(current + MyTools.Directions[k])))
                {
                    break;
                } 
            }
            else
            {
                break;
            }
            last = pos;
        }

        if(current.x < 0f) 
        {       
            return last;
        }
        else
        {
            return current;
        }
    }
    private bool IsHitBox(Vector2 vector2)
    {
        int i = GetInt(vector2);
        if (i < texture.Length && i > 0 && texture[i].a == 1f && texture[i].g == 1f)
            return true;  
        else
            return false;
    }
    private int GetDirection(bool[] neighbors)
    {
        bool last = neighbors[0];
        for (int i = 1; i < 8; i++)
        {
            if (!last && neighbors[i])
            {
                return i;
            }
            last = neighbors[i];
        }

        if (neighbors[0] && !neighbors[7]) 
        {
            return 0;
        }

        return -1;
    }
    private bool[] GetNeighbors(Vector2 pos)
    {
        bool[] neighbors = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            neighbors[i] = IsHitBox(pos + MyTools.Directions[i]);
        }
        return neighbors;
    }

}
