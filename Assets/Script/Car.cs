using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using ABSnamespace;

public class Car : MonoBehaviour{
    public Rigidbody rigid;
    public WheelCollider[] frontWheels;
    public WheelCollider[] backWheels;
    public float drivespeed, steerspeed;
    [HideInInspector] public float horizontalInput, verticalInput, smoothVerticalInput, accelerationSpeed, verticalForward, verticalBackwards, horizontalLeft, horizontalRight, currentYRot, engineRPM, redlineRPM, reverseRedlineRPM, carCrashWheelsActive, finalGearRatio, autoComShift;
    public float engineActive;
    public ParticleSystem smoke;
    private InputControl forwardKey, backwardsKey, leftKey, rightKey, brakingKey, unstuckKey, resetKey;
    public int currentGear;
    public bool braking, shifting, reverse;
    public AnimationCurve gearPowerScailingCurve;
    public static Car Instance { get; private set; }
    public Keybinds keybinds;
    private ABS abs;
    private Shifting shift;
    public float[] gears;
    public string menuSelectedCar;
    public CarSelector carSelector;
    private SettingMenu keys;

    private void SetCarKeybinds()
    {
        forwardKey = keys.forwardsControl;
        backwardsKey = keys.backwardsControl;
        leftKey = keys.leftControl;
        rightKey = keys.rightControl;
        brakingKey = keys.brakeControl;
        unstuckKey = keys.unstuckControl;
        resetKey = keys.resetControl;
    }

    void Awake()
    {
        Instance = this;
        abs = new ABS();
        shift = new Shifting();
        carSelector.SelectCar(menuSelectedCar);
    }

    void Start() 
    {
        keys = SettingMenu.Instance;
        SetCarKeybinds();
        accelerationSpeed = 3f;
        braking = false;
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
        

        if(((ButtonControl)brakingKey).isPressed)
        {
            braking = true;
        }
        else
        {
            braking = false;
        }
        if (((ButtonControl)forwardKey).isPressed && !braking)
        {
            verticalForward = 1f;
        }
        else
        {
            verticalForward = 0f;
        }
        if (((ButtonControl)backwardsKey).isPressed && !braking)
        {
            verticalBackwards = -1f;
            reverse = true;
        }
        else
        {
            verticalBackwards = 0f;
            reverse = false;
        }
        if (((ButtonControl)rightKey).isPressed && !braking)
        {
            horizontalRight = 1f;
        }
        else
        {
            horizontalRight = 0f;
        }
        if (((ButtonControl)leftKey).isPressed && !braking)
        {
            horizontalLeft = -1f;
        }
        else
        {
            horizontalLeft = 0f;
        }

        if (((ButtonControl)unstuckKey).isPressed)
        {
            currentYRot = rigid.transform.eulerAngles.y;
            rigid.transform.eulerAngles = new Vector3(0f, currentYRot, 0f);
            Vector3 carPos = rigid.transform.position;
            carPos.y += 0.3f;
            rigid.transform.position = carPos;
        }

        if (((ButtonControl)resetKey).isPressed)
        {
            //Logic Here
        }

        verticalInput = verticalBackwards + verticalForward;
        horizontalInput = horizontalLeft + horizontalRight;
    }

    void FixedUpdate()
    {
        Debug.Log(verticalInput);
        
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
        Debug.Log(motor);

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
