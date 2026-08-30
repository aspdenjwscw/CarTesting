using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaded : MonoBehaviour
{
    private WeatherAndParticles weather;



    private void Start()
    { 
        weather = MenuButtons.Instance.weather;
        Debug.Log("Something's working");
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "FinalMountainScene")
        {
            Debug.Log("SceneFound");
            Debug.Log(weather);
            if (weather != null)
            {
                weather.SetCurrentWeather();
            }
        }
        else if (currentScene.name == "MapSelectionScene" || currentScene.name == "CarSelectScene" || currentScene.name == "FinalOtherside" || currentScene.name == "TutorialMap")
        {
            if (weather != null)
            {
                weather.SetCurrentWeather();
            }
        }
        //This will fire every time a scene loads, if I attach it to scene loader with the Menu buttons script
    }
}
