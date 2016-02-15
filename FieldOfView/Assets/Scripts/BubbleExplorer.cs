using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BubbleExplorer : MonoBehaviour {
    //http://www.cs.cmu.edu/~motionplanning/papers/sbp_papers/integrated1/edlinger_exploration.pdf

    List<Node> realBorder;
    List<Node> virtualBorder;
    public Grid grid;
    public Transform target;
    public float viewRadius;
    Vector3 height = new Vector3(0.0f, 0.5f, 0.0f);
    Node dest ;

    // Use this for initialization
    void Start () {
        realBorder = new List<Node>();
        virtualBorder = new List<Node>();
        height = new Vector3(0.0f, transform.position.y, 0.0f);
        dest = null;
        viewRadius = GetComponent<FieldOfView>().viewRadius;
    }
	
	// Update is called once per frame
	void Update () {
        checkBorders(transform.position, viewRadius+1);
        
        //print(virtualBorder.Count);
        if (virtualBorder.Count > 0)
        {
            Node localNode= grid.NodeFromWorldPoint(transform.position);
            if (localNode != null)
            {
                localNode.visited=true;
            }

            if (dest == null)
            {
                foreach (Node n in virtualBorder)
                {
                    if (!n.visited)
                    {
                        dest = n;
                        break;
                    }
                }
                if (dest != null)
                {
                    target.position = dest.worldPosition + height;
                }
                else
                {
                    print("Exploration complete");
                }
            }
            else {

                if (dest.danger > 0 || dest.visited)
                {
          
            
                    dest = getMostAvailableNode(virtualBorder);
                    //dest = getNearestAvailableNode(virtualBorder);

                    if (dest != null)
                    {
                        target.position = dest.worldPosition + height;
                    }
                    else
                    {
                        print("Exploration complete");
                    }
                }
            }

        }

    	
	}
    void checkBorders(Vector3 position, float radius)
    {
        List<Node> nodesInRange=grid.nodesInRadius(position,radius);
        foreach(Node n in nodesInRange)
        {
            if (n.danger==0) {
                if (n.seen) {
                    bool isBorder = isVirtualBorder(n);
                    
                    if (virtualBorder.Contains(n) && !isBorder) {
                        virtualBorder.Remove(n);
                    }
                    else {
                        if (!virtualBorder.Contains(n) && isBorder) {
                            virtualBorder.Add(n);
                        }
                    }
                }
            }
            else
            {
                if (!realBorder.Contains(n)){
                    realBorder.Add(n);
                }
            }
        }
        clearVirtualBorder();
    }

    bool isVirtualBorder(Node n){
        List<Node> neighbours = grid.GetNeighbours(n);
        foreach(Node node in neighbours)
        {
            if (!node.seen)
            {
                return true;
            }
        }
        return false;
    }

    void clearVirtualBorder()
    {
        foreach(Node n in realBorder)
        {
            if (virtualBorder.Contains(n))
            {
                virtualBorder.Remove(n);
            }
        }
    }

    Node getNearestAvailableNode(List<Node> nodeList)
    {
        Node ret=null;
        float minDist=-1;
        foreach(Node n in nodeList)
        {
            if (minDist < 0)
            {
                if (!n.visited && n.danger == 0) { 
                    minDist = Vector3.Distance(transform.position, n.worldPosition);
                    ret = n;
                }
            }
            else {
                if (minDist > Vector3.Distance(transform.position, n.worldPosition))
                {
                    if (!n.visited && n.danger == 0)
                    {
                        minDist = Vector3.Distance(transform.position, n.worldPosition);
                        ret = n;
                    }
                }
            }
        }

        return ret;
    }

    Node getMostAvailableNode (List < Node > nodeList)
    {
        BFSSearch bfs = new BFSSearch();
        return bfs.searchNearestNode(transform.position, nodeList, grid);
    }

}
