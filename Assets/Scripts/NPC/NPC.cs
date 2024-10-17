using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{

    Rigidbody2D rigidbody2D;
    [SerializeField] Vector2 target; 
    [SerializeField] NPCSpriteController characterSpriteController;

    List<GridTile> path;
    
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        characterSpriteController = GetComponent<NPCSpriteController>();
        NewPath();
        if (path.Count > 0)
        target = GridVisualization.instance.GetWorldPosition(path[0].x, path[0].y);
    }

    private void FixedUpdate()
    {
        Follow();
    }
    public void Follow()
    {
        Vector2 vector2 = target - (Vector2)transform.position;
        rigidbody2D.velocity = vector2.normalized * 0.7f;
        characterSpriteController.UpdateSprite(vector2.normalized, vector2.normalized);
        if (Vector2.Distance(transform.position, target) <= 0.02)
        {
            path.RemoveAt(0);
            if (path.Count > 0)
            {
                target = GridVisualization.instance.GetWorldPosition(path[0].x, path[0].y);
            }
            else
            {
                NewPath();
            }
        }
       
    }

    public void NewPath()
    {
        Vector2 xy = GridVisualization.instance.GetGridPosition(transform.position);
        int x = Random.Range(0, 20);
        int y = Random.Range(0, 20);
        path = GridVisualization.instance.pathfinding.FindPath((int)xy.x, (int)xy.y,x, y);
        Debug.Log(x + " " + y);
    }
}
