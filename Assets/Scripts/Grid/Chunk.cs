using UnityEngine;

public class Chunk
{
    public GridTile[,] grid;
    public Vector2 ChunkCoordinates {  set; get; }
    public Vector2 position {  set; get; }

    public Chunk(int gridSize,Vector2 chunkCoordinates, Vector2 position)
    {
        this.grid = new GridTile[gridSize,gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = new GridTile((int)chunkCoordinates.x + i,(int)chunkCoordinates.y + j);
            }
        }
        this.ChunkCoordinates = chunkCoordinates;
        this.position = position;
    }
}

