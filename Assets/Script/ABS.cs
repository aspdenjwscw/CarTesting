using UnityEngine;
using ABSnamespace;

namespace ABSnamespace
{
    public class ABS
    {
        float slipThreshold = 0.3f;
        int brakingForce = 3000;
        int gentleBreaking = 200;
        public float ApplyABS(WheelCollider wheel)
        {
            float velocity = Car.Instance.rigid.linearVelocity.magnitude;
            WheelHit hit;
            if(wheel.GetGroundHit(out hit))
            {
                float forwardSlipAmount = hit.forwardSlip;
                float absoluteSlipValue = Mathf.Abs(forwardSlipAmount);
                Debug.Log(absoluteSlipValue);
                if (wheel.rpm == 0f && velocity > 3f) forwardSlipAmount = 1f;
                if (absoluteSlipValue > slipThreshold && velocity > 3f)
                {
                    Debug.Log("Easing");
                    return gentleBreaking;
                }
                else
                {
                    return brakingForce;
                }
            }

            return brakingForce;
        }
    }
}