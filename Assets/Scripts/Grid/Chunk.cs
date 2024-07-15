using UnityEngine;

public class Chunk
{
    public Grid<GridTile> tiles;
    public Vector2 position {  set; get; }


    public Chunk(Grid<GridTile> tiles, Vector2 position)
    {
        this.tiles = tiles;
        this.position = position;
    }
}

