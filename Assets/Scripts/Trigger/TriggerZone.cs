using UnityEngine;

namespace Santa
{
    /// <summary>
    /// Aktiviert ein GameObjekt. Spielt optional ein Sound.
    /// </summary>
    public class TriggerZone : MonoBehaviour, ITrigger
    {
        public GameObject target;
        //public bool onlyOnce = true;
        public bool canUsedAsGhost = true;
        public bool deactivateObject = false;

        //private bool activated;

        public bool CanBeTriggered()
        {
            return true;
        }

        public void Trigger(MonoBehaviour user, TriggerCommand cmd)
        {
            if (CanBeTriggered())
            {
                //activated = true;
                if (target) target.SetActive(!deactivateObject);
                if (TryGetComponent(out AudioSource audio)) audio.Play();
            }
        }
    }
}
