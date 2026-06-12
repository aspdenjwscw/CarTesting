using UnityEngine;

public class Wheels : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelMesh;
    public bool wheelTurn;

    void Update()
    {
        if (wheelTurn)
        {
            wheelMesh.localEulerAngles = new Vector3(wheelMesh.localEulerAngles.x, wheelCollider.steerAngle - wheelMesh.localEulerAngles.z + 90f, wheelMesh.localEulerAngles.z);
        }
        wheelMesh.Rotate(0, wheelCollider.rpm / 60 * 360 * Time.deltaTime, 0);
        //Add something that detects the surface you're on and changes the stiffness
        //Mostly difficult due to having to find a way to detect the surface change.


    }
}
