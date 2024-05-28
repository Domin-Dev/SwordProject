using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    public static Grid<GridTile> grid { private set; get; }
    public static Actions instance { private set; get; }
    public Grid<GridTile> _grid
    {
        set
        {
            if (grid == null)
            {
                grid = value;
            }
        }
    }

    Vector2 lastPos;

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
    }

    public static Vector2 GetMousePosXY()
    {
        return instance.lastPos;
    }
    private void Update()
    {
        Vector2 pos = grid.GetXY(MyTools.GetMouseWorldPosition());
        if(pos != lastPos)
        {
            lastPos = pos;

        }

        if(Input.GetMouseButtonDown(0))
        {
            GridObject gridObject = grid.GetValueByXY(pos).gridObject;
            if(gridObject != null)
            {
                Debug.Log(gridObject.GetType());
                BuildingManager.instance.ChangeSprite(pos, 1);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            GridObject gridObject = grid.GetValueByXY(pos).gridObject;
            if (gridObject != null)
            {
                Debug.Log(gridObject.GetType());
                BuildingManager.instance.ChangeSprite(pos, 0);
            }
        }
    }
}
