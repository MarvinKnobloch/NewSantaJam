
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Santa
{
    public class DialogGui : MonoBehaviour
    {
        public GameObject root;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI messageText;

        public bool stopPlayerController = true;
        public bool autoMode = false;

        public DialogEventChannelSO dialogEventChannel;

        private Dialog dialog;
        private int page = 0;

        private Controls controls;
        private bool isDialogActive = false;

        private void Awake()
        {
            controls = Keybindinputmanager.Controls;
            controls.Dialog.Disable();
            root.SetActive(false);
        }

        private void Start()
        {
            controls.Dialog.Disable();
            root.SetActive(false);
            isDialogActive = false;
        }

        private void OnEnable()
        {
            dialogEventChannel.OnEventRaised += StartDialog;
            controls.Dialog.Next.performed += OnNextMessage;
            controls.Dialog.Close.performed += OnCloseDialog;
        }

        private void OnDisable()
        {
            dialogEventChannel.OnEventRaised -= StartDialog;
            controls.Dialog.Next.performed -= OnNextMessage;
            controls.Dialog.Close.performed -= OnCloseDialog;
        }

        public void StartDialog(Dialog dialog, int page = 0)
        {
            this.dialog = dialog;
            if (dialog)
            {
                if (dialog.messages.Length > page)
                {
                    ShowMessagePage(page);
                    root.SetActive(true);
                    isDialogActive = true;
                    if (stopPlayerController)
                    {
                        Player.Instance.Controller.enabled = false;
                    }
                    if (!autoMode)
                        controls.Dialog.Enable();
                }
            }
            else
            {
                CloseDialog();
            }
        }

        public void CloseDialog()
        {
            root.SetActive(false);
            if (stopPlayerController)
            {
                Player.Instance.Controller.enabled = true;
            }
            isDialogActive = false;
            controls.Dialog.Disable();
        }

        public void NextMessage()
        {
            if (dialog.messages.Length <= page + 1)
            {
                CloseDialog();
                return;
            }
            ShowMessagePage(page + 1);
        }

        private void OnNextMessage(InputAction.CallbackContext ctx)
        {
            if (isDialogActive) NextMessage();
        }

        private void OnCloseDialog(InputAction.CallbackContext ctx)
        {
            if (isDialogActive) CloseDialog();
        }

        private void ShowMessagePage(int page)
        {
            this.page = page;
            //nameText.color = dialog.messages[page].color;
            nameText.text = dialog.messages[page].name;
            messageText.text = dialog.messages[page].text;
        }
    }
}
