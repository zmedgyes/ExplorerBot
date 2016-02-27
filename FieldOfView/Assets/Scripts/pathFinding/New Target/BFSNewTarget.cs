using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class BFSNewTarget : MonoBehaviour
{


    NewTargetManager requestManager;
    public Grid grid;

    void Awake()
    {
        requestManager = GetComponent<NewTargetManager>();
        //grid = GetComponent<Grid>();
    }


    public void StartFindTarget(Vector3 startPos, List<Node> available)
    {
        StartCoroutine(FindTarget(startPos, available));
    }

    IEnumerator FindTarget(Vector3 startPos, List<Node> available)
    {

        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node target = null;
        bool targetSuccess = false;
        List<Node> visitedNodes = new List<Node>();
        List<Node> lastAddedNodes = new List<Node>();
        List<Node> nowAddedNodes = new List<Node>();
        Node center = grid.NodeFromWorldPoint(startPos);
        if (center != null && available != null)
        {
            if (available.Count > 0)
            {
                visitedNodes.Add(center);
                lastAddedNodes.Add(center);
                int addedCount = 1;
                while (addedCount>0)
                {
                    addedCount = 0;
                    foreach (Node n in lastAddedNodes)
                    {
                        List<Node> neighbours = grid.GetNeighbours(n);

                        foreach (Node node in neighbours)
                        {
                            if (node.danger == 0 && !node.visited && !visitedNodes.Contains(node) && !grid.dynamicUnwalkable.Contains(node)
                                )
                            {
                                if (available.Contains(node))
                                {
                                    target =  node;
                                    targetSuccess = true;
                                    sw.Stop();
                                    break;
                                }
                                visitedNodes.Add(node);
                                nowAddedNodes.Add(node);
                                addedCount++;

                            }
                        }
                        if (targetSuccess) { break; }
                        
                    }
                    lastAddedNodes = nowAddedNodes;
                    nowAddedNodes = new List<Node>();
                    if (targetSuccess) { break; }
                }

            }

        }
        yield return null;

        requestManager.FinishedProcessingTarget(target, targetSuccess);

    }

}
