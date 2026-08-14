using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaded : MonoBehaviour
{
    private MenuButtons weatherGrabber;
    private WeatherAndParticles weather;



    private void Start()
    { 
        weather = MenuButtons.Instance.weather;
        Debug.Log("Something's working");
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "NewSceneFinlay")
        {
            Debug.Log("SceneFound");
            Debug.Log(weather);
            if (weather != null)
            {
                weather.SetCurrentWeather();
            }
        }
        else if (currentScene.name == "MapSelectionScene")
        {
            weather.SetCurrentWeather();
        }
        //This will fire every time a scene loads, if I attach it to scene loader with the Menu buttons script
    }
}
