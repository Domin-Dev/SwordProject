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
    public void SelectedBuildingObject(int ID)
    {
        SwitchBuildingUI(this, null);     
        buildingMode = true;
    }

    List<Transform> plan = new List<Transform>();

    Vector2 startPos;
    Vector2 endPos;
    Vector2 size;

    private void Start()
    {
        UIManager.instance.SetUpUIBuilding(this);
    }

    private void Update()
    {
        if(buildingMode)
        {
            Vector2 pos = grid.GetXY(MyTools.GetMouseWorldPosition());
            if (Input.GetMouseButtonDown(0))
            {
                startPos = pos;
            }

            if(Input.GetMouseButton(0))
            {
                if (pos != endPos)
                {
                    endPos = pos;
                    CountSize();
                    TooltipSystem.ShowInstant(size.ToString());
                    Planning();
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                TooltipSystem.Hide();
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
    private void Planning()
    {
        int value = (int)size.x;
        for (int i = 0; i <= math.abs(size.x); i++)
        {
            Vector2 vector;
            if (size.x >= 0) vector = new Vector2(value - i, 0);
            else vector = new Vector2(value + i,0);
            SpawnPlan(grid.GetPosition(startPos + vector));
            SpawnPlan(grid.GetPosition(endPos - vector));
        }

        value = (int)size.y;   
        for (int i = 0; i <= math.abs(size.y); i++)
        {
            Vector2 vector;
            if (size.y >= 0) vector = new Vector2(0,value - i);
            else vector = new Vector2(0,value + i);
            SpawnPlan(grid.GetPosition(startPos + vector));
            SpawnPlan(grid.GetPosition(endPos - vector));
        }



    }

    private void SpawnPlan(Vector2 pos)
    {
        Transform transform = Instantiate(BuildingPrefab, pos, Quaternion.identity, parent).transform;
        transform.GetComponent<SpriteRenderer>().color = planColor;
        transform.GetComponent<Collider2D>().enabled = false;
        plan.Add(transform);
    }

    private void Build()
    {
        plan.Clear();
        Sounds.instance.Hammer();
    }


}
