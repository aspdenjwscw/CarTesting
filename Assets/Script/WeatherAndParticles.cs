using UnityEngine;
using UnityEngine.SceneManagement;

public class WeatherStats
{
    public float light;
    public Material weather;

    public WeatherStats(Material skybox, float lightIntensity)
    {
        weather = skybox;
        light = lightIntensity;
    }
}


public class WeatherAndParticles : MonoBehaviour
{
    public static WeatherAndParticles Instance { get; private set; }
    private static  ParticleSystem rain;
    private static Material mainDaySkyBox;
    private static Material rainDaySkyBox;
    private static Material mainNightSkyBox;
    private static Material mainSunsetSkyBox;
    int currentWeather = 0;
    bool currentRain = false;
    public string setScene;
    public string setCar;

    private void Awake()
    {
        Instance = this;
    }

    public WeatherStats[] weatherStatus = new WeatherStats[]{
        //0 = Day, 1 = Nights, 2 = Sunset, 3 = Cloudy
        new WeatherStats(mainDaySkyBox, 1f),
        new WeatherStats(mainNightSkyBox, 0.2f),
        new WeatherStats(mainSunsetSkyBox, 0.5f),
        new WeatherStats(rainDaySkyBox, 1f)
    };


    public void ChangeWeather(int weatherID)
    {
        if (weatherID <= 3)
        {
            RenderSettings.skybox = weatherStatus[weatherID].weather;
            directionalLight.intensity = weatherStatus[weatherID].light;
            DynamicGI.UpdateEnvironment();
            currentWeather = weatherID;
        }
        if (weatherID == 4)
        {
            currentRain = true;
            rain.Play();
        }
        else if (weatherID == 5)
        {
            currentRain = false;
            rain.Stop();
        }
    }

    public void SetCurrentWeather()
    {
        Debug.Log(currentWeather);
        ChangeWeather(currentWeather);

        if(currentRain)
        {
            ChangeWeather(4);
        }
        else
        {
            ChangeWeather(5);
        }

    }

    private static Light directionalLight;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        mainDaySkyBox = Resources.Load<Material>("mainDay");
        rainDaySkyBox = Resources.Load<Material>("rainDay");
        mainNightSkyBox = Resources.Load<Material>("mainNight");
        mainSunsetSkyBox = Resources.Load<Material>("mainSunset");
        FindRain();
        //rain = GameObject rainObject = GameObject.Find("/rain");

        GameObject lightObject = new GameObject("DownDirectionalLight");
        directionalLight = lightObject.AddComponent<Light>();
        directionalLight.type = LightType.Directional;
        directionalLight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        directionalLight.intensity = 0.5f;


        //The Update class won't work without an object so this makes one that won't be deleted between scenes to make sure it all works.
        GameObject bootstrapper = new GameObject("WeatherSystem_Runtime");
        bootstrapper.AddComponent<WeatherAndParticles>();
        DontDestroyOnLoad(bootstrapper);
        DontDestroyOnLoad(lightObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneChangeTriggered;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChangeTriggered;
    }


    public void OnSceneChangeTriggered(Scene scene, LoadSceneMode mode)
    {
        FindRain();        
    }
    void Start()
    {
        FindRain();
    }

    private static void FindRain()
    {
        rain = null;
        GameObject rainObject = GameObject.Find("CarFollowingCamera/Rain");

        if (rainObject != null)
        {
            rain = rainObject.GetComponent<ParticleSystem>();
        }
        else
        {
            rainObject = GameObject.Find("Rain");
            if (rainObject != null)
            {
                rain = rainObject.GetComponent<ParticleSystem>();
            }
        }
        if (rain != null)
        {
            rain.Stop();
        }

    }

}
