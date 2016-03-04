using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadTreeNode : IHeapItem<QuadTreeNode>
{

    public QuadTree tree;
    public Vector3 worldPosition;

    public int iterationCount;
    public float radius;

    public bool walkable = true;
    HashSet<QuadTreeNode> blockedBy;

    public QuadTreeNode parent;

    public HashSet<QuadTreeNode> children;

    public float gCost;
    public float hCost;
    public QuadTreeNode successor;
    int heapIndex;

    public int gridX;
    public int gridY;

    public bool isLeaf = true;

    public Dictionary<QuadTreeNode, float> neighbours;


    public QuadTreeNode(QuadTreeNode _parent, Vector3 worldPos, int iteration, bool selfUpdate) {

        parent = _parent;

        if (parent != null) { 
            tree = parent.tree;
            gridX = parent.gridX;
            gridY = parent.gridY;
            radius = parent.radius / 2;
        }

        worldPosition = worldPos;

        iterationCount = iteration;

        blockedBy = new HashSet<QuadTreeNode>();

        children = new HashSet<QuadTreeNode>();
        neighbours = new Dictionary<QuadTreeNode, float>();

        if (selfUpdate) {

            updateNeighbours();

            foreach (KeyValuePair<QuadTreeNode, float> entry in neighbours)
            {
                entry.Key.updateNeighbours();
            }
        }
        
    }

    public QuadTreeNode(QuadTree _tree, Vector3 worldPos, int _gridX, int _gridY)
    {
        tree = _tree;
        radius = tree.startCellRadius;
        gridX = _gridX;
        gridY = _gridY;

        parent = null;

        worldPosition = worldPos;

        iterationCount = 0;



        blockedBy = new HashSet<QuadTreeNode>();

        children = new HashSet<QuadTreeNode>();
        neighbours = new Dictionary<QuadTreeNode, float>();

        updateNeighbours();

        foreach (KeyValuePair<QuadTreeNode, float> entry in neighbours)
        {
            entry.Key.updateNeighbours();
        }
    }


    public bool isWalkable(){
        if(blockedBy.Count==0 && walkable)
        {
            return true;
        }
        return false;
    }

    public void block(QuadTreeNode source){
        blockedBy.Add(source);
    }

    public void unBlock(QuadTreeNode source) {
        blockedBy.Remove(source);
    }

    public bool generateLeafs()
    {
        if (!isLeaf) { return false; }
        if (tree.maxIterationCount < iterationCount+1) { return false; }
        else
        {
            isLeaf = false;
            children.Add( new QuadTreeNode(this, worldPosition + new Vector3(radius / 2, 0, radius / 2), iterationCount + 1,false));
            children.Add( new QuadTreeNode(this, worldPosition + new Vector3(-radius / 2, 0, radius / 2), iterationCount + 1,false));
            children.Add( new QuadTreeNode(this, worldPosition + new Vector3(radius / 2, 0, -radius / 2), iterationCount + 1,false));
            children.Add( new QuadTreeNode(this, worldPosition + new Vector3(-radius / 2, 0, -radius / 2), iterationCount + 1,false));

            //blocking
            //walkable

            foreach (QuadTreeNode n in children) {
                n.updateNeighbours();
            }

            foreach(KeyValuePair<QuadTreeNode, float> entry in neighbours)
            {
                entry.Key.updateNeighbours();
            }

            neighbours.Clear();

            return true;
        }
    }

    public void revertLeafs()
    {
        if (children.Count > 0)
        {
            HashSet<QuadTreeNode> tmpNeighbours = new HashSet<QuadTreeNode>();
            foreach (QuadTreeNode n in children)
            {
                foreach (KeyValuePair<QuadTreeNode, float> entry in n.neighbours)
                {
                    tmpNeighbours.Add(entry.Key);
                }
            }
            foreach (QuadTreeNode n in children)
            {
                tmpNeighbours.Remove(n);
            }

            isLeaf = true;
            children.Clear();

            foreach (QuadTreeNode n in tmpNeighbours)
            {
                n.updateNeighbours();
                neighbours.Add(n, Vector3.Distance(worldPosition, n.worldPosition));
            }


        }
    }

    public void updateNeighbours()
    {
        HashSet<QuadTreeNode> tmpNeighbors = new HashSet<QuadTreeNode>();
        Vector3 tmpVec;
        QuadTreeNode tmpNode;

        
        int iterationDifference = tree.maxIterationCount - iterationCount;

        if (iterationDifference > 0)
        {
            int steps = Mathf.RoundToInt(Mathf.Pow(2, iterationDifference - 1));
            for (int i = 0; i <= steps; i++)
            {
                //top left
                tmpVec = worldPosition + new Vector3((2 * i + 1) * tree.minCellRadius, 0, radius + tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode!=null)
                {
                    tmpNeighbors.Add(tmpNode);
                }
                //top right
                tmpVec = worldPosition + new Vector3(-(2 * i + 1) * tree.minCellRadius, 0, radius + tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode != null)
                {
                    tmpNeighbors.Add(tmpNode);
                }
                //bottom left
                tmpVec = worldPosition + new Vector3((2 * i + 1) * tree.minCellRadius, 0, -radius - tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode != null)
                {
                    tmpNeighbors.Add(tmpNode);
                }
                //bottom right
                tmpVec = worldPosition + new Vector3(-(2 * i + 1) * tree.minCellRadius, 0, -radius - tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode != null)
                {
                    tmpNeighbors.Add(tmpNode);
                }

                //right up
                tmpVec = worldPosition + new Vector3(radius + tree.minCellRadius,0,(2 * i + 1) * tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode != null)
                {
                    tmpNeighbors.Add(tmpNode);
                }
                //right down
                tmpVec = worldPosition + new Vector3(radius + tree.minCellRadius, 0, -(2 * i + 1) * tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode != null)
                {
                    tmpNeighbors.Add(tmpNode);
                }

                //left up
                tmpVec = worldPosition + new Vector3(-radius - tree.minCellRadius, 0, (2 * i + 1) * tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode != null)
                {
                    tmpNeighbors.Add(tmpNode);
                }
                //left down 
                tmpVec = worldPosition + new Vector3(-radius - tree.minCellRadius, 0, -(2 * i + 1) * tree.minCellRadius);
                tmpNode = tree.locateNode(tmpVec);
                if (tmpNode != null)
                {
                    tmpNeighbors.Add(tmpNode);
                }
            }
        }
        else
        {
            //top
            tmpVec = worldPosition + new Vector3(2 * tree.minCellRadius, 0, 0);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
            //bottom
            tmpVec = worldPosition + new Vector3(-2 * tree.minCellRadius, 0, 0);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
            //right
            tmpVec = worldPosition + new Vector3(0,0,2 * tree.minCellRadius);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
            //left
            tmpVec = worldPosition + new Vector3(0,0,-2 * tree.minCellRadius);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
            //topright
            tmpVec = worldPosition + new Vector3(2 * tree.minCellRadius, 0, 2 * tree.minCellRadius);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
            //topleft
            tmpVec = worldPosition + new Vector3(2 * tree.minCellRadius, 0, -2 * tree.minCellRadius);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
            //bottomright
            tmpVec = worldPosition + new Vector3(-2 * tree.minCellRadius, 0, 2 * tree.minCellRadius);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
            //bottomleft
            tmpVec = worldPosition + new Vector3(-2 * tree.minCellRadius, 0, -2 * tree.minCellRadius);
            tmpNode = tree.locateNode(tmpVec);
            if (tmpNode != null)
            {
                tmpNeighbors.Add(tmpNode);
            }
        }
        neighbours.Clear();
        foreach(QuadTreeNode n in tmpNeighbors)
        {
            neighbours.Add(n, Vector3.Distance(worldPosition, n.worldPosition));
        }
        
    }


    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(QuadTreeNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    public void setTree(QuadTree _tree)
    {
        tree = _tree;
    }

    public bool containsPoint(Vector3 point)
    {
        if (Mathf.Abs(point.x - worldPosition.x) <= radius && Mathf.Abs(point.z - worldPosition.z) <= radius)
        {
            return true;
        }
        return false;
    }
}
