using UnityEngine;

public class WeatherAndParticles : Monobehaviour
{
    private static Material mainDaySkyBox;
    private static Material rainDaySkyBox;
    private static Material mainNightSkyBox;
    private static Material mainSunsetSkyBox;

    void static mainDaySkyBox = SkyBoxes.Load<Material>("mainDay")

    void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            RenderSettings.skybox = mainDaySkyBox;
        }
        if (Input.GetKey(KeyCode.K))
        {
            RenderSettings.skybox = mainNightSkyBox;
        }
    }


}
