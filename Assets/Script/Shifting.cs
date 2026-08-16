using UnityEngine;

public class Shifting
{
    Car car = Car.Instance;
    bool allowShift = true;
    bool firstFrame;
    float shiftTime = 0.3f;
    float shiftingCooldown = 0.8f;

    public void MaybeShift()
    {
        if (allowShift)
        {
            float horizontalVelocity = Vector2(car.rigid.linearVelocity.x, car.rigid.linearVelocity.z);
            float groundSpeed = horizontalVelocity.magnitude;
            float pitchAngle = transform.eulerAngles.x;
            if (pitchAngle > 180f) pitchAngle -= 360f;
            if (pitchAngle < -20f) uphill = true;
            else uphill = false;
            if (engineRPM > 3000f && !uphill && car.currentGear >= 1 && car.currentGear <= 7 && groundSpeed >= 5f)
            {
                firstFrame = true;
                StartCoroutine(UpShift());
            }
            if (engineRPM < 1500f && allowShift && currentGear > 2)
            {
                DownShift();
            }
        }
        else return;
    }
    IEnumerator UpShift()
    {
        while (true)
        {
            car.Currentgear++;
            car.engineActive = 0f;
            allowShift = false;
            yield return new WaitForSeconds(shiftTime);

            car.engineActive = 1f;

            yield return new WaitForSeconds(shiftingCooldown);
            allowShift = true;
            yield break;
        }
    }

    IEnumerator DownShift()
    {
        car.Currentgear--;
    }


    if (upShift)
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, upShiftEngineRPM, ref rpmVelocity, shiftTime);
            //shiftTime -= Time.deltaTime;
            //shiftingCooldown -= Time.deltaTime;
            //engineActive = 0f;
            //if(shiftTime <= 0)
            //{
            //    upShift = false;
            //    shiftValueReset = true;
            //}
        }
        if (!upShift && shiftingCooldown > 0f && shiftValueReset) shiftingCooldown -= Time.deltaTime;
    if (shiftingCooldown <= 0f)
    {
        allowShift = true;
    }
    if (shiftValueReset && shiftingCooldown <= 0f)
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
        }
    }
    if (!downShift && shiftingCooldown > 0f && shiftValueReset) shiftingCooldown -= Time.deltaTime;
    if (shiftingCooldown <= 0f)
    {
        allowShift = true;
    }
    if (shiftValueReset && shiftingCooldown <= 0f)
    {
        shiftValueReset = false;
        shiftingCooldown = 0.8f;
        shiftTime = 0.3f;
    }
}
