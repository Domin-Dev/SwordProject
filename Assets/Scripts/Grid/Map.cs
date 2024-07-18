using UnityEngine;
using System.Collections.Generic;

public class Map
{
    public Vector2 offset { private set;  get; }
    public float cellSize { private set;  get; }
    public float chunkSize { private set;  get; }
    public int widthInChunks { private set; get; }
    public int heightInChunks { private set; get; }

    public Dictionary<int, Chunk> chunks;
   

    public Map(Vector2 offset, float cellSize, float chunkSize,int widthInChunks, int heightInChunks)
    {
        chunks = new Dictionary<int, Chunk>();
        this.offset = offset;
        this.cellSize = cellSize;  
        this.chunkSize = chunkSize;
        this.widthInChunks = widthInChunks;
        this.heightInChunks = heightInChunks;
    }
}

