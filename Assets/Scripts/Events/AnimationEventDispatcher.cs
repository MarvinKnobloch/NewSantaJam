using UnityEngine;

namespace Events
{
    // Workaround Klasse, weil unity doof ist
    public class AnimationEventDispatcher : MonoBehaviour
    {
        public MonoBehaviour target;

        void OnDing()
        {
            target.Invoke("OnDing", 0);
        }
    }
}
