
public class GridTile
{
    public enum TileType
    {
        Grass,
        Sand,
        Water,
        Mud,
    };

    public TileType tile;
    public GridObject gridObject;

    public int x, y;
    public Grid<GridTile> grid;

    public GridTile(int x, int y, Grid<GridTile> grid)
    {
        tile = TileType.Grass;
        this.x = x;
        this.y = y;
        this.grid = grid;
    }
    public void ChangeTileType(TileType tile)
    {
        this.tile = tile;
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
        return tile.ToString();
    }

}