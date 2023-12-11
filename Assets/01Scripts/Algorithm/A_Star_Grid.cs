using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Star_Grid : MonoBehaviour
{
    public bool onlyDisplayPathGizmos;

    public Transform player;
    public LayerMask unWalableMask;
    public LayerMask groundMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    // x, z 축 보정값
    float xOffset = 50f;
    float zOffset = 50f;

    public List<Node> path;

    private void Start()
    {
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * (gridWorldSize.x / 2 - xOffset) - Vector3.up * (gridWorldSize.y / 2 - zOffset);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                //z 축 추가
                worldPoint += Vector3.up * GetHeightAtWorldPosition(worldPoint);

                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unWalableMask));

                // 수정된 부분: z 값을 받아오도록 수정
                float z = worldPoint.z;

                grid[x, y] = new Node(walkable, worldPoint, x, y, (int)z);
            }
        }
    }

    float GetHeightAtWorldPosition(Vector3 worldPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(worldPosition + Vector3.up * 50f, Vector3.down, out hit, 100f, unWalableMask))
        {
            return hit.point.y;
        }
        return worldPosition.y;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbois = new List<Node>();
        for(int x = -1; x<=1; x++)
        {
            for(int y = -1; y<=1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;


                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY )
                {
                    neighbois.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbois;
    }

    private Vector3 GetBottomLeftPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 50f, Vector3.down, out hit, 100f, groundMask))
        {
            return hit.point - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        }
        return transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2 - xOffset) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2 - zOffset) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position - new Vector3(0, 10, 0), new Vector3(gridWorldSize.x, 1, gridWorldSize.y));


        if(onlyDisplayPathGizmos)
        {
            if(path != null)
            {
                foreach(Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
                }
            }
        }
        else
        {
            if (grid != null)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    for (int y = 0; y < gridSizeY; y++)
                    {
                        Node currentNode = grid[x, y];
                        Gizmos.color = (currentNode.walkable) ? Color.white : Color.red;

                        if (path != null)
                            if (path.Contains(currentNode))
                                Gizmos.color = Color.black;

                        Gizmos.DrawCube(currentNode.worldPosition, Vector3.one * (nodeDiameter - .1f));
                    }
                }
            }
        }
    }
        
    public void InitBakeGride()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        player = CharacterManager.Instance.transform;
        CreateGrid();
    }

    

}


#region 레거시

//private void OnDrawGizmos()
//{
//    Gizmos.DrawCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

//    if(grid != null)
//    {
//        foreach(Node i in grid)
//        {
//            Gizmos.color = (i.walkable) ? Color.white : Color.red;
//            Gizmos.DrawCube(i.worldPosition, Vector3.one * (nodeDiameter - .1f));
//        }
//    }
//}

// Unity 에디터에서 그리기용



/*
 
 
 
  public bool onlyDisplayPathGizmos;

    public Transform player;
    public LayerMask unWalableMask;
    public LayerMask groundMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    // x, z 축 보정값
    float xOffset = 50f;
    float zOffset = 50f;

    public List<Node> path;

    private void Start()
    {
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    void CreateGrid()
    {   
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * (gridWorldSize.x / 2 - xOffset) - Vector3.up * (gridWorldSize.y / 2 - zOffset);
        for (int x = 0; x < gridSizeX; x++) 
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x   * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                // 지형 높이를 찾기 위해 Raycast 사용
                RaycastHit hit;
                if (Physics.Raycast(worldPoint + Vector3.up * 50f, Vector3.down, out hit, 100f, unWalableMask))
                {
                    worldPoint.y = hit.point.y;
                }

                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unWalableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbois = new List<Node>();
        for(int x = -1; x<=1; x++)
        {
            for(int y = -1; y<=1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;


                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY )
                {
                    neighbois.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbois;
    }

    private Vector3 GetBottomLeftPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 50f, Vector3.down, out hit, 100f, groundMask))
        {
            return hit.point - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        }
        return transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2 - xOffset) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2 - zOffset) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position - new Vector3(0, 10, 0), new Vector3(gridWorldSize.x, 1, gridWorldSize.y));


        if(onlyDisplayPathGizmos)
        {
            if(path != null)
            {
                foreach(Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
                }
            }
        }
        else
        {
            if (grid != null)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    for (int y = 0; y < gridSizeY; y++)
                    {
                        Node currentNode = grid[x, y];
                        Gizmos.color = (currentNode.walkable) ? Color.white : Color.red;

                        if (path != null)
                            if (path.Contains(currentNode))
                                Gizmos.color = Color.black;

                        Gizmos.DrawCube(currentNode.worldPosition, Vector3.one * (nodeDiameter - .1f));
                    }
                }
            }
        }
    }
        
    public void InitBakeGride()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        player = CharacterManager.Instance.transform;
        CreateGrid();
    }

    
 
 
 
 
 */


#endregion

