using UnityEngine;
using Santa;

public class Platform : MonoBehaviour
{
    private PlatformObject platformObject;
    private Vector3 endPosi;
    private Vector3 startPosi;
    [SerializeField] private float travelTime;
    private float timer;
    [SerializeField] private bool moveOnEnter;

    private BoxCollider boxCollider;

    public State state;
    public enum State
    {
        moveToEnd,
        moveToStart,
        dontMove,
    }
    private void Awake()
    {
        platformObject = transform.GetChild(0).GetComponent<PlatformObject>();
        startPosi = transform.position;
        endPosi = transform.GetChild(1).transform.position;
        timer = 0;

        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider)
        {
            var collider = platformObject.GetComponent<BoxCollider>();
            boxCollider.size = Vector3.Scale(collider.size, platformObject.transform.lossyScale);
            boxCollider.center = collider.center + Vector3.up * 0.2f;
        }

        if (moveOnEnter) state = State.dontMove;

    }

}
