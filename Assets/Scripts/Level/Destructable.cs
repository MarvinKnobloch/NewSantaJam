using System.Collections;
using UnityEngine;

namespace Santa
{
    public class Destructable : MonoBehaviour, ITrigger, IPlatform
    {
        public GameObject destructPrefab;
        public float timeToBreak = 1f;
        public float force = 10f;

        private AudioSource audio;
        private bool canBeDestructed = true;

        public Vector3 velocity => Vector3.zero;

        public bool CanBeTriggered()
        {
            return canBeDestructed;
        }

        public void Trigger(MonoBehaviour user, TriggerCommand cmd)
        {
            if (canBeDestructed)
            {
                canBeDestructed = false;
                StartCoroutine(BreakSlowly());
            }
        }

        public void OnStepOn()
        {
            if (CanBeTriggered())
            {
                Trigger(null, TriggerCommand.Toggle);
            }
        }

        IEnumerator BreakSlowly()
        {
            if (audio) audio.Play();

            yield return new WaitForSeconds(timeToBreak);

            var obj = Instantiate(destructPrefab, transform.position, transform.rotation);
            obj.transform.localScale = transform.localScale;
            foreach (Rigidbody rig in obj.transform.GetComponentsInChildren<Rigidbody>())
            {
                rig.AddForce((Vector3.down + Random.insideUnitSphere) * force, ForceMode.Force);
            }
            Destroy(gameObject);
        }
    }
}
