using UnityEngine;

namespace Santa
{
    public class KeybindManager : MonoBehaviour
    {
        public static Controls inputActions;

        private void Awake()
        {
            if (inputActions == null)
            {
                inputActions = Keybindinputmanager.inputActions; //new Controls();
            }
        }
    }
}