using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{

    public float moveSpeed = 6;

    Rigidbody rb;
    Camera viewCamera;
    Vector3 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
        //transform.LookAt(mousePos + Vector3.up * transform.position.y);
        //velocity = new Vector3(0, 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        velocity = transform.forward*Input.GetAxisRaw("Vertical") * moveSpeed;


    }

    void FixedUpdate()
    {
        Quaternion deltaRotation;
        if (Input.GetAxisRaw("Vertical")>=0) { deltaRotation = Quaternion.Euler(0.0f, Input.GetAxisRaw("Horizontal"), 0.0f); }
        else { deltaRotation = Quaternion.Euler(0.0f, -Input.GetAxisRaw("Horizontal"), 0.0f); }
        //Quaternion deltaRotation = Quaternion.Euler(0.0f, Input.GetAxisRaw("Horizontal"),0.0f);

        rb.MoveRotation(rb.rotation * deltaRotation);
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
