using UnityEngine;
using System.Collections;
using TMPro;

public class Gate : MonoBehaviour, IActivate
{
    private Vector3 startPosi;
    private Vector3 endPosi;
    [SerializeField] private Transform endTransform;

    private int currentGoals;
    public int requiredGoals;
    [SerializeField] private float doorOpenDuration = 1.0f;
    private float timer;
    private float travelTime;

    [SerializeField] private bool fastBack;
    [SerializeField] private float backDuration;

    [SerializeField] private TextMeshProUGUI requirementText;

    private State state;
    public enum State
    {
        dontMove,
        moveToEnd,
        moveToStart,
    }
    private void Awake()
    {
        startPosi = transform.position;
        endPosi = endTransform.position;
    }
    private void FixedUpdate()
    {
        switch (state)
        {
            case State.dontMove:
                break;
            case State.moveToEnd:
                Move(startPosi, endPosi);
                break;
            case State.moveToStart:
                Move(endPosi, startPosi);
                break;
        }
    }

    public void Activate()
    {
        currentGoals++;
        TextUpdate();

        if (currentGoals >= requiredGoals)
        {
            if (timer != 0)
            {
                if (fastBack) timer = backDuration - timer;
                else timer = doorOpenDuration - timer;

            }

            travelTime = doorOpenDuration;
            if (fastBack)
            {
                timer *= doorOpenDuration / backDuration;
            }

            state = State.moveToEnd;

            if (requirementText != null) requirementText.gameObject.SetActive(false);
            //StopAllCoroutines();
            //StartCoroutine(MoveGate(startPosi, endPosi, doorOpenDuration));
        }
    }

    public void Deactivate()
    {
        currentGoals--;
        TextUpdate();

        if (currentGoals != requiredGoals)
        {
            if (timer != 0) timer = doorOpenDuration - timer;

            travelTime = doorOpenDuration;
            if (fastBack)
            {
                travelTime = backDuration;
                timer *= backDuration / doorOpenDuration; 
            }

            state = State.moveToStart;

            if (requirementText != null) requirementText.gameObject.SetActive(true);
            //StopAllCoroutines();
            //StartCoroutine(MoveGate(endPosi, startPosi, duration));
        }
    }
    private void Move(Vector3 start, Vector3 end)
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
            state = State.dontMove;
        }
    }
    //private IEnumerator MoveGate(Vector3 start, Vector3 end, float duration)
    //{
    //    while (Vector3.Distance(transform.position, end) > 0.1f)
    //    {
    //        float time = timer / duration;
    //        transform.position = Vector3.Lerp(start, end, time);
    //        timer += Time.deltaTime;
    //        yield return new WaitForFixedUpdate();
    //    }
    //    timer = 0;
    //    StopAllCoroutines();
    //}

    public void SetRequirement()
    {
        requiredGoals++;
        TextUpdate();
    }
    private void TextUpdate()
    {
        if (requirementText != null) requirementText.text = (requiredGoals - currentGoals).ToString();
    }
}
