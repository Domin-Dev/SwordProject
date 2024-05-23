using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.Rendering;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance { private set; get; }

    public event EventHandler builtObject;
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

    Vector2 startPos;
    int selectedObjectID = -1;
    bool buildingMode;

    Transform objectBar;
    bool barIsActive;

    Transform planObject;

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
    private void Start()
    {
        SetUpBar();
        SetUpPlanObject();
    }

    Action<Vector2> build;
    private void Update()
    {
        if (buildingMode)
        {
            if (selectedObjectID > 0)
            {
                Vector2 pos = grid.GetXY(MyTools.GetMouseWorldPosition());
                Plan(pos);
                if (Input.GetMouseButtonDown(0))
                {
                    build(pos);
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

                //if (Input.GetMouseButtonDown(0))
                //{
                //    var obj = grid.GetValueByXY(startPos);
                //    if (obj.IsBuildObject() && obj.gridObject as ObjectPlan != null)
                //    {
                //        if ((obj.gridObject as ObjectPlan).Building(40) >= 1)
                //        {
                //            (obj.gridObject as ObjectPlan).objectTransform.GetComponent<SpriteRenderer>().color = Color.white;
                //        }
                //        UpdateBar();
                //        Sounds.instance.Hammer();
                //    }
                //}
            }
        }
    }
    public void StartBuildingMode(int id)
    {
        selectedObjectID = id;
        planObject.gameObject.SetActive(true);
        planObject.GetComponent<SpriteRenderer>().sprite = ItemsAsset.instance.GetBuildingObjectSprite(id, 0);
        SetBuildMode();
        buildingMode = true;
    }
    public void EndBuildingMode()
    {
        buildingMode = false;
        planObject.gameObject.SetActive(false);
        selectedObjectID = -1;
    }
    private void SetUpBar()
    {
        objectBar = Instantiate(buildingBar, Vector3.one, Quaternion.identity).transform;
        objectBar.gameObject.SetActive(false);
        barValue = objectBar.GetChild(0).GetChild(0);
        barIsActive = false;
    }
    private void SetUpPlanObject()
    {
        planObject = Instantiate(buildingPrefab, Vector2.zero, Quaternion.identity, parent).transform;
        planObject.GetComponent<Collider2D>().enabled = false;
        planObject.GetComponent<SpriteRenderer>().color = planColor;
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
    private void Plan(Vector2 pos)
    {
        planObject.transform.position = grid.GetPosition(pos);
    }

    private void SetBuildMode()
    {
        Item item = ItemsAsset.instance.GetItem(selectedObjectID);
        if (item as Wall != null) build = BuildWall;
        else if (item as Floor != null) build = BuildFloor;    
    }
    private void BuildWall(Vector2 posXY)
    {
        if (grid.GetValueByXY(posXY).IsBuildObject()) return;    

        Sounds.instance.Hammer();
        Transform obj = Instantiate(buildingPrefab,grid.GetPosition(posXY), Quaternion.identity, parent).transform;
        grid.GetValueByXY(posXY).gridObject = new GridObject(selectedObjectID,obj);
        SetNewSprite(posXY);
        builtObject(this, null);
    }

    private void BuildFloor(Vector2 posXY)
    {
        grid.GetValueByXY(posXY).tileID = selectedObjectID;
        GridVisualization.instance.UpdateMesh((int)posXY.x, (int)posXY.y,true);
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

        if (neighbors[2])
        {
            gridObject.objectTransform.GetComponent<SortingGroup>().sortingOrder = 1;
        }
        else
        {
            gridObject.objectTransform.GetComponent<SortingGroup>().sortingOrder = 0;
        }

        gridObject.objectTransform.GetComponent<SpriteRenderer>().sprite = ItemsAsset.instance.GetBuildingObjectSprite(selectedObjectID, value); ;
        gridObject.objectTransform.GetComponent<PolygonCollider2D>().points = ItemsAsset.instance.GetBuildingObjectHitbox(selectedObjectID,value);

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
