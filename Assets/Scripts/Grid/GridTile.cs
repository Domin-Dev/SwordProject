
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
    public int x, y;
    public Grid<GridTile> grid;

    public GridTile(int x, int y, Grid<GridTile> grid)
    {
        tile = TileType.Grass;
        this.x = x;
        this.y = y;
        this.grid = grid;
    }
    public void ChangeValue(TileType tile)
    {
        this.tile = tile;
        grid.TObjectChanged(x, y);
    }

    public override string ToString()
    {
        return tile.ToString();
    }

}