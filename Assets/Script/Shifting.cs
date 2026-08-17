using UnityEngine;
using System.Collections;

public class Shifting
{
    Car car = Car.Instance;
    bool allowShift = true;
    bool uphill = false;    
    float shiftTime = 0.3f;
    float shiftingCooldown = 0.5f;
    int upshiftRPM = 3000;
    int downshiftRPM = 1500;
    bool velocityForward, velocityBackwards;

    public void MaybeShift()
    {
        Vector2 horizontalVelocity = new Vector2(car.rigid.linearVelocity.x, car.rigid.linearVelocity.z);
        float groundSpeed = horizontalVelocity.magnitude;
        Vector2 forwardDirection = new Vector2(car.rigid.transform.forward.x, car.rigid.transform.forward.z);
        float dotProduct = Vector2.Dot(forwardDirection, horizontalVelocity);
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
        if (car.reverse && velocityForward)
        {
            car.braking = true;
            car.engineActive = 0f;
        }
        if (car.verticalInput > 0f && velocityBackwards)
        {
            car.braking = true;
            car.engineActive = 0f;
        }
        if (car.currentGear == 1 && car.verticalInput > 0 && (groundSpeed <= 0.1f || velocityForward)) car.currentGear = 2;
        else if (car.currentGear == 1 && car.verticalInput < 0 && (groundSpeed <= 0.1f || velocityBackwards)) car.currentGear = 0;
        else if (velocityBackwards && car.currentGear == 2 && car.verticalInput < 0f) car.currentGear = 1;
        else if (velocityForward && car.currentGear == 0 && car.verticalInput > 0f) car.currentGear = 1;

        if (allowShift)
        {
            float pitchAngle = car.rigid.transform.eulerAngles.x;
            if (pitchAngle > 180f) pitchAngle -= 360f;
            if (pitchAngle < -20f) uphill = true;
            else uphill = false;
            if (car.engineRPM > upshiftRPM && !uphill && car.currentGear >= 1 && car.currentGear <= 7 && groundSpeed >= 5f) //Change to Max Gear
            {
                car.StartCoroutine(UpShift());
            }
            if (car.engineRPM < downshiftRPM && allowShift && car.currentGear > 2)
            {
                car.StartCoroutine(DownShift());
            }
        }
        else return;
    }
    IEnumerator UpShift()
    {
        while (true)
        {
            car.currentGear++;
            car.engineActive = 0f;
            allowShift = false;
            car.shifting = true;
            yield return new WaitForSeconds(shiftTime);

            car.engineActive = 1f;
            car.shifting = false;

            yield return new WaitForSeconds(shiftingCooldown);
            allowShift = true;

            yield break;
        }
    }

    IEnumerator DownShift()
    {
        while (true)
        {
            car.currentGear--;
            car.engineActive = 0f;
            allowShift = false;
            car.shifting = true;
            yield return new WaitForSeconds(shiftTime);

            car.engineActive = 1f;
            car.shifting = false;

            yield return new WaitForSeconds(shiftingCooldown);
            allowShift= true;
            yield break;
        }
    }
}
