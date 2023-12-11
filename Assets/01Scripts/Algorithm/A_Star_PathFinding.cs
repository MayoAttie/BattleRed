using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class A_Star_PathFinding : MonoBehaviour
{
    A_Star_Grid grid;
    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<A_Star_Grid>();
        
    }

    private void Update()
    {
        if(target != null)
            FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // 탐색 할 노드
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        // 탐색 마친 노드
        HashSet<Node> closedSet = new HashSet<Node>();

        while(openSet.Count >0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                sw.Stop();
                print("Path found : " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode);
                return;
            }



            foreach (Node i in grid.GetNeighbours(currentNode))
            {
                if (!i.walkable || closedSet.Contains(i))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, i);
                if (newMovementCostToNeighbour < i.gCost || !openSet.Contains(i))
                {
                    // 코스트값 갱신
                    i.gCost = newMovementCostToNeighbour;
                    i.hCost = GetDistance(i, targetNode);
                    i.parent = currentNode;

                    if (!openSet.Contains(i))
                    {
                        openSet.Add(i);
                    }
                }

            }

        }
        
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int DistX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int DistY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int DistZ = Mathf.Abs(Mathf.RoundToInt(nodeA.worldPosition.y) - Mathf.RoundToInt(nodeB.worldPosition.y));

        int heuristic = 14 * (DistX + DistY + DistZ);

        return heuristic;
    }


    public Transform Seeker
    {
        get { return seeker; }
        set { seeker = value; } 
    }

    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }
}


#region 레거시

/*
 
  A_Star_Grid grid;
    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<A_Star_Grid>();
        
    }

    private void Update()
    {
        if(target != null)
            FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // 탐색 할 노드
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        // 탐색 마친 노드
        HashSet<Node> closedSet = new HashSet<Node>();

        while(openSet.Count >0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                sw.Stop();
                print("Path found : " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode);
                return;
            }



            foreach (Node i in grid.GetNeighbours(currentNode))
            {
                if (!i.walkable || closedSet.Contains(i))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, i);
                if (newMovementCostToNeighbour < i.gCost || !openSet.Contains(i))
                {
                    // 코스트값 갱신
                    i.gCost = newMovementCostToNeighbour;
                    i.hCost = GetDistance(i, targetNode);
                    i.parent = currentNode;

                    if (!openSet.Contains(i))
                    {
                        openSet.Add(i);
                    }
                }

            }

        }
        
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int DistX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int DistY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (DistX > DistY)
            return 14 * DistY + 10 * (DistX - DistY);

        return 14 * DistX + 10 * (DistY - DistX);
    }


    public Transform Seeker
    {
        get { return seeker; }
        set { seeker = value; } 
    }

    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }
 
 
 
 
 
 */


#endregion