using UnityEngine;

namespace Santa
{
    public class Checkpoint : MonoBehaviour, ITrigger, IMemento
    {
        [SerializeField] private Transform playerSpawnpoint;

        private bool activated;

        void Awake()
        {
            if (Application.isPlaying)
                GameManager.Instance.AddMementoObject(this);
        }

        public void SetData(MementoData data)
        {
            activated = data.a != 0;
        }

        public MementoData GetData()
        {
            return new MementoData(activated);
        }

        public bool CanBeTriggered()
        {
            return !activated;
        }

        public void Trigger(MonoBehaviour user, TriggerCommand cmd)
        {
            if (CanBeTriggered() && user == Player.Instance)
            {
                activated = true;
                GameManager.Instance.CreateCheckpoint(gameObject.scene, playerSpawnpoint);
                Debug.Log("Checkpoint gesetzt");
            }
        }
    }
}
