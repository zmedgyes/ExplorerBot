
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerformanceCalculator : MonoBehaviour {

    Vector3 currentPosition;
    Vector3 height;
    Rigidbody rb;
    
    public GUIText performanceText;
    private float speed;
    private float way;
    private System.DateTime diffTime;
    private System.DateTime otherTime;

    private float hour;
    private float min;
    private float sec;

    private List<Vector3> vectors = new List<Vector3>();
    private List<System.DateTime> wholeTime = new List<System.DateTime>();

    // Use this for initialization
    void Start () {
        height = new Vector3(0.0f, transform.position.y, 0.0f);
        currentPosition = transform.position - height;
        vectors.Add(currentPosition);
        rb = GetComponent<Rigidbody>();
        speed = 0;
        hour = 0.0f;
        min = 0.0f;
        sec = 0.0f;
        diffTime = System.DateTime.Now;
        otherTime = System.DateTime.Now;
        wholeTime.Add(System.DateTime.Now);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
    }

    public void GetPosition()
    {
        return;
    }

    public void AddPositions(Vector3 position)
    {
        vectors.Add(position);
        EndCalculating();
    }

    public void TimeAdd ()
    {
        System.DateTime time = System.DateTime.Now;
        wholeTime.Add(time);
        TimeCalculator();
    }

    void TimeCalculator()
    {
        for (int i = 1; i < wholeTime.Count; i++)
        {
            if (i == wholeTime.Count - 1)
            {
                diffTime += wholeTime[i] - wholeTime[i - 1];
                hour += wholeTime[i].Hour - wholeTime[i - 1].Hour;
                min += wholeTime[i].Minute - wholeTime[i - 1].Minute;
                sec += wholeTime[i].Second - wholeTime[i - 1].Second;
            }
        }
    }

    public void EndCalculating()
    {
        float x=0.0f, z=0.0f;
        for (int i = 1; i < vectors.Count; i++)
        {
            x += Mathf.Abs(Mathf.Abs(vectors[i].x)- Mathf.Abs(vectors[i-1].x));
            z += Mathf.Abs(Mathf.Abs(vectors[i].z) - Mathf.Abs(vectors[i - 1].z));
        }
        /*for (int i = 0; i < wholeTime.Count; i++)
        {
            diffTime += wholeTime[i];
        }*/
        way = Mathf.Sqrt(x * x + z * z);
        TimeAdd();
        UpdateText();
    }

    public void UpdateText()
    {
        performanceText.text = "Way: " + way + "\nSpeed: " + speed + "\nPower: " + "" + "\ndiffTime: " + hour + ":" + min + ":" + sec;
    }
}
