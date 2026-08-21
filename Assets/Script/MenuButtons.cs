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

    public void SetKeybinds()
    {
        //Calls Function in SettingsMenu.cs to change the keybinds
    }

    public void ChangeKeybind()
    {
        //Maybe a while(true){ loop until break this would be for picking up a new keybind being presed.
        //Also remember to make it so that when the escape key is pressed it cancels everything
        //If(escape not pressed && another allowed key is pressed){
        //Calls a function with data to send button pressed, and changed key
        //}

    }

}
