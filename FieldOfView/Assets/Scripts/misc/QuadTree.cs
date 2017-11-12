﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadTree : MonoBehaviour {

    public float startCellRadius;
    public int maxIterationCount;
    public float minCellRadius;

    public HashSet<QuadTreeNode> coreNodeSet;

	// Use this for initialization
	void Start () {
        coreNodeSet = new HashSet<QuadTreeNode>();
        minCellRadius = startCellRadius / Mathf.Pow(2, maxIterationCount);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public QuadTreeNode locateNode(Vector3 pos)
    {
        QuadTreeNode ret = null;
        foreach(QuadTreeNode n in coreNodeSet)
        {
            if (n.containsPoint(pos)) {
                ret = n;
                break;
            }
        }
        if (ret != null)
        {
            while (!ret.isLeaf)
            {
                foreach(QuadTreeNode n in ret.children)
                {
                    if (n.containsPoint(pos))
                    {
                        ret = n;
                        break;
                    }
                }
            }
        }

        return ret;
    }

    public QuadTreeNode insertNewPoint(Vector3 pos)
    {
        QuadTreeNode ret = null;
        foreach (QuadTreeNode n in coreNodeSet)
        {
            if (n.containsPoint(pos))
            {
                ret = n;
                break;
            }
        }

        //add new rootNode
        if(ret == null)
        {
            QuadTreeNode neighbour = null; ;
            int verticalDirectionIndicator = 0;
            int horizontalDirectionIndicator = 0;
            Vector3 rightPos = pos+new Vector3(2*startCellRadius,0,0);
            Vector3 leftPos = pos + new Vector3(-2 * startCellRadius, 0, 0);
            Vector3 topPos = pos + new Vector3(0, 0, 2 * startCellRadius);
            Vector3 botPos = pos + new Vector3(0, 0, -2 * startCellRadius);

            foreach (QuadTreeNode n in coreNodeSet)
            {
                if (n.containsPoint(topPos))
                {
                    verticalDirectionIndicator = -1;
                    horizontalDirectionIndicator = 0;
                    neighbour = n;
                    break;
                }
                else if (n.containsPoint(botPos))
                {
                    verticalDirectionIndicator = 1;
                    horizontalDirectionIndicator = 0;
                    neighbour = n;
                    break;
                }
                else if (n.containsPoint(rightPos))
                {
                    verticalDirectionIndicator = 0;
                    horizontalDirectionIndicator = -1;
                    neighbour = n;
                    break;
                }
                else if (n.containsPoint(leftPos))
                {
                    verticalDirectionIndicator = 0;
                    horizontalDirectionIndicator = 1;
                    neighbour = n;
                    break;
                }
            }

            if (neighbour != null)
            {
                coreNodeSet.Add(new QuadTreeNode(this, neighbour.worldPosition + new Vector3(2*horizontalDirectionIndicator * startCellRadius, 0, 2*verticalDirectionIndicator * startCellRadius),neighbour.gridX+horizontalDirectionIndicator,neighbour.gridY+horizontalDirectionIndicator ));
            }
            else
            {
                return null;
            }
            
        }

        ret = locateNode(pos);

        while (ret.generateLeafs()){
            foreach(QuadTreeNode n in ret.children){
                if (n.containsPoint(pos)){
                    ret = n;
                    break;
                }
            }
        }

        return ret;
    }

    void tryRevert(QuadTreeNode node)
    {
        if (node != null)
        {
            bool flag = true;
            foreach(QuadTreeNode n in node.parent.children)
            {
                if (!n.Equals(node))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                node.parent.revertLeafs();
                tryRevert(node.parent);
            }
        }
    }

    public void insertUnwalkable(Vector3 pos)
    {
        QuadTreeNode node = insertNewPoint(pos);
        node.walkable = false;
        tryRevert(node);
    }

    public void insertSeen(Vector3 pos)
    {
        QuadTreeNode node = insertNewPoint(pos);
        node.seen = true;
        tryRevert(node);
    }

    public HashSet<QuadTreeNode> findNodesInCone(Vector3 center, Vector3 direction, float angle, float radius)
    {
        HashSet<QuadTreeNode> ret = new HashSet<QuadTreeNode>();
        HashSet<QuadTreeNode> coreNodesInRange = getCoreNodesInRadius(center, radius);
        HashSet<QuadTreeNode> tmp;
        foreach (QuadTreeNode n in coreNodesInRange)
        {
            tmp = n.recursiveNodesInCone(center, direction, angle, radius);
            foreach (QuadTreeNode node in tmp)
            {
                ret.Add(node);
            }
        }

        return ret;
    }

    HashSet<QuadTreeNode> getCoreNodesInRadius(Vector3 center, float radius)
    {
        HashSet<QuadTreeNode> ret = new HashSet<QuadTreeNode>();
        foreach(QuadTreeNode n in coreNodeSet)
        {
            if (Vector3.Distance(center, n.worldPosition) <= (radius + startCellRadius))
            {
                ret.Add(n);
            }
        }
        return ret;
    }


}
