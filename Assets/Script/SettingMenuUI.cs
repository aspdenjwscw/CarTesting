using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingMenuUI : MonoBehaviour
{
    public List<TextMeshProUGUI> buttonText = new List<TextMeshProUGUI>();
    public static SettingMenuUI Instance { get; private set; }
    public GameObject buttonsParent;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < buttonsParent.transform.childCount; i++)
        {
            Transform child = buttonsParent.transform.GetChild(i);

            if (child.GetChild(0).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI component))
            {
                buttonText.Add(component);
            }
        }
        for (int i = 0; i < buttonText.Count; i++)
        {
            buttonText[i].text = SettingMenu.Instance.numToKeybind[i].ToUpper();

        }
    }
}
