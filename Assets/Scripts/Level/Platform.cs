using UnityEngine;
using Santa;

public class Platform : MonoBehaviour
{
    private Vector3 endPosi;
    private Vector3 startPosi;
    [SerializeField] private float travelTime;
    private float timer;
    private BoxCollider boxCollider;

    [System.NonSerialized] public Vector3 velocity;
    private Vector3 oldPosi;

    public State state;
    public enum State
    {
        moveToEnd,
        moveToStart,
    }
    private void Awake()
    {
        startPosi = transform.position;
        endPosi = transform.GetChild(1).transform.position;
        timer = 0;

        boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = transform.GetChild(0).localScale;

    }
    private void FixedUpdate()
    {
        switch (state)
        {
            case State.moveToEnd:
                Move(startPosi, endPosi, State.moveToStart);
                break;
            case State.moveToStart:
                Move(endPosi, startPosi, State.moveToEnd);
                break;
        }
        velocity = (transform.position - oldPosi) / Time.fixedDeltaTime;
        oldPosi = transform.position;
    }
    private void Move(Vector3 start, Vector3 end, State nextState)
    {
        if (timer < travelTime)
        {
            float time = timer / travelTime;
            transform.position = Vector3.Lerp(start, end, time);
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            state = nextState;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out PlayerController player))
            {
                player.movingPlatform = this;
                player.isOnPlatform = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out PlayerController player))
            {
                if (player.movingPlatform = this)
                {
                    player.isOnPlatform = false;
                    player.movingPlatform = null;
                    if (player.state == PlayerController.States.GroundState)
                    {
                        player.velocity.y = 0;
                    }
                }
            }
        }
    }
}
