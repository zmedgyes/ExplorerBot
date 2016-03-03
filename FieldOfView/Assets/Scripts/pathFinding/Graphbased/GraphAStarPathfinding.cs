using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class GraphAStarPathfinding : MonoBehaviour
{


    PathRequestManager requestManager;
    QuadTree grid;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<QuadTree>();
    }


    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        QuadTreeNode startNode = grid.locateNode(startPos);
        QuadTreeNode targetNode = grid.locateNode(targetPos);

        //if ((targetNode.danger < 1 && targetNode.walkable))
        if (startNode.walkable && targetNode.walkable)
        {
            ListHeap<QuadTreeNode> openSet = new ListHeap<QuadTreeNode>();
            HashSet<QuadTreeNode> closedSet = new HashSet<QuadTreeNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                QuadTreeNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    // print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (KeyValuePair<QuadTreeNode, float> entry in currentNode.neighbours)
                {
                    //if (neighbour.danger > 0 || !neighbour.walkable || closedSet.Contains(neighbour))
                    QuadTreeNode neighbour = entry.Key;

                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbour = currentNode.gCost + entry.Value;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = Vector3.Distance(neighbour.worldPosition,targetNode.worldPosition);
                        neighbour.successor = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);

    }

    Vector3[] RetracePath(QuadTreeNode startNode, QuadTreeNode endNode)
    {
        List<QuadTreeNode> path = new List<QuadTreeNode>();
        QuadTreeNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.successor;
        }
        /*Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);*/
        path.Reverse();
        List<Vector3> waypoints = new List<Vector3>();
        foreach (QuadTreeNode n in path)
        {
            waypoints.Add(n.worldPosition);
        }
        return waypoints.ToArray();

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    /*int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }*/


}
