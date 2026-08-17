using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABSnamespace;

public class Car : MonoBehaviour{
    public Rigidbody rigid;
    public WheelCollider[] frontWheels;
    public WheelCollider[] backWheels;
    public float drivespeed, steerspeed;
    [HideInInspector] public float horizontalInput, verticalInput, smoothVerticalInput, accelerationSpeed, verticalForward, verticalBackwards, currentYRot, engineRPM, redlineRPM, reverseRedlineRPM, carCrashWheelsActive, finalGearRatio, autoComShift;
    public float engineActive;
    private KeyCode forwardKey, backwardsKey, brakingKey;
    public int currentGear;
    public bool braking, shifting, reverse;
    public AnimationCurve gearPowerScailingCurve;
    public static Car Instance { get; private set; }
    private ABS abs;
    private Shifting shift;
    private CarSelector carSelector;
    public float[] gears;
    public string menuSelectedCar;

    void Start(){
        Instance = this;
        abs = new ABS();
        shift = new Shifting();
        carSelector = new CarSelector();
        carSelector.SelectCar(menuSelectedCar);
        forwardKey = KeyCode.W;
        backwardsKey = KeyCode.S;
        brakingKey = KeyCode.Space; 
        redlineRPM = 6000;
        accelerationSpeed = 3f;
        braking = false;
        reverseRedlineRPM = -3000f;
        carCrashWheelsActive = 1f;
        engineActive = 1f;
        shifting = false;
        reverse = false;

        Vector3 autoCoM = rigid.centerOfMass;
        rigid.centerOfMass = new Vector3(0f, autoCoM.y + autoComShift, autoCoM.z);
        //Setting the gears values.
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

        verticalInput = verticalBackwards + verticalForward;
    }

    void FixedUpdate()
    {
        Debug.Log(verticalInput);
        horizontalInput = Input.GetAxis("Horizontal");
        
        engineRPM = ((backWheels[0].rpm * finalGearRatio * gears[currentGear]) + (backWheels[1].rpm * finalGearRatio * gears[currentGear])) / 2; // Multiplying by negative 1 since I accidentally made the wheels work backwards
        
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
        float motor = smoothVerticalInput * finalGearRatio * gears[currentGear] * drivespeed * engineActive * motorMultiplyer; 
        
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
