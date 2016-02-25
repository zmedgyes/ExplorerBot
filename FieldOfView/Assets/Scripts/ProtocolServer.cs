using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class ProtocolServer : MonoBehaviour {

    public HashSet<Node> virtualBorder = new HashSet<Node>();
    public HashSet<Node> realBorder = new HashSet<Node>();
    public HashSet<Node> dynamic = new HashSet<Node>();
    public List<ProtocolClient> clients= new List<ProtocolClient>();
    public Grid grid;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void requestNewTarget(ProtocolClient client, Action<Node, bool> callback) {
        List<Node> available = new List<Node>();
        clearVirtualBorder();
        foreach (Node n in virtualBorder)
        {
            available.Add(n);
        }
        NewTargetManager.RequestTarget(client.transform.position, available, callback);
    }

    public void requestNewPath(ProtocolClient client, Node pathEnd, Action<List<Node>, bool> callback) {
        NodePathRequestManager.RequestPath(client.transform.position, pathEnd.worldPosition, callback);
    }

    public void uploadSensorData(List<Node> walkable,List<Node> unwalkable) {
        foreach(Node n in unwalkable)
        {
            n.walkable = false;
            n.danger = 1;
            n.seen = true;
            realBorder.Add(n);
            foreach (Node node in grid.nodesInRadius(n.worldPosition, 1.0f))
            {
                node.danger = 1;
            }
            //virtualBorder.Remove(n);
        }

        foreach (Node n in walkable)
        {
            //n.walkable = true;
            n.seen = true;
            realBorder.Remove(n);
            if (isVirtualBorder(n)){
                virtualBorder.Add(n);
            }
            else{
               // virtualBorder.Remove(n);
            }
        }
    }

    bool isVirtualBorder(Node n){
        List<Node> neighbours = grid.GetNeighbours(n);
        foreach (Node node in neighbours)
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
        HashSet<Node> temp = new HashSet<Node>();
        foreach(Node n in virtualBorder){
            if (isVirtualBorder(n) && !realBorder.Contains(n))
            {
                temp.Add(n);
            }
        }
        virtualBorder = temp;
    }
}
