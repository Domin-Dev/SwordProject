using UnityEngine;

public class MapGenerator : MonoBehaviour
{
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

 
    private void Start()
    {
        gridVisualization = GridVisualization.instance;
        mapGeneratorSettings = gridVisualization.settings;
        offsetX = Random.Range(0f,99999f);
        offsetY = Random.Range(0f,99999f);

        var map = GenerateMap(0.25f, offset);
        gridVisualization.SetMap(map); 
    }

    private void SetValue(Chunk chunk ,int x,int y,int index)
    {
        chunk.tiles.GetValue(x, y).tileID = mapGeneratorSettings.tiles[index].tileID;
    }
    public Map GenerateMap(float cellSize, Vector2 offset)
    {
        Map map = new Map(offset, cellSize,chunkSize,widthInChunks,heightInChunks);

        for (int y = 0; y < heightInChunks; y++)
        {
            for (int x = 0; x < widthInChunks; x++)
            {
                grid = new Grid<GridTile>(chunkSize, chunkSize, cellSize, offset + new Vector2(x * chunkSize * cellSize, y * chunkSize * cellSize), 
                    (Grid<GridTile> g, int x, int y) => { return new GridTile(x, y, g); });
                map.chunks.Add(x + y * widthInChunks,new Chunk(grid,new Vector2(x * chunkSize,y * chunkSize)));
            }
        }

        
        foreach (var item in map.chunks)
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
        return map;
    }

    private float Generate(int x,int y)
    {
        float xf = (float)x / chunkSize * scale + offsetX;
        float yf = (float)y / chunkSize * scale + offsetY;
        float value = Mathf.PerlinNoise(xf,yf);
        return value;
    }


}
