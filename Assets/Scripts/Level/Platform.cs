using UnityEngine;

public class Platform : MonoBehaviour
{
    private Vector3 endPosi;
    private Vector3 startPosi;
    [SerializeField] private float travelTime;
    private float timer;

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
}
