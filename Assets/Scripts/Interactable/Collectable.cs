using UnityEngine;
using Santa;

public class Collectable : MonoBehaviour, ITrigger
{
    [SerializeField] private GameObject[] objsToActivate;
    [SerializeField] private int ID;

    private void Start()
    {
        if(ID != 0)
        {
            if(PlayerPrefs.GetInt("Collectable" + ID) == 1)
            {
                gameObject.SetActive(false);
                return;
            }
        }

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
        if (ID != 0) PlayerPrefs.SetInt("Collectable" + ID, 1);

        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.collect);
        gameObject.SetActive(false);
    }
}
