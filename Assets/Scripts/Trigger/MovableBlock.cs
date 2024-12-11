using UnityEngine;

namespace Santa
{
    // Der Block kann aktuell nicht fallen, da er nicht auf der Y-Achse bewegt wird.
    [RequireComponent(typeof(BoxCollider))]
    public class MovableBlock : MonoBehaviour, ITrigger
    {
        public float moveSpeed = 4.0f;
        public float friction = 0.5f;

        private Vector3 movement;

        void FixedUpdate()
        {
            if (movement != Vector3.zero)
            {
                var pos = transform.position;
                var newPos = pos + movement * Time.fixedDeltaTime;

                var collider = GetComponent<BoxCollider>();
                if (Physics.BoxCast(pos + collider.center, collider.bounds.extents * 0.99f, movement, Quaternion.identity, Vector3.Magnitude(movement * Time.fixedDeltaTime)))
                {
                    movement = Vector3.zero;
                    return;
                }
                transform.position = newPos;

                if (friction != 0)
                {
                    movement = movement * (1 - friction);
                }
            }
        }

        public bool CanBeTriggered()
        {
            return true;
        }

        public void Trigger(MonoBehaviour user, TriggerCommand cmd)
        {
            Debug.Log("MovableBlock Triggered " + user.name);
            if (user.gameObject.layer == Layers.Spieler)
            {
                var pos = transform.position;
                var userPos = user.transform.position;

                var dx = userPos.x - pos.x;
                var dz = userPos.z - pos.z;
                if (Mathf.Abs(dx) > Mathf.Abs(dz))
                {
                    movement = new Vector3(-Mathf.Sign(dx) * moveSpeed, 0, 0);
                }
                else
                {
                    movement = new Vector3(0, 0, -Mathf.Sign(dz) * moveSpeed);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            movement = Vector3.zero;
        }
    }
}