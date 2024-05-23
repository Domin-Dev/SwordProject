
using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private float scale = 20;

    [SerializeField] private float offsetX = 100f;
    [SerializeField] private float offsetY = 100f;


    private MapGeneratorSettings mapGeneratorSettings;
    GridVisualization gridVisualization;
    Grid<GridTile> grid;

    private void Start()
    {
        gridVisualization = GridVisualization.instance;
        mapGeneratorSettings = gridVisualization.settings;
        offsetX = Random.Range(0f,99999f);
        offsetY = Random.Range(0f,99999f);
        gridVisualization.SetGrid(GenerateMap(0.25f, new Vector2(-10, -10)));    
    }

    private void SetValue(int x,int y,int index)
    {
        grid.GetValue(x, y).tileID = mapGeneratorSettings.tiles[index].tileID;
    }
    public Grid<GridTile> GenerateMap(float cellSize, Vector2 position)
    {
        grid = new Grid<GridTile>(width, height, cellSize, position, (Grid<GridTile> g, int x, int y) => { return new GridTile(x, y, g); });

        for (int y = 0; y < grid.height; y++)
        {
            for (int x = 0; x < grid.height; x++)
            {
                float value = Generate(x, y);
                if (value > 0.65)
                {
                    SetValue(x, y, 0);
                }
                else if (value > 0.3)
                {
                    SetValue(x, y, 1);
                }
                else 
                {
                    SetValue(x, y, 2);
                }
            }
        }
        return grid;
    }

    private float Generate(int x,int y)
    {
        float xf = (float)x / width * scale + offsetX;
        float yf = (float)y / height * scale + offsetY;
        float value = Mathf.PerlinNoise(xf,yf);
        return value;
    }


}
