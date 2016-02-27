using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProtocolClient : MonoBehaviour {

    //public Transform self;
    public ProtocolServer server;
    public float bodyRadius = 0.5f * Mathf.Sqrt(2);

    List<Node> path = null;
    Node target = null;
    Node lastTarget;
    int currentWaypoint;
    Grid grid;
    MovementController controller;

    public List<Node> lastWalkable = new List<Node>();
    public List<Node> lastUnwalkable = new List<Node>();

    bool pathRequestPending = false;
    bool targetRequestPending = false;

    bool pathSuccess = false;
    bool targetSuccess = true;

    bool finished = false;
    bool dataSent = false;

    public Transform targetIndicator;

    // Use this for initialization
    void Start () {
        controller = GetComponent<MovementController>();
        lastTarget = target;
        //self = transform;
        server.clients.Add(this);
        grid = server.grid;

	}

    // Update is called once per frame
    void Update() {

        uploadSensorDataToServer();
        
        //ha nem( útra várás || targetre várás || nincs elérhető target)
        if (!(pathRequestPending || targetRequestPending || !targetSuccess)){
            //ha nem indulás
            if (path != null) {
                
                //ha új target van
                if (!target.Equals(lastTarget)) {
                    getNewPath(target);
                    lastTarget = target;
                }
                //ha nincs új target
                else {
                    //ha rossz a jelenlegi út 
                    if (!checkPath()) {
                        if (wrongPosition())
                        {
                            rescue();
                        }
                        if (!pathSuccess) {
                            getNewTarget(); //ha nem létezik út a jelenlegi célhoz
                        }
                        else {
                            getNewPath(path[path.Count - 1]); //ha létezik út a jelenlegi célhoz
                        }
                    }
                    //ha jó a jelenlegi út
                    else {

                        controller.moveTo(path[currentWaypoint].worldPosition);

                        //ha elértük az aktuális waypointot
                        if (waypointReached()) {
                            //ha az elért pont a target
                            if (path.Count.Equals(currentWaypoint + 1)) {
                                getNewTarget();
                            }
                            else {
                                currentWaypoint++;
                                controller.moveTo(path[currentWaypoint].worldPosition);
                            }
                        }
                    }
                }
            }
            //ha indulás
            else {
                //ha van már target
                if (target!=null) {
                    getNewPath(target);
                    lastTarget = target;
                }
                //ha még nincs target
                else {
                    if(dataSent)
                    getNewTarget();
                }
            }
        }
        //ha nincs elérhető target
        if (!targetSuccess){
            if (!finished){
                print("no more target");
                finished = true;
            }
           
        }
	}

    bool checkPath() {
        //TODO
        //grid.updatePlayerPositions(transform);
        for (int i = currentWaypoint; i < path.Count; i++)
        {
            if (path[i].danger > 0 || server.getDynamicUnwalkable(this).Contains(path[i])){
                return false;
            }
        }
        return true;
    }

    void getNewPath(Node target){
        pathRequestPending = true;
        server.requestNewPath(this, target, OnPathFound);
    }

    void getNewTarget(){
        targetRequestPending = true;
        server.requestNewTarget(this, OnTargetFound);
    }

    bool waypointReached(){
        int i = 0;
        for (i=0; i < path.Count; i++)
        {
            if (grid.NodeFromWorldPoint(transform.position).Equals(path[i]))
            {
                currentWaypoint = i;
                return true;
            }
        }
        if (grid.NodeFromWorldPoint(transform.position).Equals(path[currentWaypoint])){
            return true;
        }
        return false;
    }

    void uploadSensorDataToServer(){
        if ((lastUnwalkable.Count + lastWalkable.Count) > 0)
        {
            dataSent = true;
        }
        lastWalkable.AddRange(grid.nodesInRadius(transform.position, 0.5f));
        server.uploadSensorData(lastWalkable, lastUnwalkable,this);
        lastWalkable.Clear();
        lastUnwalkable.Clear();
        
    }

    public void OnPathFound(List<Node> newPath, bool pathSuccessful)
    {
        pathRequestPending = false;
        pathSuccess = pathSuccessful;
        if (pathSuccessful){
            path = newPath;
            currentWaypoint = 0;
        }
    }

    public void OnTargetFound(Node newTarget, bool targetSuccessful)
    {
        targetRequestPending = false;
        targetSuccess = targetSuccessful;
        if (targetSuccessful){
            target = newTarget;
            targetIndicator.position = target.worldPosition;
        }
    }

    bool wrongPosition()
    {
        Node currentNode = grid.NodeFromWorldPoint(transform.position);
        if (currentNode.danger>0 || !currentNode.walkable)
        {
            return true;
        }
        return false;
    }

    void rescue()
    {
        int i;
        for (i = currentWaypoint; i < path.Count; i++)
        {
            if(path[i].danger==0 && path[i].walkable)
            {
                controller.moveTo(path[i].worldPosition);
                currentWaypoint = i;
                break;
            }
        }
    }


    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = currentWaypoint; i < path.Count; i++)
            {
                Gizmos.color = Color.black;

                if (i == currentWaypoint)
                {
                    Gizmos.DrawLine(transform.position, path[i].worldPosition);
                }
                else {
                    Gizmos.DrawLine(path[i - 1].worldPosition, path[i].worldPosition);
                }

            }
        }
    }
}

