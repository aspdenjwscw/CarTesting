using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearInfo
{
    public float acceleration;
    public float topSpeed;


}

public class Car : MonoBehaviour{
    public Rigidbody rigid;
    public WheelCollider wheel1, wheel2, wheel3, wheel4;
    public float drivespeed, steerspeed;
    float horizontalInput, verticalInput, verticalForward, verticalBackwards, brakingForce, brakesActive;
    private KeyCode forwardKey, backwardsKey, brakingKey;
    public GearInfo[] gearInfo;
    public int currentGear;

    void Start(){
        forwardKey = KeyCode.W; //change forward to equal a different variable that we import from the settings menu later on.
                                //This is just temporary until we make a settings menu, but will make it easier to change to when we do.
        backwardsKey = KeyCode.S;
        brakingKey = KeyCode.Space;
        brakingForce = 100f;
    }


    void Update(){

        if(Input.GetKey(forwardKey) && !Input.GetKey(brakingKey))
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
        }
        else
        {
            verticalBackwards = 0f;
        }
        if(Input.GetKey(brakingKey))
        {
            brakesActive = brakingForce;
        }
        else
        {
            brakesActive= 0f;
        }
        //There are ways to change what the Input.GetAxis("Vertical"); need to be pressed to give change them
        verticalInput = verticalBackwards + verticalForward; 

        horizontalInput = Input.GetAxis("Horizontal"); //Can also do the same for Horizontal at some point with the same as vertical
                                                       //also could try to put it in a loop to reduce code length


    }

    void FixedUpdate()
    {
        float motor = verticalInput * drivespeed; //You could make it if moving foward, and also your pressing backwards it will break before automatically switching to reversing.
        wheel1.motorTorque = motor;
        wheel2.motorTorque = motor;
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
