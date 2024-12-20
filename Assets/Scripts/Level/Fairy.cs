using UnityEngine;

public class Fairy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player.Instance.Controller.ResetAbilities();
            gameObject.SetActive(false);
        }
    }
}
