using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{


    public event EventHandler<ValueArgs> onValueChange;
    public class ValueArgs : EventArgs
    {
        public int x;
        public int y;
        public ValueArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public static GameManager instance { private set; get; }

  
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
       // grid = new Grid<GridObject>(10, 10, 0.5f, new Vector2(-2,-1),(Grid<GridObject> grid,int x,int y) => { return new GridObject(6,x,y,grid); });
      //  grid.DebugDraw();
      //  onValueChange += OnValue;
      
    }

    private void OnValue(object s,EventArgs e)
    {
        Debug.Log("dziala");
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // grid.SetValue(MyTools.GetMouseWorldPosition(), 10);
            //  onValueChange?.Invoke(this, new ValueArgs(9,9));
          //  grid.GetValue(MyTools.GetMouseWorldPosition())?.AddValue(5);
        }
        if (Input.GetMouseButtonDown(1))
        {
        }
    }
}
