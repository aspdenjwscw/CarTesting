using UnityEngine;

public class WeatherAndParticles : MonoBehaviour
{
    private static Material mainDaySkyBox;
    private static Material rainDaySkyBox;
    private static Material mainNightSkyBox;
    private static Material mainSunsetSkyBox;

    private static Light directionalLight;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        mainDaySkyBox = Resources.Load<Material>("mainDay");
        rainDaySkyBox = Resources.Load<Material>("rainDay");
        mainNightSkyBox = Resources.Load<Material>("mainNight");
        mainSunsetSkyBox = Resources.Load<Material>("mainSunset");

        GameObject lightObject = new GameObject("DownDirectionalLight");
        directionalLight = lightObject.AddComponent<Light>();
        directionalLight.type = LightType.Directional;
        directionalLight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        directionalLight.intensity = 1f;


        //The Update class won't work without an object so this makes one that won't be deleted between scenes to make sure it all works.
        GameObject bootstrapper = new GameObject("WeatherSystem_Runtime");
        bootstrapper.AddComponent<WeatherAndParticles>();
        DontDestroyOnLoad(bootstrapper);

        DontDestroyOnLoad(lightObject);
    }




    void Update()
    {
        //Checking if its working, and the null is so it only works if the mainDaySkybox has acutally been defined
        if (Input.GetKey(KeyCode.L) && mainDaySkyBox != null)
        {
            RenderSettings.skybox = mainDaySkyBox;
            //Redos all the lighting so the skybox changes will be applied properly to everything.
            directionalLight.intensity = 1f;
            DynamicGI.UpdateEnvironment();
        }
        if (Input.GetKey(KeyCode.K))
        {
            RenderSettings.skybox = mainNightSkyBox;
        }
    }


}
