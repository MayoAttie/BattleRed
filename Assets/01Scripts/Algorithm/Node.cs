using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;
    public int gridZ;   

    public int gCost;
    public int hCost;

    public int xCost;
    public int yCost;
    public int zCost;

    public Node parent;

    int heapIndex;

    public Node(bool walkable, Vector3 worldPosition)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
    }

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY) : this(walkable, worldPosition)
    {
        this.gridX = gridX;
        this.gridY = gridY; 
    }

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int gridZ) : this(walkable, worldPosition, gridX, gridY)
    {
        this.gridZ = gridZ;
    }           

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int targetX, int targetY, int targetZ) : this(walkable, worldPosition, gridX, gridY)
    {
        this.xCost = Mathf.Abs(gridX - targetX);
        this.yCost = Mathf.Abs(gridY - targetY);
        this.zCost = Mathf.Abs(gridZ - targetZ); 
    }

    public int fCost
    { get { return gCost + hCost + xCost + yCost + zCost; } }

    public int HeapInedx 
    {
        get{ return heapIndex; } 
        set{ heapIndex = value; } 
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

}


#region 레거시
/*
 
 public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;

    int heapIndex;
    public Node(bool walkable, Vector3 worldPosition)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
    }

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY) : this(walkable, worldPosition)
    {
        this.gridX = gridX;
        this.gridY = gridY;
    }   

    public int fCost
    { get { return gCost + hCost; } }

    public int HeapInedx 
    {
        get{ return heapIndex; } 
        set{ heapIndex = value; } 
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }




 
 
 
 
 */

#endregion