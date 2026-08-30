using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public static MenuButtons Instance { get; private set; }

    public WeatherAndParticles weather;

    void Awake()
    {

        weather = WeatherAndParticles.Instance;
        Instance = this;
    }
    public void LoadWeather(int weatherId)
    {
        weather.ChangeWeather(weatherId);
    }

    public void SetScene(string sceneName)
    {
        weather.setScene = sceneName;
    }

    public void LoadSetScene(string car)
    {
        weather.setCar = car;
        if (weather.setScene != null)
        {
            SceneManager.LoadScene(weather.setScene);
        }
        else
        {
            Debug.Log("Scene Not Set");
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadBackgroundWeather(int weatherID)
    {
        weather.ChangeWeather(weatherID);
    }

    public void ChangeKeybind(int button)
    {
    
        SettingMenu.Instance.DetectingKeys(button);

    }

}
