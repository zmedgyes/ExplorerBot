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
    Node[,] grid;

    public List<Node> path;

    float nodeDiameter;
    int gridSizeX, gridSizeY;


    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

   /* void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    */
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
        Vector3 bottomLeft = worlPosition - new Vector3(radius, 0.0f, radius);
        int ticks = Mathf.RoundToInt(2*radius/nodeDiameter);
        for (int x = 0; x < ticks; x++) {
            for (int y = 0; y < ticks; y++) {
                Node node = NodeFromWorldPoint(bottomLeft + new Vector3(x * nodeDiameter, 0.0f, y * nodeDiameter));
                if (node != null) {
                    ret.Add(node);
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
                   // if (path.Contains(n))
                     //   Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                 }
             }
        }
    }
}
