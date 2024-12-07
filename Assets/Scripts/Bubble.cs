using UnityEngine;

[ExecuteInEditMode]
public class Bubble : MonoBehaviour
{
    public Renderer outerSpace;
    public Renderer innerSpace;

    [Space]
    public float speed = 1.0f;

    private Transform t;

    void Start()
    {
        t = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (outerSpace == null || innerSpace == null) return;

        if (Application.isPlaying)
        {
            Vector3 pos = t.position;
            pos.z += speed * Time.deltaTime;
            t.position = pos;
        }

        outerSpace.sharedMaterial.SetVector("_Cut_Position", t.position);
        innerSpace.sharedMaterial.SetVector("_Cut_Position", t.position);
        outerSpace.sharedMaterial.SetFloat("_Cut_Radius", t.localScale.x / 2);
        innerSpace.sharedMaterial.SetFloat("_Cut_Radius", t.localScale.x / 2);
    }
}
