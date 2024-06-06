using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;

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

    [SerializeField] GameObject pointer;
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] GameObject buildingBar;
    private Transform barValue;

    [SerializeField] Transform parent;
    [SerializeField] Color planColor;

    Vector2 startPos;
    int selectedObjectID = -1;
    int rotationStates = 0;
    bool buildingMode;
    int rotation = 0;

    Transform objectBar;
    bool barIsActive;

    Transform planObject;
    Transform pointerObject;

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
        SetUpPointer();
    }

    Action<Vector2> build;
    Vector2 lastPos;

    private void Update()
    {
        if (buildingMode)
        {
            if (selectedObjectID > 0 && UIManager.instance.WindowsAreClosed())
            {
                Vector2 pos = Actions.GetMousePosXY();
                if (lastPos != pos) Plan(pos);
                if (Input.GetMouseButtonDown(0))
                {
                    build(pos);
                }
                if (Input.GetKeyDown(KeyCode.R) && rotationStates > 0)
                {
                    if(rotation >= 3)
                    {
                        rotation = 0;
                    }
                    else
                    {
                        rotation++;
                    }
                    planObject.GetComponent<SpriteRenderer>().sprite = ItemsAsset.instance.GetBuildingObjectSprite(selectedObjectID,rotation%rotationStates);

                }
            }
        }
    }
    public void StartBuildingMode(int id)
    {
        selectedObjectID = id;
        planObject.gameObject.SetActive(true);
        SetBuildMode();
        SpriteRenderer spriteRenderer = planObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ItemsAsset.instance.GetBuildingObjectSprite(id,rotation % rotationStates); 
        
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
        planObject = Instantiate(buildingPrefab, Vector2.zero, Quaternion.identity, parent).transform.GetChild(0);
        planObject.GetComponent<Collider2D>().enabled = false;
        planObject.GetComponent<SpriteRenderer>().color = planColor;
    }
    private void SetUpPointer()
    {
        pointerObject = Instantiate(pointer, parent).transform;
        pointerObject.gameObject.SetActive(false);
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
        lastPos = pos;
        planObject.transform.position = grid.GetPosition(pos);
        if (grid.GetValueByXY(pos).IsBuildObject())
            planObject.gameObject.SetActive(false);
        else
            planObject.gameObject.SetActive(true);
    }
    private void SetBuildMode()
    {
        Item item = ItemsAsset.instance.GetItem(selectedObjectID);
        if (item is Wall) build = BuildWall;
        else if (item is Floor) build = BuildFloor;
        else if (item is BuildingObject) build = BuildObject;

        if (item is BuildingObject) rotationStates = (item as BuildingObject).objectVariants.Length;
        else
        {
            rotationStates = 1;
            rotation = 0;
        }
    }
    private void BuildWall(Vector2 posXY)
    {
        if (grid.GetValueByXY(posXY).IsBuildObject()) return;    
        Sounds.instance.Hammer();
        Transform obj = Instantiate(buildingPrefab,grid.GetPosition(posXY), Quaternion.identity, parent).transform;
        grid.GetValueByXY(posXY).gridObject = new GridObject(selectedObjectID,0,obj);
        SetNewSprite(posXY);
        builtObject(this, null);
    }
    private void BuildFloor(Vector2 posXY)
    {
        GridTile gridTile = grid.GetValueByXY(posXY);
        if (gridTile.tileID != selectedObjectID)
        {
            grid.GetValueByXY(posXY).tileID = selectedObjectID;
            GridVisualization.instance.UpdateMesh((int)posXY.x, (int)posXY.y, true);
            Sounds.instance.Hammer();
            builtObject(this, null);
        }
    }
    private void BuildObject(Vector2 posXY)
    {
        if (grid.GetValueByXY(posXY).IsBuildObject()) return;

        Sounds.instance.Hammer();
        Transform obj = Instantiate(buildingPrefab, grid.GetPosition(posXY), Quaternion.identity, parent).transform.GetChild(0);
      
        ObjectVariant objectVariant = ItemsAsset.instance.GetObjectVariant(selectedObjectID, rotation % rotationStates);

        obj.GetComponent<SpriteRenderer>().sprite = objectVariant.variants[0].sprite;
        obj.GetComponent<PolygonCollider2D>().points = objectVariant.variants[0].hitbox;
        CreateGridObject(posXY, rotation % rotationStates, obj.parent);
        ChangePositionPivot(obj.parent, obj.TransformPoint(0, objectVariant.variants[0].minY, 0));
        builtObject(this, null);
    }
    private void CreateGridObject(Vector2 posXY,int indexVariant, Transform buildingObj)
    {
        Item item = ItemsAsset.instance.GetItem(selectedObjectID);
        GridTile gridObject = grid.GetValueByXY(posXY);

        switch (item)
        {
            case DoorItem : gridObject.gridObject = new GridDoor(selectedObjectID,indexVariant, buildingObj);
                return;
        }
        gridObject.gridObject = new GridObject(selectedObjectID,indexVariant, buildingObj);
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

        Transform child = gridObject.objectTransform.GetChild(0);
        ObjectVariant objectVariant = ItemsAsset.instance.GetObjectVariant(selectedObjectID, value);

        child.GetComponent<SpriteRenderer>().sprite = objectVariant.variants[0].sprite;
        child.GetComponent<PolygonCollider2D>().points = objectVariant.variants[0].hitbox;
        ChangePositionPivot(gridObject.objectTransform,child.TransformPoint(0, objectVariant.variants[0].minY, 0));
    }
    private void ChangePositionPivot(Transform transform,Vector3 newPosition)
    {
        Transform child = transform.GetChild(0);
        child.SetParent(null);
        transform.position = newPosition;
        child.SetParent(transform);
    }
    private bool[] GetNeighbors(Vector2 positionXY)
    {
        bool[] neighbors = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            var obj = grid.GetValueByXY(positionXY + MyTools.directions4[i]);
            if (obj != null && obj.IsBuildObject(selectedObjectID)) neighbors[i] = true;
            else neighbors[i] = false;
        }
        return neighbors;
    }
    public void ChangeSprite(Vector2 posXY, int index)
    {
        GridObject gridObject = grid.GetValueByXY(posXY).gridObject;
        Variant  variant = ItemsAsset.instance.GetObjectVariant(gridObject.ID, gridObject.indexVariant).variants[index];
        gridObject.objectTransform.GetComponentInChildren<SpriteRenderer>().sprite = variant.sprite;
        PolygonCollider2D polygonCollider2D = gridObject.objectTransform.GetComponentInChildren<PolygonCollider2D>();
        polygonCollider2D.points = variant.hitbox;
        polygonCollider2D.usedByComposite = false;
        Timer.Create(2f, () => { polygonCollider2D.usedByComposite = true; });
        ChangePositionPivot(gridObject.objectTransform, gridObject.objectTransform.GetChild(0).TransformPoint(0, variant.minY, 0));
    }
}
