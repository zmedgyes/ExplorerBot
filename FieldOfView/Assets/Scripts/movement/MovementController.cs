using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

    public float reachRadius; //gridradius/4
    public float rotationUnit;
    public int maxRotationSpeed;
    public int rotationDirectionDifference;
    private int movementSpeed;

    Vector3 target;
    Rigidbody rb;
    Vector3 height;
    

    // Use this for initialization
    void Start () {
        height = new Vector3(0.0f, transform.position.y, 0.0f);
        target = transform.position - height;
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (!targetReached()){
            Vector3 targetDirection = target + height - transform.position;

            float angle = Vector3.Angle(transform.forward, targetDirection);

            bool posDir = (Vector3.Angle(transform.right,targetDirection) > 90);
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
                transform.position = Vector3.MoveTowards(transform.position, target + height, movementSpeed * Time.deltaTime);
            }
        }
	}

    public void moveTo(Vector3 newTarget) {
        target = newTarget;
    }

    bool targetReached() {
        if (Vector3.Distance(target,new Vector3(transform.position.x,0.0f,transform.position.z)) <= reachRadius) {
            return true;
        }
        return false;
    }

    public void setMovementSpeed(float speed)
    {
        movementSpeed = (int)speed;
    }
}
