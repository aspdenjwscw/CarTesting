using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear
{
    public float ratio;
    public float gear;
    public bool reverse;
    public string gearTitle;

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
    public WheelCollider wheel1, wheel2, wheel3, wheel4;
    public float drivespeed, steerspeed, shiftTime;
    float horizontalInput, verticalInput, verticalForward, verticalBackwards, brakingForce, brakesActive, currentYRot, engineRPM, engineActive, shiftingCooldown, redlineRPM;
    private KeyCode forwardKey, backwardsKey, brakingKey;
    int currentGear;
    public bool automatic, uphill, velocityForward, upShift, downShift, allowShift, shiftValueReset;
    public Gear[] gears;
    private float rpmVelocity, downShiftEngineRPM, upShiftEngineRPM;

    void Start(){
        forwardKey = KeyCode.W; //change forward to equal a different variable that we import from the settings menu later on.
                                //This is just temporary until we make a settings menu, but will make it easier to change to when we do.
        backwardsKey = KeyCode.S;
        brakingKey = KeyCode.Space;
        brakingForce = 10000f;
        shiftTime = 0.3f;
        shiftingCooldown = 0.8f;
        downShiftEngineRPM = 3000f;
        upShiftEngineRPM = 2000f;
        redlineRPM = 6000f;
        automatic = true;
        allowShift = true;

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
        Vector3 horizontalVelocity = new Vector3(rigid.linearVelocity.x, 0f, rigid.linearVelocity.z);
        float groundSpeed = horizontalVelocity.magnitude; //Calculates the speed of the car but not the direction
        Vector3 forwardDirection = transform.forward; //Calculates if it's going forward since you can reverse if the car has a forward velocity
        float dotProduct = Vector3.Dot(forwardDirection, horizontalVelocity);

        if (upShift)
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, upShiftEngineRPM, ref rpmVelocity, shiftTime);
            shiftTime -= Time.deltaTime;
            shiftingCooldown -= Time.deltaTime;
            engineActive = 0f;
            if(shiftTime <= 0)
            {
                upShift = false;
                shiftValueReset = true;
                engineActive = 1f;
            }
        }
        if (!upShift && shiftingCooldown > 0f && shiftValueReset) shiftingCooldown -= Time.deltaTime;
        if(shiftingCooldown == 0f)
        {
            allowShift = true;
        }
        if(shiftValueReset && shiftingCooldown == 0f)
        {
            shiftValueReset = false;
            shiftingCooldown = 0.8f;
            shiftTime = 0.3f;
        }
        if (downShift)
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, downShiftEngineRPM, ref rpmVelocity, shiftTime);
            shiftTime -= Time.deltaTime;
            shiftingCooldown -= Time.deltaTime;
            engineActive = 0f;
            if (shiftTime <= 0f)
            {
                downShift = false;
                shiftValueReset = true;
                engineActive = 1f;
            }
        }
        if (!downShift && shiftingCooldown > 0f && shiftValueReset) shiftingCooldown -= Time.deltaTime;
        if (shiftingCooldown == 0f)
        {
            allowShift = true;
        }
        if (shiftValueReset && shiftingCooldown == 0f)
        {
            shiftValueReset = false;
            shiftingCooldown = 0.8f;
            shiftTime = 0.3f;
        }





        if (dotProduct > 0f)
        {
            velocityForward = true;
        }

        if (Input.GetKey(forwardKey) && !Input.GetKey(brakingKey))
        {
            verticalForward = -1f;
        }
        else
        {
            verticalForward = 0f;
        }
        if (Input.GetKey(backwardsKey) && !Input.GetKey(brakingKey))
        {
            verticalBackwards = 1f;
            gears[0].reverse = true;
        }
        else
        {
            verticalBackwards = 0f;
            gears[0].reverse = false;
        }
        if(Input.GetKey(brakingKey))
        {
            brakesActive = brakingForce;
        }
        else if(!Input.GetKey(brakingKey) && !gears[0].reverse)
        {
            brakesActive= 0f;
        }

        if(groundSpeed > 0f && gears[0].reverse) 
        {
            brakesActive = brakingForce;
            engineActive = 0f;
        }

        if (Input.GetKey(KeyCode.R))
        {
            currentYRot = rigid.transform.eulerAngles.y;
            rigid.transform.eulerAngles = new Vector3(0f, currentYRot, 0f);
            Vector3 carPos = rigid.transform.position;
            carPos.y += 0.3f;
            rigid.transform.position = carPos;
            brakesActive = 1000000f;
        }
        else
        {
            brakesActive = 0f;
        }


        Debug.Log(gears[currentGear].ratio);
        Debug.Log(currentGear);
        //There are ways to change what the Input.GetAxis("Vertical"); need to be pressed to give change them
        verticalInput = verticalBackwards + verticalForward;
        if (gears[0].reverse && groundSpeed > 1 && velocityForward)
        {
            brakesActive = brakingForce;
        }
        horizontalInput = Input.GetAxis("Horizontal"); //Can also do the same for Horizontal at some point with the same as vertical
                                                       //also could try to put it in a loop to reduce code length
        float pitchAngle = transform.eulerAngles.x;
        if (pitchAngle > 180f) pitchAngle -= 360f; //This makes it so that if it goes over 180 then it goes to -180 to show its declining and makes it easier
        if (pitchAngle > 20f) uphill = true;
        if (!allowShift) engineRPM = wheel3.rpm * 4.56f * gears[currentGear].ratio;
        if(engineRPM > 3000f && !uphill && automatic && currentGear >= 1 && currentGear <7 && allowShift)
        {
            currentGear++;
            upShift = true;
        }
        else if(engineRPM > redlineRPM)
        {
            engineActive = 0f;
        }
        else if(engineRPM < redlineRPM) engineActive = 1;
        else if(engineRPM < 1500f && automatic && allowShift && currentGear > 1)
        {
            currentGear--;
            downShift = true;
        }// Problem: How do I make it not autoshift back down when the RPM drops, How do I even make the RPM drop realistically, If I use time to make the RPM drop how do I make it work.
        else if(engineRPM == 0f && gears[currentGear].gear == 1 && automatic && !gears[0].reverse)
        {
            currentGear = 1;
        }
        if (currentGear == 1 && verticalInput < 0) currentGear++;
        if (currentGear == 1 && verticalInput > 0) currentGear--;
    }

    void FixedUpdate()
    {
        float motor = verticalInput * 4.56f * gears[currentGear].ratio * drivespeed * engineActive; //You could make it if moving foward, and also your pressing backwards it will break before automatically switching to reversing.
        // The 4.56 is to replicate the FinalDriveRatio and the engineMax is to model going to high on the RPM and to stop them getting infinite Torque
        wheel3.motorTorque = motor;
        wheel4.motorTorque = motor;
        wheel3.brakeTorque = brakesActive;
        wheel4.brakeTorque = brakesActive;

        wheel1.steerAngle = steerspeed * horizontalInput;
        wheel2.steerAngle = steerspeed * horizontalInput;

        if(wheel1.steerAngle > 20f)
        {
            wheel1.steerAngle = 20f;
            wheel2.steerAngle = 20f;
        }
        if(wheel1.steerAngle < -20f)
        {
            wheel1.steerAngle = -20f;
            wheel2.steerAngle = -20f;
        }
    }
}
