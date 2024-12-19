using UnityEngine;
using Santa;

public class Collectable : MonoBehaviour, ITrigger
{
    [SerializeField] private GameObject[] objsToActivate;

    private void Start()
    {
        foreach (GameObject obj in objsToActivate)
        {
            if (obj.TryGetComponent(out IActivate activate))
            {
                activate.SetRequirement();
            }
        }
    }
    public bool CanBeTriggered()
    {
        return true;
    }

    public void Trigger(MonoBehaviour user, TriggerCommand cmd)
    {
        foreach (GameObject obj in objsToActivate)
        {
            if(obj.TryGetComponent(out IActivate activate))
            {
                activate.Activate();
            }
        }
        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.collect);
        gameObject.SetActive(false);
    }
}
