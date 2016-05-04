using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerformanceCalculator : MonoBehaviour {

    Vector3 currentPosition;
    Vector3 height;
    Rigidbody rb;
    
    public GUIText performanceText;
    private float speed;
    private float moment;
    private float way;
    private float wayFIFO;
    private System.DateTime diffTime;
    private System.DateTime otherTime;

    private float hour;
    private float min;
    private float sec;

    private List<Vector3> vectors = new List<Vector3>();
    private List<System.DateTime> wholeTime = new List<System.DateTime>();
    private List<System.DateTime> speedTime = new List<System.DateTime>();

    VariableScheduler variables;

    // Use this for initialization
    void Start () {
        variables = GetComponent<VariableScheduler>();
        height = new Vector3(0.0f, transform.position.y, 0.0f);
        currentPosition = transform.position - height;
        vectors.Add(currentPosition);
        rb = GetComponent<Rigidbody>();
        speed = variables.getMinSpeed();
        hour = 0.0f;
        min = 0.0f;
        sec = 0.0f;
        moment = variables.getMinSpeed();
        diffTime = System.DateTime.Now;
        otherTime = System.DateTime.Now;
        wholeTime.Add(System.DateTime.Now);
        wholeTime.Add(System.DateTime.Now);
    }
	
	// Update is called once per frame
	void Update () {
        SpeedCalc();
        speedTime.Add(System.DateTime.Now);
    }

    void SpeedRise()
    {
        List<float> avg = new List<float>();
        System.DateTime time = System.DateTime.Now;
        speedTime.Add(time);
        for(int i = 0; i < speedTime.Count-1; i++)
        {
            avg.Add(speedTime[i+1].Hour*60*60*1000 + speedTime[i + 1].Minute*60*1000 + speedTime[i + 1].Second*1000 + speedTime[i + 1].Millisecond);
            avg.Add(speedTime[0].Hour*60*60*1000 + speedTime[0].Minute*60*1000 + speedTime[0].Second*1000 + speedTime[0].Millisecond);

            if ((avg[0]-avg[1] > 50) && ((way - wayFIFO) != 0))
            {
                moment++;
            }
            avg.Clear();
        }
    }
    public void ClearLists()
    {
        speedTime.Clear();
        speed = variables.getMinSpeed();
    }

    void SpeedCalc()
    {
        SpeedRise();
        float T = 25;
        speed = variables.getMinSpeed()*(1 - Mathf.Pow(2.71828f,  -(variables.getESC() / T)));      //egytárolós lengő tag.
        if(speed > variables.getMaxSpeed())
        {
            speed = variables.getMaxSpeed();
        }
        /*print(variables.getMinSpeed());
        print(Mathf.Pow(2.71828f, (variables.getESC() / T)));
        print(1 - Mathf.Pow(2.71828f, (variables.getESC() / T)));
        print(speed);*/
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
        hour += wholeTime[wholeTime.Count - 1].Hour;
        min += wholeTime[wholeTime.Count - 1].Minute;
        sec += wholeTime[wholeTime.Count - 1].Second;
        if(wholeTime.Count - 2 >= 0)
        {
            hour -= wholeTime[wholeTime.Count - 2].Hour;
            min -= wholeTime[wholeTime.Count - 2].Minute;
            sec -= wholeTime[wholeTime.Count - 2].Second;
        }
        /*for (int i = 1; i < wholeTime.Count; i++)
        {
            if (i == wholeTime.Count - 1)
            {
                diffTime += wholeTime[i] - wholeTime[i - 1];
                hour += wholeTime[i].Hour - wholeTime[i - 1].Hour;
                min += wholeTime[i].Minute - wholeTime[i - 1].Minute;
                sec += wholeTime[i].Second - wholeTime[i - 1].Second;
            }
        }*/
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
        wayFIFO = way;
        way = Mathf.Sqrt(x * x + z * z);
        TimeAdd();
        UpdateText();
    }

    public void UpdateText()
    {
        performanceText.text = "Way: " + way + "\nSpeed: " + speed + "\nPower: " + "" + "\ndiffTime: " + hour + ":" + min + ":" + sec;
        //performanceText.text += "\nESCTemp: " + variables.tempESC + "\n";
    }

    public float getSpeed()
    {
        return speed;
    }
}
