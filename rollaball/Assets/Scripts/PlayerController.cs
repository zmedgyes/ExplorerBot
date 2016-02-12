using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float speed;
    private Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
        float hmov = Input.GetAxis("Horizontal");
        float vmov = Input.GetAxis("Vertical");
        Vector3 mov = new Vector3(0.0f, 0.0f, vmov);
        rb.MovePosition(rb.position + mov.normalized * speed);
        if (vmov < 0) { transform.Rotate(new Vector3(0.0f, -hmov, 0.0f)); }
        else { transform.Rotate(new Vector3(0.0f, hmov, 0.0f)); }
       // transform.Rotate(new Vector3(0.0f,hmov,0.0f));
       // rb.AddForce(mov * speed);
    }
}
