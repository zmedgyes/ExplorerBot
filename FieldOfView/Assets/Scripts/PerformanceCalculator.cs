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
    private float power;
    private float curPower;
    private float curPowerPercent;
    private float way;
    private float esc;
    private float diffTime;
    private float escPar;
    private float maxSpeed;
    private bool haveToCharge;
    private System.DateTime otherTime;

    private float hour;
    private float min;
    private float sec;

    private List<float> wayFIFO = new List<float>();
    private List<Vector3> vectors = new List<Vector3>();
    private List<System.DateTime> wholeTime = new List<System.DateTime>();
    private List<System.DateTime> speedTime = new List<System.DateTime>();

    VariableScheduler variables;
    public ChargerServer charger;
    // Use this for initialization
    void Start () {
        charger = GetComponent<ChargerServer>();
        variables = GetComponent<VariableScheduler>();
        height = new Vector3(0.0f, transform.position.y, 0.0f);
        currentPosition = transform.position - height;
        vectors.Add(currentPosition);
        rb = GetComponent<Rigidbody>();
        speed = variables.getMinSpeed();
        maxSpeed = variables.getMaxSpeed();
        hour = 0.0f;
        min = 0.0f;
        sec = 0.0f;
        esc = 0.0f;
        escPar = variables.getESC();
        moment = variables.getMinSpeed();
        diffTime = 0.0f;
        otherTime = System.DateTime.Now;
        wholeTime.Add(System.DateTime.Now);
        wholeTime.Add(System.DateTime.Now);
        Power();
        PowerInput();
    }
	
	// Update is called once per frame
	void Update () {
        PowerInput();
        CheckCharging();
    }

    void SpeedRise()
    {
        if (curPowerPercent >= 0.0f)
        {
            List<float> avg = new List<float>();
            speedTime.Add(System.DateTime.Now);
            bool chg = false;
            if (wayFIFO.Count >= 2)
            {
                if (wayFIFO[wayFIFO.Count - 1] != wayFIFO[wayFIFO.Count - 2])
                {
                    chg = true;
                }
                else
                    chg = false;
                float transition = wayFIFO[wayFIFO.Count - 1];
                wayFIFO.Clear();
                wayFIFO.Add(transition);
            }
            if (speedTime.Count >= 2)
            {
                avg.Add(speedTime[speedTime.Count - 1].Hour * 60 * 60 * 1000 + speedTime[speedTime.Count - 1].Minute * 60 * 1000 + speedTime[speedTime.Count - 1].Second * 1000 + speedTime[speedTime.Count - 1].Millisecond);
                avg.Add(speedTime[0].Hour * 60 * 60 * 1000 + speedTime[0].Minute * 60 * 1000 + speedTime[0].Second * 1000 + speedTime[0].Millisecond);
                if ((((avg[0] - avg[1]) / 1000) >= 0) && chg)
                {
                    moment += (1.0f * ((avg[0] - avg[1]) / 10));
                }
                else
                    moment = variables.getMinSpeed();
                if ((avg[0] - avg[1] < 1))
                {
                    System.DateTime time = speedTime[0];
                    speedTime.Clear();
                    speedTime.Add(time);
                }
                else
                {
                    System.DateTime time = speedTime[speedTime.Count - 1];
                    speedTime.Clear();
                    speedTime.Add(time);
                }
                avg.Clear();
            }
            SpeedCalc();
        }
    }

    public void ClearLists()
    {
        speedTime.Clear();
        speed = variables.getMinSpeed();
    }

    void SpeedCalc()
    {
        float T = 25;
        //speed = variables.getMinSpeed()*(1 - Mathf.Pow(2.71828f,  -(variables.getESC() / T)));      //egytárolós lengő tag.
        speed = (moment * (1 - Mathf.Pow(2.71828f, -(escPar / T))));      //egytárolós lengő tag.
        /*if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }*/
        if (curPowerPercent == 0.0f)
        {
            speed = 0;
        }
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
        if(wholeTime.Count - 2 >= 0)
        {
            diffTime += (wholeTime[wholeTime.Count - 1].Hour * 60 * 60 + wholeTime[wholeTime.Count - 1].Minute * 60 + wholeTime[wholeTime.Count - 1].Second);
            diffTime -= (wholeTime[wholeTime.Count - 2].Hour * 60 * 60 + wholeTime[wholeTime.Count - 2].Minute * 60 + wholeTime[wholeTime.Count - 2].Second);
        }
        hour = (int)diffTime / (60 * 60);
        if (hour <= 0.0f)
            hour = 0.0f;
        min = (int)(((int)diffTime - hour) / 60);
        if (min <= 0.0f)
            min = 0.0f;
        sec = (int)((int)diffTime - hour * 60 * 60 - min * 60);
        if (sec <= 0.0f)
            sec = 0.0f;
       // print("diffTime: " + diffTime + " hour: " + hour + " min: " + min + " sec: " + sec);
        
    }

    public void EndCalculating()
    {
        float x=0.0f, z=0.0f;
        if(vectors.Count >= 2)
        {
            for (int i = 1; i < vectors.Count; i++)
            {
                x += Mathf.Abs(vectors[i].x - vectors[i - 1].x);
                z += Mathf.Abs(vectors[i].z - vectors[i - 1].z);
            }
            Vector3 transition = vectors[vectors.Count - 1];
            vectors.Clear();
            vectors.Add(transition);
        }
        wayFIFO.Add(way);
        way += Mathf.Sqrt(x * x + z * z);
        SpeedRise();
        TimeAdd();
        UpdateText();
    }

    public void UpdateText()
    {
        performanceText.text = "Way: " + way.ToString("n2") + "\navSpeed: " + speed.ToString("n3") + "\nPower: " + curPowerPercent.ToString("n2") + "%\ndiffTime: " + hour + ":" + min + ":" + sec;
        //performanceText.text += "\nESCTemp: " + variables.tempESC + "\n";
    }

    public float getSpeed()
    {
        SpeedCalc();
        return speed;
    }

    void Power()
    {
        power = variables.getTensity() * variables.getCapacity();   //mWh
        print(power);
        curPower = power * 60 * 60; //mWs
        //print("curPower: " + curPower);
    }

    void PowerInput()
    {
        float F = (variables.getMotorPower() * 0.9f * 1000) / variables.getMaxSpeed();    // mN
        float minTime = power / (variables.getMotorPower() * 1000);
        float P = F * speed;        //felvett teljesítmény mW-ban
        if(P > 0)
            curPower -= P;
        curPowerPercent = (curPower / (power * (60 * 60))) * 100;
        if (curPowerPercent < 0.0f)
            curPowerPercent = 0.0f;
        //print("power: "+power+" F:" + F + " P: " + P + " curPower: " + curPower);
    }
    
    public void CheckCharging()
    {
        float x = 0.0f, z = 0.0f;
        Vector3 curPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        x = Mathf.Abs(curPos.x - charger.GetPosition().x);
        z = Mathf.Abs(curPos.z - charger.GetPosition().z);
        float distence = Mathf.Sqrt(x * x + z * z);

        if (curPowerPercent < 20.0f)
            haveToCharge = true;
        else
            haveToCharge = false;
        print(haveToCharge);
    }

}