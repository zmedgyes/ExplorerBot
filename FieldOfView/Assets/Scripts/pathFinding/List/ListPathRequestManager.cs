using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ListPathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static ListPathRequestManager instance;
    ListAStarPathfinding pathfinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<ListAStarPathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, List<Node> _unwalkable, List<Node> _dynamic, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, _unwalkable, _dynamic, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd,currentPathRequest.unwalkable,currentPathRequest.dynamicException);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public List<Node> unwalkable;
        public List<Node> dynamicException;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, List<Node> _unwalkable, List<Node> _dynamic, Action<Vector3[], bool> _callback)
        {

            pathStart = _start;
            pathEnd = _end;
            unwalkable = _unwalkable;
            dynamicException = _dynamic;
            callback = _callback;
        }

    }
}
