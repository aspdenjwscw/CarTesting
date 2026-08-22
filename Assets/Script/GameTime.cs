using UnityEngine;
using TMPro;

public class GameTime : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public float gameTime;

    void FixedUpdate()
    {
        gameTime += Time.deltaTime;
        UpdateTimer(gameTime);
    }

    public void UpdateTimer(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60); //Gets the result of time/60 but rounds down to the nearest number
        float seconds = Mathf.FloorToInt(time % 60); //Gets the remander of multiples of 60. This means that every 60s gets removed to become 1 minute
        float miliseconds = (time % 1) * 100; //This gets numbers less than 1 then times by 100 to get miliseconds
        timer.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, miliseconds);
    }


}
