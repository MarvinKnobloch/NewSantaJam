using UnityEngine;
using TMPro;
using Events;

public class PlayerUI : MonoBehaviour
{
    public GameObject messageBox;
    private TextMeshProUGUI messageText;

    [SerializeField] private StringEventChannelSO messageBoxChannel;

    private void Awake()
    {
        messageText = messageBox.GetComponentInChildren<TextMeshProUGUI>();
        messageBoxChannel.OnEventRaised += ActivateMessageBox;
    }
    private void OnDestroy()
    {
        messageBoxChannel.OnEventRaised -= ActivateMessageBox;
    }

    public void ActivateMessageBox(string messageBoxText)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
        messageBox.SetActive(true);
        messageText.text = messageBoxText;
    }
    public void CloseMessageBox()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;
        messageBox.SetActive(false);
    }
}
