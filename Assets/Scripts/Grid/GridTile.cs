﻿
using UnityEngine;
[System.Serializable]
public class GridTile: IGetBarValue
{
    
    public int tileID;
    public int borders;
    public GridObject gridObject { private set; get; }
    public int x, y;

    //pathfinding
    public int gCost;
    public int hCost;
    public int fCost;

    public GridTile cameFrom;

    public bool isWalkable;
    public enum TileType
    {

    };
    public GridTile(int x, int y)
    {
        this.tileID = -1;
        this.borders = 0;
        this.x = x;
        this.y = y;
        ResetNode();
        this.isWalkable = true;
    }

    public void SetGridObject(GridObject gridObject, bool isWalkable = false)
    {
        this.gridObject = gridObject;
        this.isWalkable = isWalkable;
    }

    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }

    public void ResetNode()
    {
        gCost = int.MaxValue;
        cameFrom = null;
        CalculateFCost();
    }

    public void ChangeTileType(int tileID)
    {
        this.tileID = tileID;
        GridVisualization.instance.TileChanged(x, y);
    }
    public void ChangeGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
        GridVisualization.instance.TileChanged(x, y);
    }
    public bool IsBuildObject()
    {
        return gridObject != null;
    }
    public bool IsBuildObject(int id)
    {  
        return IsBuildObject() && gridObject.ID == id; 
    }
    public override string ToString()
    {
        return $"Position : [{x},{y}]";
    }
    public float GetBarValue()
    {
       return gridObject.GetBarValue();
    }
    public void IncreaseHitPoints(float value)
    {
       gridObject.IncreaseHitPoints(value);       
    }
    public bool DecreaseHitPoints(float value)
    {
        if(!gridObject.DecreaseHitPoints(value))
        {
            GridVisualization.instance.DestroyObject(this);
            return false;
        }
        return true;
    }
    public GridTile[] GetNeighbors()
    {
        GridTile[] neighbors = new GridTile[8];
        for (int i = 0; i < 8; i++)
        {
           var obj = GridVisualization.instance.GetValueByGridPosition(new Vector2(x,y) + MyTools.directions8[i]);
           if(obj != null && obj.gridObject != null && obj.gridObject.objectTransform != null) neighbors[i] = obj;
        }
        return neighbors;
    }

    
}