
using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private float scale = 20;

    [SerializeField] private float offsetX = 100f;
    [SerializeField] private float offsetY = 100f;

    GridVisualization gridVisualization;
    Grid<GridTile> grid;

    private void Start()
    {
        gridVisualization = GetComponent<GridVisualization>();
        offsetX = Random.Range(0f,99999f);
        offsetY = Random.Range(0f,99999f);
        gridVisualization.SetGrid(GenerateMap(0.25f, new Vector2(-10, -10)));
    }

    private void Update()
    {

        //for (int y = 0; y < grid.height; y++)
        //{
        //    for (int x = 0; x < grid.height; x++)
        //    {
        //        float value = Generate(x, y);
        //        if (value > 0.65)
        //        {
        //            grid.GetValue(x, y).tile = GridTile.TileType.Water;
        //        }
        //        else if (value > 0.6)
        //        {
        //            grid.GetValue(x, y).tile = GridTile.TileType.Sand;
        //        }
        //        else if (value > 0.3)
        //        {
        //            grid.GetValue(x, y).tile = GridTile.TileType.Grass;
        //        }
        //        else
        //        {
        //            grid.GetValue(x, y).tile = GridTile.TileType.Mud;
        //        }

        //    }
        //}
        //gridVisualization.CreateMesh(grid);
        
    }
    public  Grid<GridTile> GenerateMap(float cellSize, Vector2 position)
    {
        grid = new Grid<GridTile>(width, height, cellSize, position, (Grid<GridTile> g, int x, int y) => { return new GridTile(x, y, g); });

        for (int y = 0; y < grid.height; y++)
        {
            for (int x = 0; x < grid.height; x++)
            {
                float value = Generate(x, y);
                if (value > 0.65)
                {
                    grid.GetValue(x, y).tile = GridTile.TileType.Water;
                }
                else if(value > 0.6)
                {
                    grid.GetValue(x, y).tile = GridTile.TileType.Sand;
                }
                else if (value > 0.3)
                {
                    grid.GetValue(x, y).tile = GridTile.TileType.Grass;
                }
                else
                {
                    grid.GetValue(x, y).tile = GridTile.TileType.Mud;
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
