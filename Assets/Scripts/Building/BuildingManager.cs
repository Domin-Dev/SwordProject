using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using static UnityEditor.PlayerSettings;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance { private set; get; }

    public event EventHandler SwitchBuildingUI;
    public event EventHandler hammerBlow;
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

    [SerializeField] GameObject buildingPrefab;


    [SerializeField] GameObject buildingBar;
    private Transform barValue;


    [SerializeField] Transform parent;
    [SerializeField] Color planColor;

    List<Transform> plan = new List<Transform>();
    int currentIndex;
    Vector2 startPos;
    Vector2 endPos;
    Vector2 size;
    Vector2[] pointsArray;
    int selectedObjectID = -1;
    Sprite sprite;

    bool buildingMode;

    Transform objectBar;
    bool barIsActive;


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
        sprite = ItemsAsset.instance.GetBuildingObjectSprite(ID, 0);
        SwitchBuildingUI(this, null);
        selectedObjectID = ID;    
    }

    private void Start()
    {
        UIManager.instance.SetUpUIBuilding(this);
        SetUpBar();
    }

    private void SetUpBar()
    {
        objectBar = Instantiate(buildingBar, Vector3.one, Quaternion.identity).transform;
        objectBar.gameObject.SetActive(false);
        barValue = objectBar.GetChild(0).GetChild(0);
        barIsActive = false;
    }
    private void Update()
    {
        if(buildingMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                SwitchBuildingUI(this, null);
            }

            if(selectedObjectID > 0)
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
            else if (selectedObjectID == 0)
            {
                Vector2 pos = grid.GetXY(MyTools.GetMouseWorldPosition());
                
                if (pos != startPos)
                {
                    startPos = pos;
                    UpdateBar();         
                }

                if (Input.GetMouseButton(0))
                {
                    hammerBlow(this, null);
                    var obj = grid.GetValueByXY(startPos);
                    if (obj.IsBuildObject() && obj.gridObject as ObjectPlan != null)
                    {
                        (obj.gridObject as ObjectPlan).Building(10);
                        UpdateBar();
                    }      
                }
            }
        }
    }

    private void UpdateBar()
    {
        var obj = grid.GetValueByXY(startPos);
        if (obj.IsBuildObject())
        {
            TurnOnBuildingBar(obj.gridObject.objectTransform.position + new Vector3(0, 0.5f), (obj.gridObject as IGetBarValue).GetBarValue());
        }
        else
        {
            TurnOffBuildingBar();
        }
    }

    private void TurnOnBuildingBar(Vector2 position,float value)
    {
        if (!objectBar.gameObject.activeSelf)
        {
            objectBar.gameObject.SetActive(true);
        }
        objectBar.transform.position = position;
        barValue.localScale = new Vector3(value, 1, 1);
    }

    private void TurnOffBuildingBar()
    {
        if (objectBar.gameObject.activeSelf)
        {
            objectBar.gameObject.SetActive(false);
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
        var obj = grid.GetValueByXY(vector);
        if (obj != null && obj.gridObject == null)
        {
            array.Add(vector);
        }
    }
    private void SpawnPlan(Vector2 pos)
    {
        if (currentIndex > plan.Count - 1)
        {
            Transform transform = Instantiate(buildingPrefab, pos, Quaternion.identity, parent).transform;
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
                Transform obj = Instantiate(buildingPrefab,grid.GetPosition(item), Quaternion.identity, parent).transform;
                obj.GetComponent<SpriteRenderer>().color = planColor;
     
                obj.GetComponent<Collider2D>().enabled = false;
                grid.GetValueByXY(item).gridObject = new ObjectPlan(selectedObjectID,100,obj);
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
            if (neighbors[i]) UpdateSprite(positionXY + MyTools.directions4[i]);
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

        if (neighbors[2])
        {
            gridObject.objectTransform.GetComponent<SortingGroup>().sortingOrder = 1;
        }
        else
        {
            gridObject.objectTransform.GetComponent<SortingGroup>().sortingOrder = 0;
        }

        buildingSprite = ItemsAsset.instance.GetBuildingObjectSprite(selectedObjectID,value);
        gridObject.objectTransform.GetComponent<SpriteRenderer>().sprite = buildingSprite;
    }

    private bool[] GetNeighbors(Vector2 positionXY)
    {
        bool[] neighbors = new bool[4];
        for (int i = 0; i < 4; i++)
        {
           var obj = grid.GetValueByXY(positionXY + MyTools.directions4[i]);
            if (obj != null) neighbors[i] = obj.IsBuildObject();
            else neighbors[i] = false;
        }
        return neighbors;
    }



}
