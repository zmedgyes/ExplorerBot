using UnityEngine;
using System.Collections;

public class ChargerServer : MonoBehaviour {

    private bool isHere;
    private Vector3 position;
    public int charging;
    public float tumble;
    Vector3 rotationY;

    //public ParticleSystem chargingEffect;
    private ParticleSystem chargingEffectIn;

    Rigidbody rb;
    // Use this for initialization
    void Start () {
        chargingEffectIn = GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        isHere = false;
        chargingEffectIn.Stop();
        chargingEffectIn.Clear();
        rotationY.Set(0f, 0f, 0f);
        rotationY = rotationY.normalized*tumble;
        Quaternion deltaRotation = Quaternion.Euler(rotationY);
        //print("charger:" + position.x + " " + position.z);
    }
	
	// Update is called once per frame
	void Update () {
        rotationY = rotationY.normalized * tumble;
        Quaternion deltaRotation = Quaternion.Euler(rotationY);
        if (charging == 0)
            chargingEffectIn.Stop();
    }

    public int AddCharge()
    {
        return charging;
    }

    public void SetPosHere()
    {
        isHere = true;
        if(!chargingEffectIn.isPlaying)
            chargingEffectIn.Play();
    }
    public void SetPosnonHere()
    {
        isHere = false;
        chargingEffectIn.Stop();
        //Destroy(chargingEffect.gameObject as GameObject, chargingEffect.startLifetime);
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
