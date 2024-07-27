using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class Grid<T>
{
    public event EventHandler<OnTObjectChangedArgs> OnTObjectChanged;
    public class OnTObjectChangedArgs : EventArgs
    {
        public int x;
        public int y;

        public OnTObjectChangedArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public int height { private set; get; }
    public int width { private set; get; }
    public float cellSize { private set; get; }
    public Vector2 position { private set; get; }

    private T[,] gridArray;
  // private TextMesh[,] textArray;
    Transform textParent;


    public Grid(int width, int height, float cellSize, Vector2 position, Func<Grid<T>, int, int, T> CreateTObject,Vector2 gridCo)
    {
        this.height = height;
        this.width = width;
        this.cellSize = cellSize;
        this.position = position;
        this.gridArray = new T[width, height];
       // this.textArray = new TextMesh[width, height];
       // textParent = new GameObject("Texts").transform;

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = CreateTObject(this, x, y);
            }
        }
        OnTObjectChanged += UpdateCounter;
    }

    private void UpdateCounter(object sender, OnTObjectChangedArgs e)
    {
     //   textArray[e.x, e.y].text = GetValue(e.x,e.y).ToString();
    }

    public void DebugDraw()
    {
        for (int x = 0; x < this.gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < this.gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetPosition(x, y), GetPosition(x, y +1), Color.red, 100f);
                Debug.DrawLine( GetPosition(x, y), GetPosition(x + 1, y ), Color.red, 100f);
             //   textArray[x,y] = MyTools.CreateText(gridArray[x,y].ToString(),15, GetPosition(x,y) + new Vector2(cellSize,cellSize)* 0.5f, textParent, Color.red);
            
            }

            Debug.DrawLine( GetPosition(0,height),  GetPosition(width, height), Color.red, 100f);
            Debug.DrawLine( GetPosition(width,0),  GetPosition(width, height), Color.red, 100f);
        }
    }

    private Vector2 GetPosition(int x, int y)
    {
        return  position + new Vector2(x,y) * cellSize + new Vector2(cellSize / 2, 0);
    }
    public Vector2 GetPosition(Vector2 xy)
    {
        return position + xy * cellSize + new Vector2(cellSize/2,0);
    }

    private void GetXY(Vector2 position,out int x,out int y)
    {
        x = Mathf.FloorToInt((position.x - this.position.x)/ cellSize);
        y = Mathf.FloorToInt((position.y - this.position.y) / cellSize);
    }

    public Vector2 GetXY(Vector2 position)
    {
        int x = Mathf.FloorToInt((position.x - this.position.x) / cellSize);
        int y = Mathf.FloorToInt((position.y - this.position.y) / cellSize);
        return new Vector2(x,y);
    }

    public Vector2 GetGridPosition(Vector3 globalPosition)
    {
        int x, y;
        GetXY(globalPosition,out x,out y);
        return GetPosition(x,y) + new Vector2(cellSize,0);
    }
    public void SetValue(int x, int y, T value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x,y] = value;
            //textArray[x, y].text = value.ToString();
            OnTObjectChanged?.Invoke(this, new OnTObjectChangedArgs(x, y));
        }
    }
    public void SetValue(Vector2 position,T value)
    {
        int x, y;
        GetXY(position,out x,out y);
        SetValue(x,y,value);
    }

    public T GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];    
        }
        return default(T);
    }
    public T GetValueByXY(Vector2 xy)
    {
        return GetValue((int)xy.x,(int)xy.y);
    }

    public T GetValue(Vector2 position)
    {
        int x, y;
        GetXY(position,out x,out y);
        return GetValue(x, y);
    }

    public void TObjectChanged(int x,int y)
    {
        OnTObjectChanged?.Invoke(this, new OnTObjectChangedArgs(x,y));
    }


}
