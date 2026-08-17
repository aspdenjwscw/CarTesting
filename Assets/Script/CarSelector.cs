using UnityEngine;
using System.Collections.Generic;

public struct CarData
{
    public Rigidbody carBody;
    public int carInt;
}


public class CarSelector : MonoBehaviour
{
    public GameObject[] carAndCameras = new GameObject[3];


    public WheelCollider[][] carFrontWheels = new WheelCollider[3][];
    public WheelCollider[][] carBackWheels = new WheelCollider[3][];
    public WheelCollider[] car1FrontWheels = new WheelCollider[2];
    public WheelCollider[] car1BackWheels = new WheelCollider[2];
    public WheelCollider[] car2FrontWheels = new WheelCollider[2];
    public WheelCollider[] car2BackWheels = new WheelCollider[2];
    public WheelCollider[] car3FrontWheels = new WheelCollider[2];
    public WheelCollider[] car3BackWheels = new WheelCollider[2];

    private int[] carRedlineRPM = { 6000, 6000, 6000 };
    private int[] carReverseRedlineRPM = { -3000, -3000, -3000 };
    private float[] finalGearRatios = { 4.56f, 4.56f, 4.56f };
    private float[] autoComShift = { -0.75f, -0.75f, -0.75f };

    private float[][] carGears = new float[][]
    {
        new float[] { -3.5f, 0f, 5f, 3.3f, 2.1f, 1.6f, 1.2f, 1f, 0.75f },
        new float[] { -3.5f, 0f, 5f, 3.3f, 2.1f, 1.6f, 1.2f, 1f, 0.75f },
        new float[] { -3.5f, 0f, 5f, 3.3f, 2.1f, 1.6f, 1.2f, 1f, 0.75f }
    };

    public Rigidbody car1;
    public Rigidbody car2;
    public Rigidbody car3;

    private Dictionary<string, CarData> carSelect = new Dictionary<string, CarData>();
    Car car;

    
    private void SetValues()
    {
        car = Car.Instance;
        carSelect.Add("Jeep", new CarData { carBody = car1, carInt = 0 });
        carSelect.Add("Pickup", new CarData { carBody = car2, carInt = 1 });
        carSelect.Add("ATV", new CarData { carBody = car3, carInt = 2 });
        carFrontWheels[0] = car1FrontWheels;
        carBackWheels[0] = car1BackWheels;
        carFrontWheels[1] = car2FrontWheels;
        carBackWheels[1] = car2BackWheels;
        carFrontWheels[2] = car3FrontWheels;
        carBackWheels[2] = car3BackWheels;
    }

    public void SelectCar(string selectedCar)
    {
        SetValues();
        if (string.IsNullOrEmpty(selectedCar)) selectedCar = "Jeep";
        car.rigid = carSelect[selectedCar].carBody;
        int currentCar = carSelect[selectedCar].carInt;

        carAndCameras[currentCar].SetActive(true);

        car.frontWheels = carFrontWheels[currentCar];
        car.backWheels = carBackWheels[currentCar];
        car.autoComShift = autoComShift[currentCar];
        car.redlineRPM = carRedlineRPM[currentCar];
        car.reverseRedlineRPM = carReverseRedlineRPM[currentCar];
        car.gears = carGears[currentCar];
        car.finalGearRatio = finalGearRatios[currentCar];


    }
}
