using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour{
    public Rigidbody rigid;
    public WheelCollider wheel1, wheel2, wheel3, wheel4;
    public float drivespeed, steerspeed;
    float horizontalInput, verticalInput, verticalForward, verticalBackwards;
    private KeyCode forward, backwards;
    
    void Start(){
        forward = KeyCode.W; //change forward to equal a different variable that we import from the settings menu later on.
                             //This is just temporary until we make a settings menu, but will make it easier to change to when we do.
        backwards = KeyCode.S;
    }


    void Update(){

        if(Input.GetKey(forward))
        {
            verticalForward = 1f;
        }
        else if(!Input.GetKey(forward))
        {
            verticalForward = 0f;
        }
        if (Input.GetKey(backwards))
        {
            verticalBackwards = -1f;
        }
        else if (!Input.GetKey(backwards))
        {
            verticalBackwards = 0f;
        }

        //There are ways to change what the Input.GetAxis("Vertical"); need to be pressed to give change them
        verticalInput = verticalBackwards + verticalForward;

        horizontalInput = Input.GetAxis("Horizontal"); //Can also do the same for Horizontal at some point with the same as vertical
                                                       //also could try to put it in a loop to reduce code length
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
