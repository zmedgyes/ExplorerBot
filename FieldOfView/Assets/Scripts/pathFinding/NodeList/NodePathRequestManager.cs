using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NodePathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static NodePathRequestManager instance;
   NodeAStarPathfinding pathfinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<NodeAStarPathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, HashSet<Node> dynamicBlocked, Action<List<Node>, bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, dynamicBlocked, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.dynamicBlocked);
        }
    }

    public void FinishedProcessingPath(List<Node> path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public HashSet<Node> dynamicBlocked;
        public Action<List<Node>, bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, HashSet<Node> _dynamicBlocked, Action<List<Node>, bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            dynamicBlocked = _dynamicBlocked;
        }

    }
}
