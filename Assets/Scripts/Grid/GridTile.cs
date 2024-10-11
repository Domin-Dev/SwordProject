
using UnityEngine;
public class GridTile: IGetBarValue
{
    public int tileID;
    public int borders;
    public GridObject gridObject;
    public int x, y;

    public enum TileType
    {

    };
    public GridTile(int x, int y)
    {
        this.tileID = -1;
        this.borders = 0;
        this.x = x;
        this.y = y;
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