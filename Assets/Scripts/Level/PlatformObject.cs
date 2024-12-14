using UnityEngine;

namespace Santa
{
    public class PlatformObject : MonoBehaviour, IPlatform
    {
        public Transform endpoint;
        private Vector3 startPosi;
        [SerializeField] private float travelTime;
        private float timer;
        [SerializeField] private bool moveOnEnter;

        public State state;
        public enum State
        {
            moveToEnd,
            moveToStart,
            dontMove,
        }

        public Vector3 velocity { get { return _velocity; } set { _velocity = value; } }

        private Vector3 _velocity;
        private Rigidbody rig;

        private void Awake()
        {
            rig = GetComponent<Rigidbody>();
            startPosi = transform.position;
            timer = 0;

            if (moveOnEnter) state = State.dontMove;
        }
        private void FixedUpdate()
        {
            switch (state)
            {
                case State.moveToEnd:
                    Move(startPosi, endpoint.position, State.moveToStart);
                    break;
                case State.moveToStart:
                    Move(endpoint.position, startPosi, State.moveToEnd);
                    break;
                case State.dontMove:
                    break;
            }
        }
        private void Move(Vector3 start, Vector3 end, State nextState)
        {
            if (timer < travelTime)
            {
                float time = timer / travelTime;
                Vector3 newPos = Vector3.Lerp(start, end, time);
                velocity = newPos - rig.position;
                rig.MovePosition(newPos);
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                if (moveOnEnter)
                {
                    if (nextState == State.moveToEnd) state = State.dontMove;
                    else state = nextState;
                }
                else state = nextState;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out PlayerController pc))
            {
                pc.externalForce.x = _velocity.x;
                pc.externalForce.z = _velocity.z;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            OnCollisionEnter(collision);
        }
    }
}
