using UnityEngine;

public class WeatherAndStats
{
    public float lightIntensity;
    public string skybox;

    public WeatherStats(string skybox, float lightIntensity)
    {
        weather = skybox;
        light = lightIntensity;
    }


}

public class WeatherAndParticles : MonoBehaviour
{
    private static Material mainDaySkyBox;
    private static Material rainDaySkyBox;
    private static Material mainNightSkyBox;
    private static Material mainSunsetSkyBox;
    
    weatherStats = new WeatherStats[]
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
        directionalLight.intensity = 0.7f;


        //The Update class won't work without an object so this makes one that won't be deleted between scenes to make sure it all works.
        GameObject bootstrapper = new GameObject("WeatherSystem_Runtime");
        bootstrapper.AddComponent<WeatherAndParticles>();
        DontDestroyOnLoad(bootstrapper);

        DontDestroyOnLoad(lightObject);
    }

    public void ChangeWeather(int weatherID)
    {
        if (weatherID <= 4) {
            RenderSettings.skybox = list[weatherID]
            directionalLight.intensity = list2[weatherID];
            DynamicGI.UpdateEnvironment();
        }
        else if (weatherID > 4)
        {
            //Turn on and off the rain.
        }
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
            directionalLight.intensity = 0.2f;
            DynamicGI.UpdateEnvironment();
        }
        if (Input.GetKey(KeyCode.O))
        {
            RenderSettings.skybox = mainSunsetSkyBox;
            //Redos all the lighting so the skybox changes will be applied properly to everything.
            directionalLight.intensity = 1f;
            DynamicGI.UpdateEnvironment();
        }
        if (Input.GetKey(KeyCode.P))
        {
            RenderSettings.skybox = rainDaySkyBox;
            directionalLight.intensity = 0.7f;
            DynamicGI.UpdateEnvironment();
        }
    }


}
