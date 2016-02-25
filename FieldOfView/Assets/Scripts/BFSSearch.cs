using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BFSSearch {

    public Node searchNearestNode(Vector3 position, List<Node> goals,Grid grid)
    {
        List<Node> visitedNodes = new List<Node>();
        List<Node> lastAddedNodes = new List<Node>();
        List<Node> nowAddedNodes = new List<Node>();
        Node center = grid.NodeFromWorldPoint(position);
        bool run = true;
        if(center!=null && goals != null)
        {
            if (goals.Count > 0)
            {
                visitedNodes.Add(center);
                lastAddedNodes.Add(center);
                int addedCount = 0;
                while (run) {
                    foreach (Node n in lastAddedNodes) {
                        List<Node> neighbours = grid.GetNeighbours(n);
                        foreach(Node node in neighbours)
                        {
                            if(node.danger==0 && !node.visited && !visitedNodes.Contains(node) && !grid.dynamicUnwalkable.Contains(node)
                                )
                            {
                                if (goals.Contains(node))
                                {
                                    return node;

                                }
                                visitedNodes.Add(node);
                                nowAddedNodes.Add(node);
                                addedCount++;
                                
                            }
                        }

                    }
                    lastAddedNodes = nowAddedNodes;
                    nowAddedNodes = new List<Node>();
                    if (addedCount == 0) { run = false; }
                    addedCount = 0;
                }

            }

        }
        return null;
    }
}
