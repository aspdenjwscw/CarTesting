using UnityEngine;

public class WeatherAndParticles : MonoBehaviour
{
    private static Material mainDaySkyBox;
    private static Material rainDaySkyBox;
    private static Material mainNightSkyBox;
    private static Material mainSunsetSkyBox;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        mainDaySkyBox = Resources.Load<Material>("mainDay");


        //The Update class won't work without an object so this makes one that won't be deleted between scenes to make sure it all works.
        GameObject bootstrapper = new GameObject("WeatherSystem_Runtime");
        bootstrapper.AddComponent<WeatherAndParticles>();
        DontDestroyOnLoad(bootstrapper);
    }
    



    void Update()
    {
        //Checking if its working, and the null is so it only works if the mainDaySkybox has acutally been defined
        if (Input.GetKey(KeyCode.L) && mainDaySkyBox != null)
        {
            RenderSettings.skybox = mainDaySkyBox;
            //Redos all the lighting so the skybox changes will be applied properly to everything.
            DynamicGI.UpdateEnvironment();
        }
        if (Input.GetKey(KeyCode.K))
        {
            RenderSettings.skybox = mainNightSkyBox;
        }
    }


}
