using UnityEngine;

namespace Santa
{
    public class BubbleActor : MonoBehaviour
    {
        public bool inverted = false;

        private Collider _collider;

        void Start()
        {
            _collider = GetComponent<Collider>();
        }

        void Update()
        {
            if (!BubbleController.Instance) return;

            var myDimensionLayer = inverted ? Layers.Dimension_2 : Layers.Dimension_1;
            var otherDimensionLayer = inverted ? Layers.Dimension_1 : Layers.Dimension_2;

            _collider.excludeLayers = Layers.Mask(otherDimensionLayer);
            for (int i = 0; i < BubbleController.COUNT; i++)
            {
                if (Vector3.Distance(transform.position, BubbleController.Instance.positions[i]) < BubbleController.Instance.radien[i])
                {
                    _collider.excludeLayers = Layers.Mask(myDimensionLayer);
                    return;
                }
            }
        }
    }
}