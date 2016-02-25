using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NewTargetManager : MonoBehaviour
{

    Queue<TargetRequest> pathRequestQueue = new Queue<TargetRequest>();
    TargetRequest currentPathRequest;

    static NewTargetManager instance;
    BFSNewTarget targeting;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        targeting = GetComponent<BFSNewTarget>();
    }

    public static void RequestTarget(Vector3 position, List<Node> available, Action<Node, bool> callback)
    {
        TargetRequest newRequest = new TargetRequest(position, available, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            targeting.StartFindTarget(currentPathRequest.position, currentPathRequest.available);
        }
    }

    public void FinishedProcessingTarget(Node target, bool success)
    {
        currentPathRequest.callback(target, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct TargetRequest
    {
        public Vector3 position;
        public List<Node> available;
        public Action<Node, bool> callback;

        public TargetRequest(Vector3 _start, List<Node> _available, Action<Node, bool> _callback)
        {
            position = _start;
            available = _available;
            callback = _callback;
        }

    }
}
