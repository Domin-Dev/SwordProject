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
    [SerializeField] private Vector2 offset;
    [Header("Temperature Map Settings")]
    [SerializeField] private float scaleTemp = 20;
    [SerializeField] private Vector2 offsetTemp;
    [Header("Rainfall Map Settings")]
    [SerializeField] private float scaleRain = 20;
    [SerializeField] private Vector2 offsetRain;
    [Header("Chunk Settings")]
    [SerializeField] private int chunkSize = 10;

    private static readonly Vector2 gridOffset = new Vector2(-1,-1); 

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
        offset.x = rand.Next(-100000, 100000);
        offset.y = rand.Next(-100000, 100000);

        offsetTemp.x = rand.Next(-100000, 100000);
        offsetTemp.y = rand.Next(-100000, 100000);

        offsetRain.x = rand.Next(-100000, 100000);
        offsetRain.y = rand.Next(-100000, 100000);

        var map = GenerateMap(0.25f, gridOffset);
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
                    GenerateTempCell(item.Value, x, y,rand);
                }
            }
        }
        return map;
    }

    private void GenerateCell(Chunk chunk, int x, int y, System.Random rand)
    {
        float value = Generate(x + (int)chunk.ChunkGridPosition.x, y + (int)chunk.ChunkGridPosition.y, offset, scale);
        if (value > 0.65)
        {
            SetValue(chunk, x, y, 0);
        }
        else if (value > 0.3)
        {
            SetValue(chunk, x, y, 1);
        }
        else
        {
            SetValue(chunk, x, y, 2);
        }

        if (rand.Next(0, 100) <= 5)
        {
            SetBuildingObject(chunk, x, y, 300);
        }
    }

    private void GenerateTempCell(Chunk chunk, int x, int y, System.Random rand)
    {
        float value = Generate(x + (int)chunk.ChunkGridPosition.x, y + (int)chunk.ChunkGridPosition.y,offsetRain,scaleRain);
        if (value >= 0.8f)
        {
            SetValue(chunk, x, y, 3);
        }
        else if (value >= 0.6f)
        {
            SetValue(chunk, x, y, 4);
        }
        else if (value >= 0.4f)
        {
            SetValue(chunk, x, y, 5);
        }
        else if (value >= 0.2f)
        {
            SetValue(chunk, x, y, 6);
        }
        else
        {
            SetValue(chunk, x, y, 7);
        }
    }

    private float Generate(int x, int y, Vector2 offset, float scale)
    {
        float xf = ((float)x  + offset.x )/ chunkSize * scale;
        float yf = ((float)y + offset.y) / chunkSize * scale;
        float value = Mathf.PerlinNoise(xf,yf);
        return value;
    }
}
