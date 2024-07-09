
using UnityEngine;
public class GridTile: IGetBarValue
{
    public int tileID;
    public int borders;
    public GridObject gridObject;


    public int x, y;
    public Grid<GridTile> grid;

    public enum TileType
    {

    };
    public GridTile(int x, int y, Grid<GridTile> grid)
    {
        this.tileID = -1;
        this.borders = 0;
        this.x = x;
        this.y = y;
        this.grid = grid;
    }
    public void ChangeTileType(int tileID)
    {
        this.tileID = tileID;
        grid.TObjectChanged(x, y);
    }
    public void ChangeGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
        grid.TObjectChanged(x, y);
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
        return tileID.ToString();
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
           var obj = grid.GetValueByXY(new Vector2(x,y) + MyTools.directions8[i]);
           if(obj != null && obj.gridObject != null && obj.gridObject.objectTransform != null) neighbors[i] = obj;
        }
        return neighbors;
    }
}