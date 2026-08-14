using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public static MenuButtons Instance { get; private set; }

    public WeatherAndParticles weather;
    private string setScene;

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
        setScene = sceneName;
    }

    public void LoadSetScene()
    {
        if (setScene != null)
        {
            SceneManager.LoadScene(setScene);
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


}
