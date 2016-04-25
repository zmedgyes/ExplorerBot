using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VariableScheduler : MonoBehaviour {
    //Battery: 2S 20C 1000mAh 
    const int cellNumber = 2;
    const int capacity = 1000;  //[mAh]
    const float tensity = 7.4f;    //[V]

    //Brushless Electric Motor
    const int Kv = 4700;    //  RPM/[V]
    const int maxCurrent = 86;  // [A]
    const int voltage = 11; //  [V]
    const int power = 360;  // [W]

    //ESC 80A Turbo 1/12th
    const int maxCell = 2;
    const float resistance = 0.00004f; //[Ohm]
    const int operatingCurrent = 80;    //[A]
    const int boostTiming = 60;     //0deg~60deg
    const int turboSlope = 30;      //30[deg/s]=3[deg/0.1s]
    public float tempESC;   //[A]

    //Gear: Driver Ratio
    //Spur Gear 75T / Pinion Gear 32T 
    //[T] T stands for Teeth
    //75/32=2.34
    const float driverRatio = 2.34f;
    //Gear: Internal Gear Ratio
    //Final Drive Ratio 7.16 : Driver Ratio 2.34
    //7.16 / 2.34 = 3.06
    const float internalGearRatio = 3.06f;
    //Gear: Final Drive Ratio
    //nternal Gear Ratio 3.06 x Driver Ratio 2.34
    //3.06 x 2.34 = 7.16
    //The Final Drive Ratio is 7.16, which means that the Motor will turn 7.16 times, while the Wheels will turn 1 time.
    private float finalDriveRatio;

    //wheel
    private int messure = 2;    //[cm]

    //Calculated values
    private float distancePerTurn;  //wheel travelling on each turn
    private float maxW0;   //szögsebesség. Kv*tensity
    private float maxSpeed;
    private float minSpeed;
    private float acceleration;     //[m/s^2]
    private float currESC;      //[A]

	// Use this for initialization
	void Start () {
        finalDriveRatioCalc();
        w0Calc();
        distancePerTurn = messure * Mathf.PI;
        speedGears();
        //print(minSpeed);
        //print((int)minSpeed);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //Speed
    private void finalDriveRatioCalc()
    {
        finalDriveRatio = driverRatio * internalGearRatio;
    }

    private void w0Calc()
    {
        maxW0 = Kv * tensity;
    }
    
    private void speedGears()
    {
        maxSpeed = (maxW0 * distancePerTurn / internalGearRatio) / 100 * 60 / 1000;
        minSpeed = maxSpeed * 0.1f;
    }

    /*private void accCalc()
    {
        currESC = 
        acceleration = 
    }*/

    //Getters
    public float getMaxSpeed()
    {
        return maxSpeed;
    }
    public float getMinSpeed()
    {
        return minSpeed;
    }
    public float getESC()
    {
        return (float)operatingCurrent;
    }
}
