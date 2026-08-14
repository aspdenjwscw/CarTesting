using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaded : MonoBehaviour
{
    [SerializeField] private MenuButtons weatherGrabber;
    private WeatherAndParticles weather;



    private void Start()
    {
        weather = MenuButtons.Instance.weather;
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "NewSceneFinlay")
        {

        }
        else if (currentScene.name == "MapSelectionScene")
        {

        }
        //This will fire every time a scene loads, if I attach it to scene loader with the Menu buttons script
    }
}
