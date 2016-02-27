using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{


    //public Transform target;

    Vector3 targetPosition;
    public Grid grid;
    public float movementSpeed = 1f;
    public float rotationSpeed;
    public float rotationUnit = 0.5f;
    public float rotationDirectionDifference=30;
    public List<Node> dynamicException =new List<Node>();

    int maxRotationSpeed;

    Vector3[] path;
    int targetIndex=0;
    Vector3 height = new Vector3(0.0f, 0.5f, 0.0f);
    Vector3 currentWayPoint;
    Rigidbody rb;
    Vector3 directiondiff;
    Vector3 lastTargetPosition;

    void Start()
    {
        targetPosition = transform.position;
        maxRotationSpeed = Mathf.RoundToInt(rotationSpeed*Time.deltaTime / rotationUnit);
        height = new Vector3(0.0f, transform.position.y, 0.0f);
        rb = GetComponent<Rigidbody>();
        lastTargetPosition = transform.position;
        grid.playerList.Add(transform);


    }

    void FixedUpdate()
    {
        targetPosition = GetComponent<BubbleExplorer>().targetPosition;

        if (!targetPosition.Equals(lastTargetPosition))
        {
            lastTargetPosition = targetPosition;
            PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
           // ListPathRequestManager.RequestPath(transform.position, targetPosition, grid.unwalkable, new List<Node>(),OnPathFound);
            path = null;
        }
        else {
            if (path != null && Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                if (Vector3.Distance(transform.position, currentWayPoint + height) < 0.05f)
                {
                    targetIndex++;
                    currentWayPoint = path[targetIndex];
                }

                if (!recheckPath())
                {
                    PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
         
           
                    //ListPathRequestManager.RequestPath(transform.position, targetPosition, grid.unwalkable,grid.dynamicUnwalkable, OnPathFound);
                    path = null;
                }
                else {
                                
                    float angle = Vector3.Angle(transform.forward, (currentWayPoint) + height - transform.position);

                    bool posDir = (Vector3.Angle(transform.right, (currentWayPoint) + height - transform.position) > 90);
                    int rot = Mathf.RoundToInt(angle / rotationUnit);

                    if (rot > maxRotationSpeed)
                    {
                        rot = maxRotationSpeed;
                    }
                    Quaternion deltaRotation;
                    if (posDir)
                    {
                        deltaRotation = Quaternion.Euler(0.0f, -(rot * rotationUnit), 0.0f);
                    }
                    else {
                        deltaRotation = Quaternion.Euler(0.0f, (rot * rotationUnit), 0.0f);
                    }
                    rb.MoveRotation(rb.rotation * deltaRotation);
                    if (Mathf.Abs(angle) < rotationDirectionDifference)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, currentWayPoint + height, movementSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            currentWayPoint = path[0];
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint+height, movementSpeed * Time.deltaTime);
            if (!recheckPath())
            {
                PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
            }
            yield return null;

        }
    }

    bool recheckPath()
    {
        //grid.updatePlayerPositions(transform);
        for (int i = 0; i < path.Length; i++) {
            if (grid.NodeFromWorldPoint(path[i]).danger > 0) {
                return false;
            }
        }
        return true;
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                //Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
                
            }
            Gizmos.color = Color.white;
            Gizmos.DrawCube(currentWayPoint, Vector3.one*1.5f);
            //Gizmos.DrawLine(transform.position, directiondiff);
        }
    }
}
