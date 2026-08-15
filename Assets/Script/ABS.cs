using UnityEngine;
using System.Linq;
using ABSnamespace;

namespace ABSnamespace
{
    public class ABS
    {
        private WheelCollider[] colWheels;
        float slipThreshold = 0.3f;
        int brakingForce = 20000;
        int gentleBreaking = 2000;
        public void CombineLists()
        {
            colWheels = Car.Instance.frontWheels.Concat(Car.Instance.backWheels).ToArray();
        }
        public void ApplyABS()
        {
            float currentBrakes = Car.Instance.brakesActive;
            float velocity = Car.Instance.rigid.linearVelocity.magnitude;
            foreach (WheelCollider wheel in colWheels)
            {
                WheelHit hit;
                if(wheel.GetGroundHit(out hit))
                {
                    float forwardSlipAmount = hit.forwardSlip;
                    if (forwardSlipAmount > slipThreshold && velocity > 5f)
                    {
                        currentBrakes = gentleBreaking;
                    }
                    else currentBrakes = brakingForce;
                }
            }
        }
    }
}