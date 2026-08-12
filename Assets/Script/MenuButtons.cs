using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{

    WeatherAndParticles weather;
    private void Awake()
    {

    }
    void Start()
    {
        weather = GameObject.Find("WeatherSystem_Runtime").GetComponent<WeatherAndParticles>();
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
