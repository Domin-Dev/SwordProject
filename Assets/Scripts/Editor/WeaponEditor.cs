using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Weapon),true)]
public class WeaponEditor : Editor
{
    Sprite hitbox;
    int width;
    float pixelSize;

    Color[] texture;
    bool [] checkedArray;
    List<Point> points;
    int index = 0;
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

        Vector2 middle = new Vector2((sprite.rect.width - 1)/2, (sprite.rect.height- 1)/2);
        texture = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width,(int)sprite.rect.height);
        for (int i = 0; i < texture.Length; i++)
        {
            Color color = texture[i];
            if (color.a == 1) 
            {
                if(color.g == 1)
                {
                    weapon.hitBoxPoints = GetPoints(i);
                    break;
                }
                if (color.r == 1)
                {
                    weapon.gripPoint = pixelSize *(GetVector(i) - middle);
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

    public class Point
    {
        public  Vector2 position;
        public bool[] neighbors;
        public bool toRemove;

        public Point(Vector2 position)
        {
            this.position = position;
            toRemove = false;
            neighbors = new bool[8];
        }
    }

    private Vector2[] GetPoints(int i)
    {
        points = new List<Point>();
        checkedArray = new bool[texture.Length];

        CheckPoint(i);
        for (int j = 0; j < points.Count; j++)
        {
            bool[] neighbors = points[j].neighbors;
            Vector2 pos = points[j].position;
            neighbors[0] = IsPoint(pos + new Vector2(0, 1));
            neighbors[1] = IsPoint(pos + new Vector2(0, -1));
            neighbors[2] = IsPoint(pos + new Vector2(1, 0));
            neighbors[3] = IsPoint(pos + new Vector2(-1,0));

            neighbors[4] = IsPoint(pos + new Vector2(1, 1));
            neighbors[5] = IsPoint(pos + new Vector2(1, -1));
            neighbors[6] = IsPoint(pos + new Vector2(-1,1));
            neighbors[7] = IsPoint(pos + new Vector2(-1,-1));
        }

        for (int k = 2; k < points.Count; k++)
        {
            Vector2 position = points[k].position;
            ReducePoints(k, position,new Vector2(0,1),0);
            ReducePoints(k, position,new Vector2(0,-1),1);
            ReducePoints(k, position,new Vector2(1,0),2);
            ReducePoints(k, position,new Vector2(-1,0),3);

            ReducePoints(k, position,new Vector2(1,1),4);
            ReducePoints(k, position,new Vector2(1,-1),5);
            ReducePoints(k, position,new Vector2(-1,1),6);
            ReducePoints(k, position,new Vector2(-1,-1),7);
        }

        points.RemoveAll(match => match.toRemove);

        Vector2[] array = new Vector2[points.Count];
        for (int k = 0; k < points.Count; k++)
        {
            array[k] = points[k].position;
        }

        return array;
    }

    private void ReducePoints(int index, Vector2 position,Vector2 offset,int neighborIndex)
    {
        int j = PointIndex(index, position + offset);
        if (j >= 0 && points[j].neighbors[neighborIndex])
        {
            points[j].toRemove = true;
        }
    }
    private int PointIndex(int j,Vector2 position)
    {
        for (int i = 0; i < j; i++)
        {
            if (points[i].position.y == position.y && points[i].position.x == position.x)
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsPoint(Vector2 position)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].position.y == position.y && points[i].position.x == position.x)
            {
                return true;
            }
        }
        return false;
    }
    private void CheckPoint(int i)
    {
        if (!checkedArray[i] && i < texture.Length && i > 0 && texture[i].a == 1f && texture[i].g == 1f)
        {
            Point point = new Point(GetVector(i));
            checkedArray[i] = true;

            
            points.Add(point);

            CheckPoint(i + 1);
            CheckPoint(i - 1);
            CheckPoint(i + width);
            CheckPoint(i - width);
        }
    }



}
