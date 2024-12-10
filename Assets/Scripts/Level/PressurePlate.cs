using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private Vector3 startPosi;
    private Vector3 endPosi;

    [SerializeField] private GameObject[] objsToActivate;

    private List<GameObject> objsOnPlate = new List<GameObject>();
    [SerializeField] private LayerMask layer;

    private void Start()
    {
        startPosi = transform.position;
        endPosi = transform.position + Vector3.down * 0.3f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layer) != 0)
        {
            if (objsOnPlate.Contains(other.gameObject) == false)
            {
                objsOnPlate.Add(other.gameObject);
                if (objsOnPlate.Count == 1)
                {
                    foreach (GameObject obj in objsToActivate)
                    {
                        obj.GetComponent<IActivate>().Activate();
                    }
                    transform.position = endPosi;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (objsOnPlate.Contains(other.gameObject))
        {
            objsOnPlate.Remove(other.gameObject);
            if(objsOnPlate.Count == 0)
            {
                foreach (GameObject obj in objsToActivate)
                {
                    obj.GetComponent<IActivate>().Deactivate();
                }
                transform.position = startPosi;
            }
        }
    }
}
