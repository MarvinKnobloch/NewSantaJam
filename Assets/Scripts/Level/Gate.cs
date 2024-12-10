using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour, IActivate
{
    private Vector3 startPosi;
    private Vector3 endPosi;
    [SerializeField] private Transform endTransform;

    private int currentGoals;
    public int requiredGoals;
    [SerializeField] private float doorOpenDuration = 1.0f;
    private float timer;


    private bool secretUnlocked;

    private void Awake()
    {
        startPosi = transform.position;
        endPosi = endTransform.position;
    }

    public void Activate()
    {
        currentGoals++;
        if (currentGoals >= requiredGoals)
        {
            if (timer != 0) timer = doorOpenDuration - timer;

            StopAllCoroutines();
            StartCoroutine(MoveGate(startPosi, endPosi));

            if (secretUnlocked == false)
            {
                secretUnlocked = true;
            }
        }
    }

    public void Deactivate()
    {
        currentGoals--;
        if (currentGoals != requiredGoals)
        {
            if (timer != 0) timer = doorOpenDuration - timer;

            StopAllCoroutines();
            StartCoroutine(MoveGate(endPosi, startPosi));
        }
    }
    private IEnumerator MoveGate(Vector3 start, Vector3 end)
    {
        while (Vector3.Distance(transform.position, end) > 0.1f)
        {
            float time = timer / doorOpenDuration;
            transform.position = Vector3.Lerp(start, end, time);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        StopAllCoroutines();
    }
}
