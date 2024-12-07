using UnityEngine;

namespace Santa
{
    public class BubbleActor : MonoBehaviour
    {
        public GameObject bubble;

        private Rigidbody rig;

        void Start()
        {
            rig = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (bubble == null) return;

            if (Vector3.Distance(transform.position, bubble.transform.position) < bubble.transform.lossyScale.x / 2.0)
            {
                rig.excludeLayers = Layers.Mask(Layers.Dimension_1);
            }
            else
            {
                rig.excludeLayers = 1 << 8;
            }
        }
    }
}