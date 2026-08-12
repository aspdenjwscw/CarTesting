using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public static MenuButtons Instance { get; private set; }
    public WeatherAndParticles weather;
    void Awake()
    {
        Instance = this;
    }
    public void WeatherCreated(GameObject weatherBase)
    {
        weather = weatherBase.GetComponent<WeatherAndParticles>();
    }
    public void LoadWeather(int weatherId)
    {
        weather.ChangeWeather(weatherId);
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
