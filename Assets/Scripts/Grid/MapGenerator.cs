
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Map Size( in chunks )")]
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [Header("Map Generator Settings")]
    [SerializeField] private float scale = 20;
    [SerializeField] private float offsetX = 100f;
    [SerializeField] private float offsetY = 100f;
    [Header("Chunk Settings")]
    [SerializeField] private int chunkSize = 10;


    private MapGeneratorSettings mapGeneratorSettings;
    GridVisualization gridVisualization;
    Grid<GridTile> grid;

    Dictionary<int,Chunk> map;
    private void Start()
    {
        gridVisualization = GridVisualization.instance;
        mapGeneratorSettings = gridVisualization.settings;
        offsetX = Random.Range(0f,99999f);
        offsetY = Random.Range(0f,99999f);
        GenerateMap(0.25f, new Vector2(-10,-10));
        gridVisualization.SetGrid(map); 
    }

    private void SetValue(Chunk chunk ,int x,int y,int index)
    {
        chunk.tiles.GetValue(x, y).tileID = mapGeneratorSettings.tiles[index].tileID;
    }
    public void GenerateMap(float cellSize, Vector2 position)
    {
        map = new Dictionary<int, Chunk>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid = new Grid<GridTile>(chunkSize, chunkSize, cellSize, position + new Vector2(x * chunkSize * cellSize, y * chunkSize * cellSize), 
                    (Grid<GridTile> g, int x, int y) => { return new GridTile(x, y, g); });
                map.Add(x + y * width,new Chunk(grid,new Vector2(x * chunkSize,y * chunkSize)));
            }
        }

        
        foreach (var item in map)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float value = Generate(x + (int)item.Value.position.x, y + (int)item.Value.position.y);
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
                }
            }
        }
    }

    private float Generate(int x,int y)
    {
        float xf = (float)x / chunkSize * scale + offsetX;
        float yf = (float)y / chunkSize * scale + offsetY;
        float value = Mathf.PerlinNoise(xf,yf);
        return value;
    }


}
