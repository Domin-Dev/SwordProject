using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinding
{
    private const int moveStraightCost = 10;
    private const int moveDiagonalCost = 14;


    GridVisualization gridVisualization;

    private List<GridTile> openList;
    private List<GridTile> closedList;
    public Pathfinding(GridVisualization gridVisualization)
    {
        this.gridVisualization = gridVisualization;
    }

    public List<GridTile> FindPath(int startX,int startY,int endX,int endY)
    {
        GridTile startNode = gridVisualization.GetGridTileByPositionXY(startX, startY);
        GridTile endNode = gridVisualization.GetGridTileByPositionXY(endX, endY);

        openList = new List<GridTile>() { startNode };
        closedList = new List<GridTile>();

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            GridTile currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                return CalculatePath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            var list = GetNeighbourList(currentNode);
            foreach (GridTile tile in list)
            {
                if (closedList.Contains(tile)) continue;
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, tile);
                if(tentativeGCost < tile.gCost)
                {
                    tile.cameFrom = currentNode;
                    tile.gCost = tentativeGCost;
                    tile.hCost = CalculateDistanceCost(tile, endNode);
                    tile.CalculateFCost();
                    if(!openList.Contains(tile)) openList.Add(tile);
                }
            }
        }
        return null;
    }

    private List<GridTile> GetNeighbourList(GridTile gridTile)
    {
        Vector2 pos = new  Vector2(gridTile.x, gridTile.y);
        List<GridTile> list = new List<GridTile>();
        for (int i = 0; i < MyTools.directions8.Length; i++)
        {
            Vector2 vector2 = MyTools.directions8[i] + pos;
            var value =  gridVisualization.GetGridTileByPositionXY((int)vector2.x, (int)vector2.y);
            if(value != null) list.Add(value);
        }
        return list;
    }

    private List<GridTile> CalculatePath(GridTile endNode)
    {
        List<GridTile> path = new List<GridTile>();
        path.Add(endNode);
        GridTile current = endNode;
        while(current != null)
        {
            path.Add(current.cameFrom);
            current = current.cameFrom;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(GridTile a, GridTile b)
    {
        int xDistance = math.abs(a.x - b.x);
        int yDistance = math.abs(a.y - b.y);
        int remaining = math.abs(xDistance - yDistance);
        return moveStraightCost * math.min(xDistance, yDistance) + moveStraightCost * remaining;
    }

    private GridTile GetLowestFCostNode(List<GridTile> noteList)
    {
        GridTile lowestFCostNode = noteList[0];
        for (int i = 0; i < noteList.Count; i++)
        {
            if (noteList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = noteList[i];
            }
        }
        return lowestFCostNode;
    }
}
