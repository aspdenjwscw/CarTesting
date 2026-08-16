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
    [System.NonSerialized] public float shiftTime, brakesActive;
    float horizontalInput, verticalInput, smoothVerticalInput, accelerationSpeed, verticalForward, verticalBackwards, currentYRot, engineRPM, engineActive, shiftingCooldown, redlineRPM, reverseRedlineRPM, carCrashWheelsActive;
    private KeyCode forwardKey, backwardsKey, brakingKey;
    int currentGear;
    public bool automatic, uphill, velocityForward, velocityBackwards, shiftValueReset, braking;
    public Gear[] gears;
    private float rpmVelocity, downShiftEngineRPM, upShiftEngineRPM;
    public AnimationCurve gearPowerScailingCurve;
    public static Car Instance { get; private set; }
    private ABS abs;

    void Start(){
        Instance = this;
        abs = new ABS();
        forwardKey = KeyCode.W; //change forward to equal a different variable that we import from the settings menu later on.
                                //This is just temporary until we make a settings menu, but will make it easier to change to when we do.
        backwardsKey = KeyCode.S;
        brakingKey = KeyCode.Space;
        downShiftEngineRPM = 3000f;
        upShiftEngineRPM = 2000f;
        redlineRPM = 6000f;
        accelerationSpeed = 3f;
        braking = false;
        reverseRedlineRPM = -3000f;
        carCrashWheelsActive = 1f;
        engineActive = 1f;

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
            gears[0].reverse = true;
        }
        else
        {
            verticalBackwards = 0f;
            gears[0].reverse = false;
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
        //Detects if the velocity is forward or backwards as a bool.
        Vector3 horizontalVelocity = new Vector3(rigid.linearVelocity.x, 0f, rigid.linearVelocity.z);
        float groundSpeed = horizontalVelocity.magnitude; //Calculates the speed of the car but not the direction
        Vector3 forwardDirection = transform.forward; //Calculates if it's going forward since you can reverse if the car has a forward velocity
        float dotProduct = Vector3.Dot(forwardDirection, horizontalVelocity);
        if (dotProduct > 0f)
        {
            velocityForward = true;
            velocityBackwards = false;
        }
        else if (dotProduct < 0f)
        {
            velocityForward = false;
            velocityBackwards = true;
        }
        else
        {
            velocityForward = false;
            velocityBackwards = false;
        }
        if (gears[0].reverse && groundSpeed > 0f && velocityForward)
        {
            braking = true;
            engineActive = 0f;
        }
        if (verticalInput > 0f && groundSpeed > 0f && velocityBackwards && gears[currentGear].gear == -1f)
        {
            braking = true;
            engineActive = 0f;
        }

        horizontalInput = Input.GetAxis("Horizontal"); //Can also do the same for Horizontal at some point with the same as vertical
                                                       //also could try to put it in a loop to reduce code length

        float pitchAngle = transform.eulerAngles.x;
        if (pitchAngle > 180f) pitchAngle -= 360f; //This makes it so that if it goes over 180 then it goes to -180 to show its declining and makes it easier
        if (pitchAngle < -20f) uphill = true;
        else uphill = false;
        if (allowShift) engineRPM = ((backWheels[0].rpm * 4.56f * gears[currentGear].ratio) + (backWheels[1].rpm * 4.56f * gears[currentGear].ratio)) / 2; // Multiplying by negative 1 since I accidentally made the wheels work backwards
        if (engineRPM > 3000f && !uphill && automatic && currentGear >= 1 && currentGear <= 7 && allowShift && groundSpeed >= 5f)
        {
            currentGear++;
            upShift = true;
            allowShift = false;
        }
        else if (engineRPM > redlineRPM)
        {
            engineActive = 0f;
        }
        else if (engineRPM < reverseRedlineRPM) engineActive = 0f;
        else if (engineRPM < redlineRPM && !downShift && !upShift) engineActive = 1f;
        else if (engineRPM > reverseRedlineRPM && !upShift && !downShift) engineActive = 1f;
        if (engineRPM < 1500f && automatic && allowShift && currentGear > 2)
        {
            currentGear--;
            downShift = true;
            allowShift = false;
        }// Problem: How do I make it not autoshift back down when the RPM drops, How do I even make the RPM drop realistically, If I use time to make the RPM drop how do I make it work.
        if (groundSpeed <= 0.1f && automatic && allowShift && currentGear == 2 && verticalInput != 1)
        {
            currentGear--;
            downShift = true;
            allowShift = false;
        }
        else if (engineRPM == 0f && gears[currentGear].gear == 1 && automatic && !gears[0].reverse)
        {
            currentGear = 1;
        }
        if (currentGear == 1 && verticalInput > 0 && (groundSpeed <= 0.1f || velocityForward) && automatic) currentGear++;
        if (currentGear == 1 && verticalInput < 0 && (groundSpeed <= 0.1f || velocityBackwards) && automatic) currentGear--;

        //Debug.Log(groundSpeed);
        if (currentGear == 0 && !gears[0].reverse && verticalInput > 0f && groundSpeed <= 0.1f && automatic)
        {
            currentGear++;
        }

        //Make it so you can switch from reverse to forwards gears if velocity is forwards, and the same vice versa.


        //Checks if you're in reverse or forward, and allows you to shift into first or into reverse if your velocity, and input is in the right direction.
        if (velocityForward && currentGear == 0 && verticalInput > 0f && automatic)
        {
            currentGear = 1;
        }

        if (velocityBackwards && currentGear == 2 && verticalInput < 0f && automatic)
        {
            currentGear = 0;
        }


        verticalInput = verticalBackwards + verticalForward;
        smoothVerticalInput = Mathf.MoveTowards(smoothVerticalInput, verticalInput, accelerationSpeed * Time.fixedDeltaTime);
        float motorMultiplyer = gearPowerScailingCurve.Evaluate(currentGear);
        float naturalRPMLimiter = Mathf.Clamp01(1f - (engineRPM / 6000f));
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
