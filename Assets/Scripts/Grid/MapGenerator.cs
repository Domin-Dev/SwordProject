using System;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Seed Settings")]
    [SerializeField] private int seed;
    [SerializeField] private bool randomSeed;
    [Header("Map Size( in chunks )")]
    [SerializeField] private int widthInChunks = 256;
    [SerializeField] private int heightInChunks = 256;
    [Header("Map Generator Settings")]
    [SerializeField] private float scale = 20;
    [SerializeField] private float offsetX = 100f;
    [SerializeField] private float offsetY = 100f;
    [Header("Chunk Settings")]
    [SerializeField] private int chunkSize = 10;

    private static readonly Vector2 offset = new Vector2(-1,-1); 

    private MapGeneratorSettings mapGeneratorSettings;
    GridVisualization gridVisualization;
    Grid<GridTile> grid;


    private void Awake()
    {
        if(randomSeed)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
    }
    private void Start()
    {
        gridVisualization = GridVisualization.instance;
        mapGeneratorSettings = gridVisualization.settings;

        var rand = new System.Random(seed);
        offsetX = rand.Next(-100000, 100000);
        offsetY = rand.Next(-100000, 100000);

        var map = GenerateMap(0.25f, offset);
        gridVisualization.SetMap(map); 
    }

    private void SetValue(Chunk chunk ,int x,int y,int index)
    {
        chunk.grid[x,y].tileID = mapGeneratorSettings.tiles[index].tileID;
    }
    
    private void SetBuildingObject(Chunk chunk, int x, int y,int index)
    {
        chunk.grid[x, y].gridObject = new GridObject(index, 0, null);
    }

    public Map GenerateMap(float cellSize, Vector2 offset)
    {
        Map map = new Map(offset, cellSize,chunkSize,widthInChunks,heightInChunks);

        for (int y = 0; y < heightInChunks; y++)
        {
            for (int x = 0; x < widthInChunks; x++)
            {
                map.chunks.Add(x + y * widthInChunks,new Chunk(chunkSize,new Vector2(x * chunkSize,y * chunkSize), offset + new Vector2(x * chunkSize * cellSize, y * chunkSize * cellSize)));
            }
        }

        var rand = new System.Random(seed);

        foreach (var item in map.chunks)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float value = Generate(x + (int)item.Value.ChunkGridPosition.x, y + (int)item.Value.ChunkGridPosition.y);
                    if (value > 0.65)
                    {
                        SetValue(item.Value,x, y, 0);
                    }
                    else if (value > 0.3)
                    {
                        SetValue(item.Value,x, y, 1);
                    }
                    else
                    {
                        SetValue(item.Value, x, y, 2);
                    }
                    
                    if(rand.Next(0,100) <= 5)
                    {
                        SetBuildingObject(item.Value, x, y, 300);
                    }
                }
            }
        }
        return map;
    }

    private float Generate(int x,int y)
    {
        float xf = ((float)x  + offsetX )/ chunkSize * scale;
        float yf = ((float)y + offsetY) / chunkSize * scale;
        float value = Mathf.PerlinNoise(xf,yf);
        return value;
    }


}
