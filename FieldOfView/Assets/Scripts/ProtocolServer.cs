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
    public float targetRepelRadius = 1;

    List<Node> available = new List<Node>();
    bool finished = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //ha egyik kliensnek sincs targetje, akkor nincs több target
        bool targetsAvailable = false;
	    foreach(ProtocolClient client in clients){
            if (client.getTargetSuccess())
            {
                targetsAvailable = true;
                break;
            }
        }
        if (!targetsAvailable)
        {
            if (!finished)
            {
                print("no more target");
                finished = true;
            }
        }
	}

    public void requestNewTarget(ProtocolClient client, Action<Node, bool> callback) {
        available = new List<Node>();
        //virtualborder frissítése
        clearVirtualBorder();

        //a dinamikusan tiltott elemek kiszedése a virtualborder-ből
        HashSet<Node> dynamicBlocked = getDynamicUnwalkable(client);
        HashSet<Node> targetRepels = getOtherTargetRepels(client);
        foreach (Node n in virtualBorder)
        {
            if (!(dynamicBlocked.Contains(n) || targetRepels.Contains(n))) {
                available.Add(n);
            }
        }

        NewTargetManager.RequestTarget(client.transform.position, available, callback);
    }

    public void requestNewPath(ProtocolClient client, Node pathEnd, Action<List<Node>, bool> callback) {
        NodePathRequestManager.RequestPath(client.transform.position, pathEnd.worldPosition,getDynamicUnwalkable(client), callback);
    }

    public void uploadSensorData(List<Node> walkable,List<Node> unwalkable,ProtocolClient client) {
        //járhatatlanok és környezetük blokkolása
        foreach (Node n in unwalkable)
        {
            n.walkable = false;
            n.danger = 1;
            n.seen = true;
            realBorder.Add(n);
            foreach (Node node in grid.nodesInRadius(n.worldPosition,client.bodyRadius))
            {
                node.danger = 1;
                node.seen=true;
            }
        }
        //járhatóak megjelölése
        foreach (Node n in walkable)
        {
            n.seen = true;
            realBorder.Remove(n);
            if (isVirtualBorder(n)){
                virtualBorder.Add(n);
            }
  
        }
        //az eszköz által lefedett területet látottnak feltételezzük
        foreach (Node n in grid.nodesInRadius(client.transform.position, client.bodyRadius))
        {
            n.seen = true;
            realBorder.Remove(n);
            if (isVirtualBorder(n))
            {
                virtualBorder.Add(n);
            }
        }

    }

    //megállapítja, hogy van-e nem látott az adott node közvetlen környezetében
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

    //a virtualBorder-ből kiszedi a nem odavaló elemeket
    void clearVirtualBorder()
    {
        HashSet<Node> temp = new HashSet<Node>();
        foreach(Node n in virtualBorder){
            if (isVirtualBorder(n) && !realBorder.Contains(n) && n.danger==0)
            {
                temp.Add(n);
            }
        }
        virtualBorder = temp;
    }

    //visszatér azokkal a node-okkal amiket a requester-en kívül a többi eszköz lefed
    public HashSet<Node> getDynamicUnwalkable(ProtocolClient requester) {
        HashSet<Node> ret = new HashSet<Node>();
        foreach(ProtocolClient client in clients)
        {
            if (client != requester)
            {
                foreach(Node n in grid.nodesInRadius(client.transform.position, client.bodyRadius))
                {
                    ret.Add(n);
                }
            }
        }
        return ret;
    }

    //visszatér a requester-en kívüli eszközök targetjei által blokkolt node-okkal
    public HashSet<Node> getOtherTargetRepels(ProtocolClient requester)
    {
        
        HashSet<Node> ret = new HashSet<Node>();
        foreach (ProtocolClient client in clients)
        {
            if (client != requester && client.target!=null)
            {
                foreach (Node n in grid.nodesInRadius(client.target.worldPosition, targetRepelRadius))
                {
                    ret.Add(n);
                }
            }
        }
        return ret;
    }


    //virtualBorder kirajzolása
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Node n in available){
            Gizmos.DrawCube(n.worldPosition, Vector3.one*0.1f);
        }
    }
}
