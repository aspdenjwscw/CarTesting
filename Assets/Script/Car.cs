using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour{
    public Rigidbody rigid;
    public WheelCollider wheel1, wheel2, wheel3, wheel4;
    public float drivespeed, steerspeed, verticleForward, verticleBackwards;
    float horizontalInput, verticalInput;
    public KeyCode forward, backwards;
    
    forward = KeyCode.W

    void Update(){

        if(Input.GetKey(KeyCode.forward))
        {
            verticleForward = 1
            return
        }
        if (Input.GetKey(KeyCode.W))
        {
            verticleBackwards = -1
            return
        }
        verticleInput = 

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        float motor = verticalInput * drivespeed;
        wheel1.motorTorque = motor;
        wheel2.motorTorque = motor;
        wheel3.motorTorque = motor;
        wheel4.motorTorque = motor;
        wheel1.steerAngle = steerspeed * horizontalInput;
        wheel2.steerAngle = steerspeed * horizontalInput;
    }
}
