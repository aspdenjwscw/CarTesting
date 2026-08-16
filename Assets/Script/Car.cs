using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABSnamespace;



public class Gear
{
    public float ratio { get; private set; }
    public float gear { get; private set; }
    public bool reverse;
    public string gearTitle { get; private set; }

    public Gear(float gearRatio, float currentGear, bool isReverse, string gearName)
    {

        ratio = gearRatio;
        gear = currentGear;
        reverse = isReverse;
        gearTitle = gearName;
    }
}

public class Car : MonoBehaviour{
    public Rigidbody rigid;
    public WheelCollider[] frontWheels;
    public WheelCollider[] backWheels;
    public float drivespeed, steerspeed;
    [HideInInspector] public float horizontalInput, verticalInput, smoothVerticalInput, accelerationSpeed, verticalForward, verticalBackwards, currentYRot, engineRPM, redlineRPM, reverseRedlineRPM, carCrashWheelsActive;
    public float engineActive;
    private KeyCode forwardKey, backwardsKey, brakingKey;
    public int currentGear;
    public bool velocityForward, velocityBackwards, braking, shifting, reverse;
    public Gear[] gears; //Do a double Array, and then a dictionary to chose based on the car.
    public AnimationCurve gearPowerScailingCurve;
    public static Car Instance { get; private set; }
    private ABS abs;
    private Shifting shift;

    void Start(){
        Instance = this;
        abs = new ABS();
        shift = new Shifting();
        forwardKey = KeyCode.W;
        backwardsKey = KeyCode.S;
        brakingKey = KeyCode.Space; 
        redlineRPM = 6000f;
        accelerationSpeed = 3f;
        braking = false;
        reverseRedlineRPM = -3000f;
        carCrashWheelsActive = 1f;
        engineActive = 1f;
        shifting = false;
        reverse = false;

        Vector3 autoCoM = rigid.centerOfMass;
        rigid.centerOfMass = new Vector3(0f, autoCoM.y - 0.75f, autoCoM.z);
        //Setting the gears values.
        gears = new Gear[]
        {
            new Gear(-3.5f, -1f, false, "Reverse"),
            new Gear(0f, 0f, false, "Neutral"),
            new Gear(5f, 1f, false, "First Gear"),
            new Gear(3.3f, 2f, false, "Second Gear"),
            new Gear(2.1f, 3f, false, "Third Gear"),
            new Gear(1.6f, 4f, false, "Fourth Gear"),
            new Gear(1.2f, 5f, false, "Fifth Gear"),
            new Gear(1f, 6f, false, "Sixth Gear"),
            new Gear(0.75f, 7f, false, "Seventh Gear")
        };
        currentGear = 1;
    }


    void Update(){
        

        if (Input.GetKey(forwardKey) && !braking)
        {
            verticalForward = 1f;
        }
        else
        {
            verticalForward = 0f;
        }
        if (Input.GetKey(backwardsKey) && !braking)
        {
            verticalBackwards = -1f;
            reverse = true;
        }
        else
        {
            verticalBackwards = 0f;
            reverse = false;
        }
        if(Input.GetKey(brakingKey))
        {
            braking = true;
        }
        else
        {
            braking = false;
        }


        if (Input.GetKey(KeyCode.R))
        {
            currentYRot = rigid.transform.eulerAngles.y;
            rigid.transform.eulerAngles = new Vector3(0f, currentYRot, 0f);
            Vector3 carPos = rigid.transform.position;
            carPos.y += 0.3f;
            rigid.transform.position = carPos;
        }
    }

    void FixedUpdate()
    {
        verticalInput = verticalBackwards + verticalForward;
        Debug.Log(verticalInput);
        horizontalInput = Input.GetAxis("Horizontal");
        
        engineRPM = ((backWheels[0].rpm * 4.56f * gears[currentGear].ratio) + (backWheels[1].rpm * 4.56f * gears[currentGear].ratio)) / 2; // Multiplying by negative 1 since I accidentally made the wheels work backwards
        
        shift.MaybeShift();

        if (engineRPM > redlineRPM && !shifting)
        {
            engineActive = 0f;
        }
        else if (engineRPM < redlineRPM && !shifting) engineActive = 1f;
        if (engineRPM < reverseRedlineRPM && !shifting) engineActive = 0f;
        
        else if (engineRPM > reverseRedlineRPM && !shifting) engineActive = 1f;


        
        


        smoothVerticalInput = Mathf.MoveTowards(smoothVerticalInput, verticalInput, accelerationSpeed * Time.fixedDeltaTime);
        float motorMultiplyer = gearPowerScailingCurve.Evaluate(currentGear);
        float motor = smoothVerticalInput * 4.56f * gears[currentGear].ratio * drivespeed * engineActive * motorMultiplyer; 
        
        foreach (WheelCollider wheel in frontWheels)
        {
            float currentBrake;
            if (braking) currentBrake = abs.ApplyABS(wheel);
            else currentBrake = 0f;
            if (!braking) wheel.motorTorque = motor * carCrashWheelsActive;
            else wheel.motorTorque = 0f;
            wheel.brakeTorque = currentBrake * 0.7f;
            wheel.steerAngle = steerspeed * horizontalInput;
            if (wheel.steerAngle > 20f) wheel.steerAngle = 20f;
            if (wheel.steerAngle < -20f) wheel.steerAngle = -20f;
        }
        foreach (WheelCollider wheel in backWheels)
        {
            float currentBrake;
            if (braking) currentBrake = abs.ApplyABS(wheel);
            else currentBrake = 0f;
            if (!braking) wheel.motorTorque = motor * carCrashWheelsActive;
            else wheel.motorTorque = 0f;
            wheel.brakeTorque = currentBrake * 0.3f;
        }
    }

    public void WheelsActive(bool wheelsActive)
    {
        if (!wheelsActive)
        {
            carCrashWheelsActive = 0f;
        }
        else if (wheelsActive)
        {
            carCrashWheelsActive = 1f;
        }
    }
}
