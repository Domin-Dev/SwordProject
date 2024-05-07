

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridVisualization: MonoBehaviour
{
    public Grid<GridTile> grid { set; get; }
    TileStats tileStats;
    int textureWidth;
    int textureHeight;
    public static GridVisualization instance { private set; get; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        tileStats = Resources.Load<TileStats>("Tiles");
    }
    public void SetGrid(Grid<GridTile> grid)
    {
        textureWidth = GetComponent<MeshRenderer>().material.mainTexture.width;
        textureHeight = GetComponent<MeshRenderer>().material.mainTexture.height;
        this.grid = grid;
        CreateMesh();
        grid.OnTObjectChanged += UpdatedGrid;
        BuildingManager.instance._grid = grid;
    }
    private void UpdatedGrid(object sender, Grid<GridTile>.OnTObjectChangedArgs e)
    {
       UpdateMesh(e.x, e.y,true);
    }
    private void UpdateMesh(int x,int y,bool repeat)
    {
        if (x > 0 && y > 0 && x < grid.width && y < grid.height)
        {
            Mesh mesh = this.GetComponent<MeshFilter>().mesh;
            Vector2[] uv = mesh.uv;
            int index = x + y * grid.width;
            GridTile.TileType tile = grid.GetValue(x, y).tile;

            Vector2 uv11, uv00;
            int borders = 0;
            if (tile == GridTile.TileType.Sand || tile == GridTile.TileType.Mud)
            {
                borders = CalculateBorders(x, y);

                GetUVTile(tile, borders, out uv00, out uv11);
                uv[index * 4] = new Vector2((uv00.x + 0.01f) / textureWidth, (uv00.y + 0.01f) / textureHeight);
                uv[index * 4 + 1] = new Vector2((uv00.x + 0.01f) / textureWidth, uv11.y / textureHeight);
                uv[index * 4 + 2] = new Vector2(uv11.x / textureWidth, uv11.y / textureHeight);
                uv[index * 4 + 3] = new Vector2(uv11.x / textureWidth, (uv00.y + 0.01f) / textureHeight);
                mesh.uv = uv;

            }

            if (repeat)
            {
                UpdateMesh(x + 1, y,false);
                UpdateMesh(x - 1, y,false);
                UpdateMesh(x, y + 1,false);
                UpdateMesh(x, y - 1,false);

                UpdateMesh(x + 1, y + 1,false);
                UpdateMesh(x - 1, y + 1,false);
                UpdateMesh(x - 1, y - 1,false);
                UpdateMesh(x + 1, y - 1,false);
            }
        }
    }
    private void GetUVTile(GridTile.TileType tile,int borders,out Vector2 uv00, out Vector2 uv11)
    {
        Tile tileK;
        uv00 = Vector2.zero;
        uv11 = Vector2.zero;

        for (int k = 0; k < tileStats.tiles.Count; k++)
        {
            tileK = tileStats.tiles[k];
            if (tileK.tile == tile)
            {
                if (borders == 0)
                {
                    if (Random.Range(1, 101) > tileK.chance)
                    {
                        uv11 = tileK.uv00 + new Vector2(25, 25);
                        uv00 = tileK.uv00;
                    }
                    else
                    {
                        int variant = Random.Range(1, tileK.variants);
                        uv00 = tileK.uv00 + (new Vector2(25, 0) * variant);
                        uv11 = (tileK.uv00 + new Vector2(25, 25)) + (new Vector2(25, 0) * variant);
                    }
                }
                else
                {
                    if(borders < 0) borders = 15 - borders;
                    borders--;

                    uv00 = tileK.uv00 + new Vector2(0, 25) + (new Vector2(25, 0) * borders);
                    uv11 = tileK.uv00 + new Vector2(25, 50) + (new Vector2(25, 0) * borders);
                }              
            }
        }
    }
    private int CalculateBorders(int x,int y)
    {
        int value = 0;
        int number = 0;

        if (grid.GetValue(x, y + 1)?.tile  == GridTile.TileType.Grass) { value += 1; number++; }
        if (grid.GetValue(x + 1, y )?.tile == GridTile.TileType.Grass) { value += 2; number++; }
        if (grid.GetValue(x , y - 1)?.tile == GridTile.TileType.Grass) { value += 4; number++; }
        if (grid.GetValue(x - 1, y )?.tile == GridTile.TileType.Grass) { value += 8; number++; }

        if (value == 0)
        {
            if (grid.GetValue(x + 1, y + 1)?.tile == GridTile.TileType.Grass) value -= 1;
            if (grid.GetValue(x + 1, y - 1)?.tile == GridTile.TileType.Grass) value -= 2;
            if (grid.GetValue(x - 1, y - 1)?.tile == GridTile.TileType.Grass) value -= 4;
            if (grid.GetValue(x - 1, y + 1)?.tile == GridTile.TileType.Grass) value -= 8;
        }
        else if(number == 1)
        {
            int k = 0;
            switch (value)
            {
                case 1:
                    if (grid.GetValue(x - 1, y - 1)?.tile == GridTile.TileType.Grass) k += 1;
                    if (grid.GetValue(x + 1, y - 1)?.tile == GridTile.TileType.Grass) k += 2;
                    break;
                case 2:
                    if (grid.GetValue(x - 1, y - 1)?.tile == GridTile.TileType.Grass) k += 1;
                    if (grid.GetValue(x - 1, y + 1)?.tile == GridTile.TileType.Grass) k += 2;
                    break;
                case 4:
                    if (grid.GetValue(x - 1, y + 1)?.tile == GridTile.TileType.Grass) k += 1;
                    if (grid.GetValue(x + 1, y + 1)?.tile == GridTile.TileType.Grass) k += 2;
                    break;
                case 8:
                    if (grid.GetValue(x + 1, y - 1)?.tile == GridTile.TileType.Grass) k += 1;
                    if (grid.GetValue(x + 1, y + 1)?.tile == GridTile.TileType.Grass) k += 2;
                    break;
            }
            if (k > 0)
            {
                if(value != 8) value = 30 + ((value / 2) * 3) + k;
                else value = 39 + k;
            }
        }
        else if (number == 2)
        {      
            switch (value)
            {
                case 3:
                    if (grid.GetValue(x - 1, y - 1)?.tile == GridTile.TileType.Grass) value = 43;
                    break;
                case 6:
                    if (grid.GetValue(x - 1, y + 1)?.tile == GridTile.TileType.Grass) value = 44;
                    break;
                case 9:
                    if (grid.GetValue(x + 1, y - 1)?.tile == GridTile.TileType.Grass) value = 45;
                    break;
                case 12:
                    if (grid.GetValue(x + 1, y + 1)?.tile == GridTile.TileType.Grass) value = 46;
                    break;
            }
        }

        return value;
    }
    public void CreateMesh()
    {
        int width = grid.width;
        int height = grid.height;
        float cellSize = grid.cellSize;
        
        Mesh mesh = new Mesh();

        transform.position = new Vector3(grid.position.x, grid.position.y, 10);
        Vector3[] vertices = new Vector3[4 * (width * height)];
        Vector2[] uv = new Vector2[4 * (width * height)];
        int[] triangles = new int[6 * (width * height)];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;
                vertices[index * 4 + 0] = new Vector3( x * cellSize,       y * cellSize);
                vertices[index * 4 + 1] = new Vector3( x * cellSize,      (y + 1) * cellSize);
                vertices[index * 4 + 2] = new Vector3((x + 1) * cellSize, (y + 1) * cellSize);
                vertices[index * 4 + 3] = new Vector3((x + 1) * cellSize,  y * cellSize);

                triangles[index * 6]     = index * 4;
                triangles[index * 6 + 1] = index * 4 + 1;
                triangles[index * 6 + 2] = index * 4 + 2;

                triangles[index * 6 + 3] = index * 4; 
                triangles[index * 6 + 4] = index * 4 + 2;
                triangles[index * 6 + 5] = index * 4 + 3;


             
                GridTile.TileType tile = grid.GetValue(x, y).tile;

                int borders = 0;
                if(tile == GridTile.TileType.Sand || GridTile.TileType.Mud == tile) borders = CalculateBorders(x, y);
                Vector2 uv11, uv00;
                GetUVTile(tile,borders, out uv00, out uv11);

                uv[index * 4] = new Vector2((uv00.x + 0.01f) / textureWidth, (uv00.y + 0.01f) / textureHeight);
                uv[index * 4 + 1] = new Vector2((uv00.x + 0.01f) / textureWidth, uv11.y / textureHeight);
                uv[index * 4 + 2] = new Vector2(uv11.x / textureWidth, uv11.y / textureHeight);
                uv[index * 4 + 3] = new Vector2(uv11.x / textureWidth, (uv00.y + 0.01f) / textureHeight);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        this.GetComponent<MeshFilter>().mesh = mesh;
    }
}
