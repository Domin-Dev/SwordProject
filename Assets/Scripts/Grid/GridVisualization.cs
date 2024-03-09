
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualization: MonoBehaviour
{

    Grid<GridTile> grid;
    TileStats tileStats;
    int textureWidth;
    int textureHeight;

    private void Awake()
    {
        tileStats = Resources.Load<TileStats>("Tiles");
    }
    public void SetGrid(Grid<GridTile> grid)
    {
        textureWidth = GetComponent<MeshRenderer>().material.mainTexture.width;
        textureHeight = GetComponent<MeshRenderer>().material.mainTexture.height;
        this.grid = grid;
        CreateMesh(grid);
        grid.OnTObjectChanged += UpdatedGrid;
    }

    private void UpdatedGrid(object sender, Grid<GridTile>.OnTObjectChangedArgs e)
    {
        UpdateMesh(e.x, e.y);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
          Debug.Log(grid.GetValue(MyTools.GetMouseWorldPosition())?.ToString());
        }
    }

    private void UpdateMesh(int x,int y)
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector2[] uv = mesh.uv;
        int index = x + y * grid.width;
        GridTile.TileType tile = grid.GetValue(x, y).tile;
        Tile tileUV =  GetUVTile(tile);
        uv[index * 4] = new Vector2((tileUV.uv00.x + 0.01f) / textureWidth, tileUV.uv00.y / textureHeight);
        uv[index * 4 + 1] = new Vector2((tileUV.uv00.x + 0.01f) / textureWidth, tileUV.uv11.y / textureHeight);
        uv[index * 4 + 2] = new Vector2(tileUV.uv11.x / textureWidth, tileUV.uv11.y / textureHeight);
        uv[index * 4 + 3] = new Vector2(tileUV.uv11.x / textureWidth, tileUV.uv00.y / textureHeight);
        mesh.uv = uv;
    }



    private Tile GetUVTile(GridTile.TileType tile)
    {
        for (int k = 0; k < tileStats.tiles.Count; k++)
        {
            if (tileStats.tiles[k].tile == tile)
            {
                Debug.Log("s");
                return tileStats.tiles[k];
            }
        }

        return default(Tile);
    }
    public void CreateMesh(Grid<GridTile> grid)
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
                Tile tileUV = GetUVTile(tile);

                uv[index * 4] = new Vector2((tileUV.uv00.x + 0.01f) / textureWidth, tileUV.uv00.y / textureHeight);
                uv[index * 4 + 1] = new Vector2((tileUV.uv00.x + 0.01f) / textureWidth, tileUV.uv11.y / textureHeight);
                uv[index * 4 + 2] = new Vector2(tileUV.uv11.x / textureWidth, tileUV.uv11.y / textureHeight);
                uv[index * 4 + 3] = new Vector2(tileUV.uv11.x / textureWidth, tileUV.uv00.y / textureHeight);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        this.GetComponent<MeshFilter>().mesh = mesh;
    }
}
