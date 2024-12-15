using UnityEngine;

namespace Santa
{
    public class BubbleActor : MonoBehaviour
    {
        public bool inverted = false;

        private Rigidbody rig;

        void Start()
        {
            rig = GetComponent<Rigidbody>();
        }

        void Update()
        {
            var myDimensionLayer = inverted ? Layers.Dimension_2 : Layers.Dimension_1;
            var otherDimensionLayer = inverted ? Layers.Dimension_1 : Layers.Dimension_2;

            rig.excludeLayers = Layers.Mask(otherDimensionLayer);
            for (int i = 0; i < BubbleController.COUNT; i++)
            {
                if (Vector3.Distance(transform.position, BubbleController.Instance.positions[i]) < BubbleController.Instance.radien[i])
                {
                    rig.excludeLayers = Layers.Mask(myDimensionLayer);
                    return;
                }
            }
        }
    }
}