using UnityEngine;
using System.Collections;

public class ChargerServer : MonoBehaviour {

    private bool isHere;
    private Vector3 position;
    public int charging;

	// Use this for initialization
	void Start () {
        position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        isHere = false;
    }
	
	// Update is called once per frame
	void Update () {
        isHere = false;
	}

    public int AddCharge()
    {
        return charging;
    }

    public void SetPos()
    {
        isHere = true;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    void OnTriggerEnter(Collider other)
    {
        print(other.name);
    }
}
