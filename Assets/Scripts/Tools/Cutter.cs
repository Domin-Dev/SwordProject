﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Cutter
{
    float pixelSize;
    int width;
    Vector2 middle;
    Color[] texture;


    public Cutter(Sprite sprite)
    {
        middle = new Vector2((sprite.rect.width - 1) / 2, (sprite.rect.height - 1) / 2);
        Setup(sprite);
    }
    public Cutter(Sprite sprite,Vector2 middle)
    {
        this.middle = middle - new Vector2(0.5f, 0.5f); 
        Setup(sprite);
    }

    private void Setup(Sprite sprite)
    {
        pixelSize = 1.0f / sprite.pixelsPerUnit;
        width = (int)sprite.rect.width;
        texture = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
    }
    public Vector2[] CutHitBox(Color hitboxColor)
    {
        for (int i = 0; i < texture.Length; i++)
        {
            Color color = texture[i];
            if (color.a == 1)
            {
                if (color == hitboxColor)
                {
                    return GetPoints(i);
                }  
            }
        }
        return null;
    }
    public Vector2?[] GetPoints(Color[] pointColors,Color hitboxColor)
    {
        Vector2?[] results = new Vector2?[pointColors.Length];
     
        for (int i = 0; i < texture.Length; i++)
        {
            Color color = texture[i];
            if (color.a == 1 && color != hitboxColor)
            {           
                for (int j = 0; j < pointColors.Length; j++)
                {
                    if (color == pointColors[j])
                    {
                        results[j] = GetPosition(i);
                    }
                }    
            }
        }
        return results;
    }

    private Vector2 GetPosition(int index)
    {
        return pixelSize * (GetVector(index) - middle);
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
        Vector2 last, current = new Vector2(-1, -1);
        int dir = GetDirection(GetNeighbors(position));
        int k;
        if (dir == 0) k = 7; else k = dir - 1;
        last = position;
        while (true)
        {
            Vector2 pos = last + MyTools.directions8[dir];
            if (IsHitBox(pos))
            {
                current = pos;
                if ((dir % 2 == 1 && IsHitBox(current + MyTools.directions8[k])) || (IsHitBox(last + MyTools.directions8[k]) && IsHitBox(current + MyTools.directions8[k])))
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

        if (current.x < 0f)
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
            neighbors[i] = IsHitBox(pos + MyTools.directions8[i]);
        }
        return neighbors;
    }
}
