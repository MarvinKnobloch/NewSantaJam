using UnityEngine;

namespace Santa
{
    public class CameraSpringArm : MonoBehaviour
    {
        public float targetDistance = 5f;
        public float wallOffset = 0.5f;
        public float springForce = 100f;
        public float hysterese = 0.25f;

        private float distance;

        void LateUpdate()
        {
            if (Physics.Raycast(transform.parent.position, -transform.forward, out RaycastHit hit, targetDistance))
            {
                if (hit.distance + hysterese < distance)
                {
                    distance = Mathf.SmoothStep(distance, hit.distance, springForce * Time.deltaTime);
                }
                else if (hit.distance - hysterese > distance)
                {
                    distance = Mathf.SmoothStep(distance, hit.distance, springForce * Time.deltaTime);
                }
            }
            else if (distance < targetDistance)
            {
                distance = Mathf.SmoothStep(distance, targetDistance, springForce * Time.deltaTime);
            }
            transform.localPosition = new Vector3(0, 0, -distance + wallOffset);
        }
    }
}