using UnityEngine;
using Events;

namespace Santa
{
    public class DialogTrigger : MonoBehaviour, ITrigger
    {
        public bool active = true;
        public Dialog dialog;

        public DialogEventChannelSO dialogEventChannel;

        public bool CanBeTriggered()
        {
            return active;
        }

        public void Trigger(MonoBehaviour user, TriggerCommand cmd)
        {
            if (dialog != null)
            {
                dialogEventChannel.RaiseEvent(dialog);
            }
        }
    }
}