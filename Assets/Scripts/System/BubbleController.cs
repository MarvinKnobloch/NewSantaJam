
using UnityEngine;

namespace Santa
{
    [ExecuteInEditMode]
    public class BubbleController: MonoBehaviour
    {
        public const int COUNT = 8;

        public static BubbleController Instance;

        public Material cutOutMaterial;
        public Material CutInMaterial;

        public readonly Vector4[] positions = new Vector4[COUNT];
        public readonly float[] radien = new float[COUNT];

        private readonly int[] pos_props = new int[COUNT];
        private readonly int[] rad_props = new int[COUNT];

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            for (int i = 0; i < COUNT; i++)
            {
                pos_props[i] = Shader.PropertyToID("_Point" + (i+1));
                rad_props[i] = Shader.PropertyToID("_Radius" + (i+1));
            }
        }

        public void SetBubble(int index, Vector3 pos, float radius)
        {
            positions[index] = pos;
            radien[index] = radius;
            if (!Application.isPlaying)
            {
                LateUpdate();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < COUNT; i++)
            {
                cutOutMaterial.SetVector(pos_props[i], positions[i]);
                cutOutMaterial.SetFloat(rad_props[i], radien[i]);

                CutInMaterial.SetVector(pos_props[i], positions[i]);
                CutInMaterial.SetFloat(rad_props[i], radien[i]);
            }
        }
    }
}
