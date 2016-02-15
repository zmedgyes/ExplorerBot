using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{


    public Transform target;
    public Grid grid;
    float speed = 0.4f;
    Vector3[] path;
    int targetIndex=0;
    Vector3 height = new Vector3(0.0f, 0.5f, 0.0f);
    Vector3 currentWayPoint;
    Rigidbody rb;
    Vector3 directiondiff;
    Vector3 lastTargetPosition;

    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        height = new Vector3(0.0f, transform.position.y, 0.0f);
        rb = GetComponent<Rigidbody>();
        lastTargetPosition = target.position;

    }

    void FixedUpdate()
    {
        if (!target.position.Equals(lastTargetPosition))
        {
            lastTargetPosition = target.position;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            path = null;
        }
        else {
            if (path != null && Vector3.Distance(transform.position, target.position) > 0.05f)
            {
                if (Vector3.Distance(transform.position, currentWayPoint + height) < 0.05f)
                {
                    targetIndex++;
                    currentWayPoint = path[targetIndex];
                }

                if (!recheckPath())
                {
                    PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                    path = null;
                }
                else {

                    float angle = Vector3.Angle(transform.forward, (currentWayPoint) + height - transform.position);
                    if (angle > 90)
                    {
                        angle = 90 - angle;
                    }
                    //print(angle);
                    if (Mathf.Abs(angle) < 0.1f)
                    {
                        //rb.MovePosition( currentWayPoint + height);
                        //rb.MovePosition(rb.position + (transform.forward * speed * Time.deltaTime));
                        //transform.position = Vector3.MoveTowards(transform.position, currentWayPoint + height, speed * Time.deltaTime);

                    }
                    else {
                        Quaternion deltaRotation;
                        //deltaRotation = Quaternion.Euler(0.0f, Mathf.Sign(angle)*0.1f, 0.0f);
                        deltaRotation = Quaternion.Euler(0.0f, -angle, 0.0f);
                        rb.MoveRotation(rb.rotation * deltaRotation);
                        transform.LookAt(currentWayPoint + height);

                    }
                    //rb.MovePosition( currentWayPoint + height);
                    //rb.MovePosition(Vector3.MoveTowards(transform.position, currentWayPoint + height, speed * Time.deltaTime));

                    transform.position = Vector3.MoveTowards(transform.position, currentWayPoint + height, speed * Time.deltaTime);

                }
            }
            else {
                print("Unit: target reached");
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

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint+height, speed * Time.deltaTime);
            if (!recheckPath())
            {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            }
            yield return null;

        }
    }

    bool recheckPath()
    {
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
