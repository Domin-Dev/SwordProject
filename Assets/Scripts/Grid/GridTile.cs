
public class GridTile
{
    public int tileID;
    public GridObject gridObject;

    public int x, y;
    public Grid<GridTile> grid;

    public enum TileType
    {

    };
    public GridTile(int x, int y, Grid<GridTile> grid)
    {
        this.tileID = -1;
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


    public override string ToString()
    {
        return tileID.ToString();
    }

}