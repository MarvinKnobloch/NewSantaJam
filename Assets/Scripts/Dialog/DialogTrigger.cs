using UnityEngine;
using Events;

namespace Santa
{
    public class DialogTrigger : MonoBehaviour, IInteractable
    {
        public bool active = true;
        public Dialog dialog;

        public DialogEventChannelSO dialogEventChannel;

        public bool CanInteractWith(MonoBehaviour user)
        {
            return active;
        }

        public string GetInteractionHint()
        {
            return "Speak";
        }

        public void Interact(MonoBehaviour user)
        {
            if (dialog != null)
            {
                dialogEventChannel.RaiseEvent(dialog, 0);
            }
        }
    }
}