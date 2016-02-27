using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public bool drawGizmos;
    public bool drawSeenGizmos;
    public List <Transform> playerList = new List<Transform>();
    Node[,] grid;

    public List<Node> path;

    //public List<Node> unwalkable;
    //public List<Node> dynamicUnwalkable;

    public List<Node> virtualBorder;
    public List<Node> realBorder;


    float nodeDiameter;
    int gridSizeX, gridSizeY;


    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        //unwalkable = new List<Node>();
        //dynamicUnwalkable = new List<Node>();
        virtualBorder = new List<Node>();
        realBorder = new List<Node>();
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint,x,y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        if(x>=0 && x<gridSizeX && y>=0 && y < gridSizeY) { return grid[x, y]; }
        return null;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Node> nodesInRadius(Vector3 worlPosition, float radius) {
        List<Node> ret = new List<Node>();
        Node node = NodeFromWorldPoint(worlPosition);

        if (node != null)
        {
            int ticks = Mathf.RoundToInt(radius / nodeDiameter);
            for (int x = node.gridX - ticks; x < +node.gridX+ticks; x++)
            {
                for (int y = +node.gridY - ticks; y < ticks + node.gridY; y++)
                {
                    if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
                    {
                        ret.Add(grid[x, y]);
                    }
                }
            }
        }
        return ret;
    }

    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

             if (grid != null)
             {
                 foreach (Node n in grid)
                 {
                     //Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (n.danger >2 ) { Gizmos.color = Color.red; }
                    else if (n.danger > 1) { Gizmos.color = Color.yellow; }
                    else if (n.danger > 0) { Gizmos.color = Color.green; }
                    else { Gizmos.color = Color.white; }
                    if (n.seen && drawSeenGizmos) { Gizmos.color = Color.cyan; }
                    if (!n.walkable) { Gizmos.color = Color.red; }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                 }
             }
        }
    }

}
