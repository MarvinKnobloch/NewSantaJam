using UnityEngine;

public class ResetZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.die();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}
