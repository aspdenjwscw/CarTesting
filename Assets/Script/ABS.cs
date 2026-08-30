using UnityEngine;
using System.Collections.Generic;
using ABSnamespace;

namespace ABSnamespace
{
    public class ABS
    {
        float slipThreshold = 0.3f;
        int maxBraking = 3000;
        private int reductionRate = 10000;
        private int recoveryRate = 5000;
        int gentleBreaking = 200;

        private Dictionary<WheelCollider, float> currentBraking = new Dictionary<WheelCollider, float>();   
        public float ApplyABS(WheelCollider wheel)
        {
            if (!currentBraking.ContainsKey(wheel))
            {
                currentBraking[wheel] = maxBraking;
            }

            float velocity = Car.Instance.rigid.linearVelocity.magnitude;
            WheelHit hit;

            if(wheel.GetGroundHit(out hit))
            {
                float forwardSlipAmount = hit.forwardSlip;
                float absoluteSlipValue = Mathf.Abs(forwardSlipAmount);
                float sidewaysSlipAmout = hit.sidewaysSlip;
                float sidewaysSlipValue = Mathf.Abs(sidewaysSlipAmout);
                //Debug.Log(absoluteSlipValue);
                if (wheel.rpm == 0f && velocity > 3f) absoluteSlipValue = 1f;
                if (absoluteSlipValue > slipThreshold && velocity > 3f || sidewaysSlipAmout > slipThreshold && velocity > 1f)
                {
                    //Debug.Log("Easing");
                    currentBraking[wheel] -= reductionRate * Time.fixedDeltaTime;
                }
                else
                {
                    currentBraking[wheel] += recoveryRate * Time.fixedDeltaTime;
                }
                currentBraking[wheel] = Mathf.Clamp(currentBraking[wheel], 200f, maxBraking);
                return currentBraking[wheel];
            }

            return maxBraking;
        }
    }
}