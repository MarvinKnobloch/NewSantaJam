using UnityEngine;

public class BubbleActor : MonoBehaviour
{
    public GameObject bubble;

    private Rigidbody rig;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bubble == null) return;

        if (Vector3.Distance(transform.position, bubble.transform.position) < bubble.transform.lossyScale.x / 2.0)
        {
            rig.excludeLayers = 1 << 7;
        }
        else
        {
            rig.excludeLayers = 1 << 8;
        }
    }
}
