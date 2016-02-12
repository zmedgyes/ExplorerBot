using UnityEngine;
using System.Collections;

public class TestPath : MonoBehaviour {
    public Grid grid;

	void Update () {
        Node node = grid.NodeFromWorldPoint(transform.position);
        node.walkable = false;
	}
}
