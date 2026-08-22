using TMPro;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;


[System.Serializable]
public class Keybinds
{
    public string forwardsKey = "w";
    public string backwardsKey = "s";
    public string leftKey = "a";
    public string rightKey = "d";
    public string brakeKey = "space";
    public string unstuckKey = "r";
    public string resetKey = "k";
}


public class SettingMenu : MonoBehaviour
{
    public MenuButtons menuButtons;
    public GameObject buttonsParent;
    public bool isListening = false;
    public int currentButtonInt;
    public string currentButton;
    private IDisposable subscription;
    public static SettingMenu Instance { get; private set; }
    public InputControl forwardsControl, backwardsControl, leftControl, rightControl, brakeControl, unstuckControl, resetControl;

    private string filePath;
    public Keybinds keyBinds { get; private set; }

    public Dictionary<int, String> numToKeybind = new Dictionary<int, String>();

    public List<TextMeshProUGUI> buttonText = new List<TextMeshProUGUI>();



    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "keybinds.json");

        LoadKeybinds();
        RedoValues();


        for (int i = 0; i < buttonsParent.transform.childCount; i++)
        {
            Transform child = buttonsParent.transform.GetChild(i);

            if(child.GetChild(0).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI component))
            {
                buttonText.Add(component);
            }
        }
        for (int i = 0; i < buttonText.Count; i++)
        {
            buttonText[i].text = numToKeybind[i].ToUpper();

        }
    }

    public void RedoValues()
    {
        forwardsControl = Keyboard.current[keyBinds.forwardsKey.ToLower()];
        backwardsControl = Keyboard.current[keyBinds.backwardsKey.ToLower()];
        leftControl = Keyboard.current[keyBinds.leftKey.ToLower()];
        rightControl = Keyboard.current[keyBinds.rightKey.ToLower()];
        brakeControl = Keyboard.current[keyBinds.brakeKey.ToLower()];
        unstuckControl = Keyboard.current[keyBinds.unstuckKey.ToLower()];
        resetControl = Keyboard.current[keyBinds.resetKey.ToLower()];

        numToKeybind.Clear();
        numToKeybind.Add(0, keyBinds.forwardsKey);
        numToKeybind.Add(1, keyBinds.backwardsKey);
        numToKeybind.Add(2, keyBinds.leftKey);
        numToKeybind.Add(3, keyBinds.rightKey);
        numToKeybind.Add(4, keyBinds.brakeKey);
        numToKeybind.Add(5, keyBinds.unstuckKey);
        numToKeybind.Add(6, keyBinds.resetKey);

        //Redo the Names when they're changed.
    }


    public void LoadKeybinds()
    {
        if (!File.Exists(filePath))
        {
            //Creates New Keybinds if the keybinds json doesn't exist.
            keyBinds = new Keybinds();
            SaveKeybinds();
            return;
        }

        string json = File.ReadAllText(filePath);

        if (!string.IsNullOrEmpty(json))
        {
           //Makes sure that Keybinds actually exist, then load them
            keyBinds = JsonUtility.FromJson<Keybinds>(json);
        }
        else
        {
            //Creates New Keybinds if empty, or first time using it.
            keyBinds = new Keybinds();
            SaveKeybinds();
        }
    }

    public void SaveKeybinds()
    {
        string json = JsonUtility.ToJson(keyBinds, true);

        File.WriteAllText(filePath, json);

        //Writes the current keyBinds into memory, and not just held on RAM
    }

    

    public void DetectingKeys(int button)
    {
        Debug.Log("Here");
        currentButtonInt = button;
        currentButton = numToKeybind[button];
        buttonText[button].text = "";
        //Button textMeshPro make empty
        if (!isListening)
        {
            isListening = true;
            subscription = InputSystem.onAnyButtonPress.Call(SelectKey);
        }
    }

    void SelectKey(InputControl key)
    {
        Debug.Log("here");
        if(key == Keyboard.current.escapeKey)
        {
            buttonText[currentButtonInt].text = currentButton.ToUpper();
            subscription?.Dispose();
            isListening = false;
        }
        else if (key.device is Keyboard)
        {
            switch (currentButtonInt)
            {
                case 0: keyBinds.forwardsKey = key.name.ToLower(); break;
                case 1: keyBinds.backwardsKey = key.name.ToLower(); break;
                case 2: keyBinds.leftKey = key.name.ToLower(); break;
                case 3: keyBinds.rightKey = key.name.ToLower(); break;
                case 4: keyBinds.brakeKey = key.name.ToLower(); break;
                case 5: keyBinds.unstuckKey = key.name.ToLower(); break;
                case 6: keyBinds.resetKey = key.name.ToLower(); break;
            }
            numToKeybind[currentButtonInt] = key.name.ToLower();
            buttonText[currentButtonInt].text = key.name.ToUpper();
            subscription?.Dispose();
            isListening = false;
            RedoValues();
            SaveKeybinds();
        }
    }

    //Make a function to send the cs file to MenuButtons, or just manually make a public variable in MenuButtons
    


}
