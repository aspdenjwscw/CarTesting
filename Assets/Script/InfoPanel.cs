using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button closeButton;
    [SerializeField] [TextArea(2, 4)] private string message;

    void Start()
    {
        infoText.text = message;
        closeButton.onClick.AddListener(ClosePanel);
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}