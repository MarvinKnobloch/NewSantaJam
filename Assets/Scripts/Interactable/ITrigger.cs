using UnityEngine;

namespace Santa
{
    public interface ITrigger
    {
        public bool CanBeTriggered();

        public void Trigger(MonoBehaviour user, TriggerCommand cmd);
    }

    public enum TriggerCommand
    {
        Toggle,
        ForceOn,
        ForceOff,
    }
}
