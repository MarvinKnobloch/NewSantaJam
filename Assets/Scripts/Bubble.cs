using UnityEngine;

namespace Santa
{
    [ExecuteInEditMode]
    public class Bubble : MonoBehaviour
    {
        [Range(0, 7)]
        public int BubbleNumber = 1;

        [Space]
        public float speed = 1.0f;

        private Transform t;

        void Start()
        {
            t = transform;
        }

        // Vorsichtig sein. Skript wird im Editor ausgeführt!
        void Update()
        {
            if (Application.isPlaying)
            {
                Vector3 pos = t.position;
                pos.z += speed * Time.deltaTime;
                t.position = pos;
            }
            if (!BubbleController.Instance) return;
            BubbleController.Instance.SetBubble(BubbleNumber, t.position, t.localScale.x / 2);
        }
    }
}