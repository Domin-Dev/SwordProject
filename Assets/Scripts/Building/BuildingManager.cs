using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Mathematics;



public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance { private set; get; }

    public event EventHandler SwitchBuildingUI;

    public static Grid<GridTile> grid { private set; get; }
    public Grid<GridTile> _grid
    {
        set
        {
            if(grid == null)
            {
                grid = value;
            }
        }
    }

    [SerializeField] GameObject BuildingPrefab;
    [SerializeField] Transform parent;
    [SerializeField] Color planColor;
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
    }

    bool buildingMode;

    public void StartBuildingMode()
    {
        buildingMode = true;
    }
    public void EndBuildingMode()
    {
        buildingMode = false;
        selectedObjectID = -1;
    }

    public void SelectedBuildingObject(int ID)
    {
        sprite = ItemsAsset.instance.GetBuildingObjectSprites(0)[0];
        SwitchBuildingUI(this, null);
        selectedObjectID = ID;    
    }

    List<Transform> plan = new List<Transform>();
    int currentIndex;
    Vector2 startPos;
    Vector2 endPos;
    Vector2 size;
    Vector2[] pointsArray;
    int selectedObjectID = -1;
    Sprite sprite;

    private void Start()
    {
        UIManager.instance.SetUpUIBuilding(this);
    }

    private void Update()
    {


        if(buildingMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                SwitchBuildingUI(this, null);
            }

            if(selectedObjectID >= 0)
            {
                Vector2 pos = grid.GetXY(MyTools.GetMouseWorldPosition());
                if (Input.GetMouseButtonDown(0))
                {
                    startPos = pos;
                }

                if (Input.GetMouseButton(0))
                {
                    if (pos != endPos)
                    {
                        endPos = pos;
                        CountSize();
                        Planning();
                        if (pointsArray.Length > 1) TooltipSystem.ShowInstant($"{math.abs(size.x) + 1} x {math.abs(size.y) + 1}");
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    TooltipSystem.Hide();
                    ClearPlan();
                    Build();
                }
            }
        }
    }
    private void CountSize()
    {
        int x, y;
        x = (int)(endPos.x - startPos.x);
        y = (int)(endPos.y - startPos.y);
        size = new Vector2(x, y);
    }
    private int CountObjects()
    {
        int absX = (int)math.abs(size.x);
        int absY = (int)math.abs(size.y);

        if(absY == 0) absX = (absX + 1);
        else if (absX > 0) absX = (absX + 1) * 2 - 2;

        if(absX == 0) absY = (absY + 1);
        else if(absY > 0) absY = (absY + 1) * 2 - 2;

        return absX + absY;
    }
    private void ClearPlan()
    {
        foreach (Transform item in plan)
        {
            item.gameObject.SetActive(false);
        }
    }
    private void Planning()
    {
        currentIndex = 0;
        pointsArray = GetPositions().ToArray();
        Debug.Log(pointsArray.Length);
        foreach (Vector2 item in pointsArray)
        {
            if (item != null)
            {
                SpawnPlan(grid.GetPosition(item));
            }
        }
    } 
    List<Vector2> GetPositions()
    {
        int absSizeX = (int)math.abs(size.x);
        int absSizeY = (int)math.abs(size.y);
        List<Vector2> positions = new List<Vector2>();

        int value = (int)size.x;

        for (int i = 0; i <= absSizeX; i++)
        {
            Vector2 vector;
            if (size.x >= 0) vector = new Vector2(value - i, 0);
            else vector = new Vector2(value + i, 0);
            if (i != absSizeX)
            {
                AddToList(positions,startPos + vector);
            }
            if (absSizeY > 0 && i != 0 && i != absSizeX)
            {
                AddToList(positions,endPos - vector);
            }
        }

        value = (int)size.y;

        for (int i = 0; i <= absSizeY; i++)
        {
            Vector2 vector;
            if (size.y >= 0) vector = new Vector2(0, value - i);
            else vector = new Vector2(0, value + i);

            AddToList(positions, startPos + vector);
            if (absSizeX > 0 && i != 0)
            {
                AddToList(positions,endPos - vector);       
            }
        }

        for (int i = currentIndex; i < plan.Count; i++)
        {
            plan[i].gameObject.SetActive(false);
        }
        return positions;
    }

    void AddToList(List<Vector2> array,Vector2 vector)
    {
        if (grid.GetValueByXY(vector).gridObject == null)
        {
            array.Add(vector);
        }
    }
    private void SpawnPlan(Vector2 pos)
    {
        if (currentIndex > plan.Count - 1)
        {
            Transform transform = Instantiate(BuildingPrefab, pos, Quaternion.identity, parent).transform;
            transform.GetComponent<SpriteRenderer>().color = planColor;
            transform.GetComponent<SpriteRenderer>().sprite = sprite;
            transform.GetComponent<Collider2D>().enabled = false;
            plan.Add(transform);
        }
        else
        {
            Transform item = plan[currentIndex];
            item.gameObject.SetActive(true);
            item.position = pos;
        }
        currentIndex++;
    }
    private void Build()
    {
        if (pointsArray != null)
        {
            Sounds.instance.Hammer();
            foreach (Vector2 item in pointsArray)
            {
                Transform obj = Instantiate(BuildingPrefab,grid.GetPosition(item), Quaternion.identity, parent).transform;
                //     obj.GetComponent<SpriteRenderer>().color = planColor;
     
                obj.GetComponent<Collider2D>().enabled = false;
                grid.GetValueByXY(item).gridObject = new ObjectPlan(selectedObjectID,obj);
                SetNewSprite(item);
            }
        }
        pointsArray = null;
    }


    private void SetNewSprite(Vector2 positionXY)
    {
        bool[] neighbors = GetNeighbors(positionXY);
        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i]) UpdateSprite(positionXY + MyTools.Directions4[i]);
        }
        UpdateSprite(positionXY);
    }

    private void UpdateSprite(Vector2 positionXY)
    {
        GridObject gridObject = grid.GetValueByXY(positionXY).gridObject;
        bool[] neighbors = GetNeighbors(positionXY);

        int value = 0;

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i])
            {
                value +=(int)math.pow(2f,i);
            }
        }
        Sprite buildingSprite = null;

        buildingSprite = ItemsAsset.instance.GetBuildingObjectSprite(0,value);
     
        gridObject.objectTransform.GetComponent<SpriteRenderer>().sprite = buildingSprite;
    }

    private bool[] GetNeighbors(Vector2 positionXY)
    {
        bool[] neighbors = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            neighbors[i] = grid.GetValueByXY(positionXY + MyTools.Directions4[i]).IsBuildObject();
        }
        return neighbors;
    }



}
