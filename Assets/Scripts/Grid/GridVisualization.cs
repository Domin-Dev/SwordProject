

using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class TileUV
{
    public TileUV(Vector2 uv00,int variants = 1, Vector2? uv00Grass = null)
    {
        this.uv00 = uv00;
        this.variants = variants;
        this.uv00Grass = uv00Grass; 
    }
    public override string ToString()
    {
        return $"UV00:{uv00} Variants:{variants} UV00Grass{uv00Grass}";
    }

    public Vector2 uv00 { private set; get; }
    public int variants { private set; get; }
    public Vector2? uv00Grass { private set; get; }
}
public class GridVisualization: MonoBehaviour
{
    [SerializeField] public GameObject worldItem;
    public const int renderChunks = 2;

    public Map map;
    public Dictionary<int, Transform> loadedChunk;
    public int lastPlayerChunk = -1;


    public Grid<GridTile> oldGrid { set; get; }
    public Dictionary<int, TileUV> TilesUV { get;private set; }

    public MapGeneratorSettings settings { private set; get; }

    int textureWidth;
    int textureHeight;
    const int sizeTile = 25;//px
    float tileWidth;//    sizeTile /textureWidth;
    float tileHeight;//   sizeTile / textureHeight;
    Texture2D mapTexture;
    float width1;
    float height1;


    public static GridVisualization instance { private set; get; }

    [SerializeField] private Material mapMaterial;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        settings = Resources.Load<MapGeneratorSettings>("mapGeneratorSettings");
        SetUpMapMaterial();
    }

    private void SetUpMapMaterial(int sizeTile = 25)
    {
        TilesUV = new Dictionary<int, TileUV>();
        Floor[] array = ItemsAsset.instance.GetItemsByType<Floor>();    
        Texture2D texture = new Texture2D(46 * sizeTile, sizeTile * CountTextures(array));
        texture.filterMode = FilterMode.Point;

        textureWidth = texture.width;
        textureHeight = texture.height;
        tileWidth = (float)sizeTile / textureWidth;
        tileHeight = (float)sizeTile / textureHeight;
        width1 = 0.01f / textureWidth;
        height1 = 0.01f / textureHeight;

        int k = 0;
        for (int i = 0; i < array.Length; i++)
        {
            Floor floor = array[i];
            Vector2? grassUV = null;
            if (floor.grassTexture != null)
            {
                grassUV = new Vector2(0, (float)k * sizeTile / textureHeight);
                CopyTexture(ref k, sizeTile, floor.grassTexture, texture);
            }
            Vector2 uv00 = new Vector2(0,(float)k * sizeTile / textureHeight);
            CopyTexture(ref k, sizeTile, floor.texture, texture);
            int variants = floor.texture.width / sizeTile;

            TilesUV.Add(floor.ID, new TileUV(uv00,variants,grassUV));
        }
        texture.Apply(true, true);
        mapTexture = texture;
    }
    private int CountTextures(Floor[] array)
    {
        int counter = 0;
        foreach (Floor floor in array)
        {
            if(floor.texture != null)
            {
                counter++;
                if(floor.grassTexture != null)
                {
                    counter++;
                }
            }
        }
        return counter;
    }
    private void CopyTexture(ref int index,int height, Texture2D from,Texture2D to)
    {
        int width = from.width;
        Color32[] grassColors = from.GetPixels32();
        to.SetPixels32(0, index * height, width, height, grassColors);
        index++;
    }
    public void SetMap(Map map)
    {
        this.map = map;
        loadedChunk = new Dictionary<int, Transform>();
        CheckChunks(Vector2.zero);

        //foreach (var item in map.chunks)
        //{
        //    CreateMesh(item.Value.tiles,item.Value.position);
        //}

        // grid.OnTObjectChanged += UpdatedGrid;
        //BuildingManager.instance._grid = grid;
        // Actions.instance._grid = grid;
    }

    private void CheckChunks(Vector2 worldPosition)
    {
        Vector2 positionXY = GetXYPosition(worldPosition);
        
        int chunkIndex = GetChunkIndexByPosition(positionXY);

        Debug.Log(positionXY + " " + chunkIndex);
        if (lastPlayerChunk != chunkIndex)
        {
            lastPlayerChunk = chunkIndex;
            Vector2 posChunk = GetChunkPositionXY(chunkIndex);

            for (int x = -renderChunks; x <= renderChunks; x++)
            {
                for (int y = -renderChunks; y<= renderChunks; y++)
                {            
                    TryLoadChunk(GetChunkIndexByCoordinates(posChunk + new Vector2(x,y)));
                }
            }
            TryUnloadChunks(chunkIndex);
        }
    }

    private void TryUnloadChunks(int chunkIndex)
    {
        List<int> chunks = new List<int>();
        Vector2 pos = GetChunkPositionXY(chunkIndex);

        foreach (var item in loadedChunk)
        {
            Vector2 chunkPos = GetChunkPositionXY(item.Key);

            if(math.abs(chunkPos.x - pos.x) > renderChunks || math.abs(chunkPos.y - pos.y) > renderChunks)
            {
                item.Value.gameObject.SetActive(false);
                chunks.Add(item.Key);  
            }
        }

        for (int i = 0; i < chunks.Count; i++)
        {
            loadedChunk.Remove(chunks[i]);
        }
    }

    

    private void TryLoadChunk(int chunkIndex)
    {
        Debug.Log(chunkIndex);
        if(!loadedChunk.ContainsKey(chunkIndex) && chunkIndex >= 0 && chunkIndex < map.chunkCount)
        {
            Chunk chunk = map.chunks[chunkIndex];
            loadedChunk.Add(chunkIndex,CreateMesh(chunk.tiles, chunk.position));
            chunk.tiles.OnTObjectChanged += UpdatedGrid;
        }
    }

    private int GetChunkIndexByPosition(Vector2 position)
    {
        return (int)position.x / map.chunkSize + ((int)position.y/ map.chunkSize) * map.widthInChunks;
    }
    private int GetChunkIndexByCoordinates(Vector2 coordinates)
    {
        Debug.Log(coordinates);
        if(coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x < map.widthInChunks && coordinates.y < map.heightInChunks)
        {
           return (int)coordinates.x + (int)coordinates.y * map.widthInChunks;
        }
        else
        {
            return -1;
        }
    }

    private void UpdatedGrid(object sender, Grid<GridTile>.OnTObjectChangedArgs e)
    {
       UpdateMesh(e.x, e.y,true);
    }
    private void UVSet(Vector2[] uv,int index, Vector2 uv00,Vector2 uv11)
    {
        uv[index * 4]     = new Vector2(uv00.x + width1,uv00.y + height1);
        uv[index * 4 + 1] = new Vector2(uv00.x + width1,uv11.y          );
        uv[index * 4 + 2] = new Vector2(uv11.x         ,uv11.y          );
        uv[index * 4 + 3] = new Vector2(uv11.x         ,uv00.y + height1);
    }
   /// <summary>
   /// naprawic!!!
   /// </summary>
   /// <param name="x"></param>
   /// <param name="y"></param>
   /// <param name="repeat"></param>
    public void UpdateMesh(int x,int y,bool repeat)
    {
        if (x > 0 && y > 0 && x < oldGrid.width && y < oldGrid.height)
        {
            Mesh mesh = this.GetComponent<MeshFilter>().mesh;
            Vector2[] uv = mesh.uv;
            int index = x + y * oldGrid.width;
            GridTile gridTile = oldGrid.GetValue(x, y);

            Vector2 uv11, uv00;
            int borders = 0;// CalculateBorders(x, y);

            if (borders != gridTile.borders || repeat)
            {
                GetUVTile(gridTile.tileID, borders, out uv00, out uv11);
                gridTile.borders = borders;
                UVSet(uv, index, uv00, uv11);
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
    private void GetUVTile(int tileID,int borders,out Vector2 uv00, out Vector2 uv11)
    {
        uv00 = Vector2.zero;
        uv11 = Vector2.zero;

        TileUV tileUV = TilesUV[tileID];
        if (borders == 0 || tileUV.uv00Grass == null)
        {
            if (UnityEngine.Random.Range(1, 101) > 60)
            {
                uv11 = tileUV.uv00 + new Vector2(tileWidth, tileHeight);
                uv00 = tileUV.uv00;
            }
            else
            {
                int variant = 0;
                if(tileUV.variants > 1) variant = UnityEngine.Random.Range(1, tileUV.variants);

                uv00 = tileUV.uv00 + (new Vector2(tileWidth, 0) * variant);
                uv11 = (tileUV.uv00 + new Vector2(tileWidth, tileHeight)) + (new Vector2(tileWidth, 0) * variant);
            }
        }
        else
        {
            if(borders < 0) borders = 15 - borders;
            borders--;

            uv00 = (Vector2)tileUV.uv00Grass + new Vector2(tileWidth, 0) * borders;
            uv11 = (Vector2)tileUV.uv00Grass + new Vector2(tileWidth,tileHeight) + new Vector2(tileWidth, 0) * borders;
        }  
        
    }
    private int CalculateBorders(Grid<GridTile> grid, int x,int y)
    {
        int value = 0;
        int number = 0;

        if (grid.GetValue(x, y + 1)?.tileID  == settings.grassID) { value += 1; number++; }
        if (grid.GetValue(x + 1, y )?.tileID == settings.grassID) { value += 2; number++; }
        if (grid.GetValue(x , y - 1)?.tileID == settings.grassID) { value += 4; number++; }
        if (grid.GetValue(x - 1, y )?.tileID == settings.grassID) { value += 8; number++; }

        if (value == 0)
        {
            if (grid.GetValue(x + 1, y + 1)?.tileID == settings.grassID) value -= 1;
            if (grid.GetValue(x + 1, y - 1)?.tileID == settings.grassID) value -= 2;
            if (grid.GetValue(x - 1, y - 1)?.tileID == settings.grassID) value -= 4;
            if (grid.GetValue(x - 1, y + 1)?.tileID == settings.grassID) value -= 8;
        }
        else if(number == 1)
        {
            int k = 0;
            switch (value)
            {
                case 1:
                    if (grid.GetValue(x - 1, y - 1)?.tileID == settings.grassID) k += 1;
                    if (grid.GetValue(x + 1, y - 1)?.tileID == settings.grassID) k += 2;
                    break;
                case 2:
                    if (grid.GetValue(x - 1, y - 1)?.tileID == settings.grassID) k += 1;
                    if (grid.GetValue(x - 1, y + 1)?.tileID == settings.grassID) k += 2;
                    break;
                case 4:
                    if (grid.GetValue(x - 1, y + 1)?.tileID == settings.grassID) k += 1;
                    if (grid.GetValue(x + 1, y + 1)?.tileID == settings.grassID) k += 2;
                    break;
                case 8:
                    if (grid.GetValue(x + 1, y - 1)?.tileID == settings.grassID) k += 1;
                    if (grid.GetValue(x + 1, y + 1)?.tileID == settings.grassID) k += 2;
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
                    if (grid.GetValue(x - 1, y - 1)?.tileID == settings.grassID) value = 43;
                    break;
                case 6:
                    if (grid.GetValue(x - 1, y + 1)?.tileID == settings.grassID) value = 44;
                    break;
                case 9:
                    if (grid.GetValue(x + 1, y - 1)?.tileID == settings.grassID) value = 45;
                    break;
                case 12:
                    if (grid.GetValue(x + 1, y + 1)?.tileID == settings.grassID) value = 46;
                    break;
            }
        }

        return value;
    }
    public Transform CreateMesh(Grid<GridTile> grid, Vector2 pos)
    {
        MeshFilter meshFilter = new GameObject("part of map").AddComponent<MeshFilter>();
        int width = grid.width;
        int height = grid.height;
        float cellSize = grid.cellSize;
        
        Mesh mesh = new Mesh();

        meshFilter.transform.position = new Vector3(grid.position.x, grid.position.y, 10);
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


                GridTile gridTile = grid.GetValue(x, y);
             
                int borders = 0;
                if (IsGrass(gridTile.tileID)) borders = CalculateBorders(grid,x, y);
                Vector2 uv11, uv00;
                GetUVTile(gridTile.tileID, borders, out uv00, out uv11);
                gridTile.borders = borders;
                UVSet(uv, index, uv00, uv11);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        MeshRenderer  meshRenderer = meshFilter.AddComponent<MeshRenderer>();
        meshRenderer.material = mapMaterial;
        meshRenderer.material.mainTexture = mapTexture;
        meshFilter.transform.parent = transform;
        meshFilter.mesh = mesh;
        return meshFilter.transform;

    }
    private bool IsGrass(int tileID)
    {
       return TilesUV.ContainsKey(tileID) && TilesUV[tileID].uv00Grass != null;
    }
    public void PlayerMovement(Vector2 pos)
    {
        CheckChunks(pos);
    }
    private Vector2 GetChunkPositionXY(int chunk)
    {
        int x = chunk % map.widthInChunks;
        int y = chunk / map.widthInChunks;
        return new Vector2(x, y);
    }
    private Vector2 GetXYPosition(Vector2 position)
    {
        int x = Mathf.FloorToInt((position.x - map.offset.x) / map.cellSize);
        int y = Mathf.FloorToInt((position.y - map.offset.y) / map.cellSize);
        return new Vector2(x, y);
    }

    public Grid<GridTile> GetGridByXY(Vector2 posXY)
    {
        int x = (int)posXY.x % map.chunkSize;
        int y = (int)posXY.y % map.chunkSize;
        return map.chunks[x + y * map.widthInChunks].tiles;
    }
    public void DestroyObject(GridTile gridTile)
    {
        int id = gridTile.gridObject.ID;
        Item item = ItemsAsset.instance.GetItem(id);
        Destroy(gridTile.gridObject.objectTransform.gameObject);
        gridTile.gridObject = null;
        Vector2 vector2 = new Vector2(gridTile.x, gridTile.y);
        Vector2 target = oldGrid.GetPosition(vector2 + new Vector2(UnityEngine.Random.Range(-0.5f,0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)));
        CreateWorldItem(new ItemStats(id), oldGrid.GetPosition(vector2 + new Vector2(0,0.5f)),target);
        if (item is Wall) UpdateNeighbors(vector2, id); 
    }
    public void UpdateNeighbors(Vector2 positionXY, int ID)
    {
        bool[] neighbors = GetNeighbors(positionXY, ID);
        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i]) UpdateSprite(positionXY + MyTools.directions4[i]);
        }
    }
    private bool[] GetNeighbors(Vector2 positionXY, int ID)
    {
        bool[] neighbors = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            var obj = oldGrid.GetValueByXY(positionXY + MyTools.directions4[i]);
            if (obj != null && obj.IsBuildObject(ID)) neighbors[i] = true;
            else neighbors[i] = false;
        }
        return neighbors;
    }
    private void UpdateSprite(Vector2 positionXY)
    {
        GridObject gridObject = oldGrid.GetValueByXY(positionXY).gridObject;
        bool[] neighbors = GetNeighbors(positionXY, gridObject.ID);
        int value = 0;

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i])
            {
                value += (int)math.pow(2f, i);
            }
        }

        if (neighbors[2])
        {
            gridObject.objectTransform.GetComponent<SortingGroup>().sortingOrder = 1;
        }
        else
        {
            gridObject.objectTransform.GetComponent<SortingGroup>().sortingOrder = 0;
        }

        Transform child = gridObject.objectTransform.GetChild(0);
        ObjectVariant objectVariant = ItemsAsset.instance.GetObjectVariant(gridObject.ID, value);

        child.GetComponent<SpriteRenderer>().sprite = objectVariant.variants[0].sprite;
        child.GetComponent<PolygonCollider2D>().points = objectVariant.variants[0].hitbox;
        MyTools.ChangePositionPivot(gridObject.objectTransform, child.TransformPoint(0, objectVariant.variants[0].minY, 0));
    }

    public void SetNewSprite(Vector2 positionXY,int id)
    {
        bool[] neighbors = GetNeighbors(positionXY, id);
        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i]) UpdateSprite(positionXY + MyTools.directions4[i]);
        }
        UpdateSprite(positionXY);
    }

    public void CreateWorldItem(ItemStats item,Vector2 pos,Vector2 target)
    {
        Instantiate(worldItem,pos,Quaternion.identity).GetComponent<WorldItem>().SetItem(item,target);
    }
}
