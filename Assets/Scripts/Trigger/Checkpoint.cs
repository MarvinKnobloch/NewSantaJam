using UnityEngine;

namespace Santa
{
    public class Checkpoint : MonoBehaviour, ITrigger, IMemento
    {
        public static Checkpoint currentCheckPoint;
        [SerializeField] private Transform playerSpawnpoint;
        [SerializeField] private GameObject checkpointEffect;
        private BoxCollider boxCollider;

        private bool activated;

        void Awake()
        {
            if (Application.isPlaying)
                GameManager.Instance.AddMementoObject(this);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, boxCollider.size.y * 0.35f, 0);
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
                //activated = true;
                GameManager.Instance.CreateCheckpoint(gameObject.scene, playerSpawnpoint);
                Debug.Log("Checkpoint gesetzt");

                if (currentCheckPoint != null)
                {
                    if (currentCheckPoint == this) return;
                    currentCheckPoint.DeactivateVisual();
                }
                currentCheckPoint = this;
                currentCheckPoint.ActivateVisual();

                AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.secret);
            }
        }
        public void ActivateVisual()
        {
            checkpointEffect.SetActive(true);
        }
        public void DeactivateVisual()
        {
            checkpointEffect.SetActive(false);
        }
    }
}
