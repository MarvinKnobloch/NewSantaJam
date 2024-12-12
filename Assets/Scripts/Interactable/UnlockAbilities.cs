using UnityEngine;
using Santa;
using TMPro;
using Events;

public class UnlockAbilities : MonoBehaviour, IInteractable, ITrigger
{
    [SerializeField] private string unlockString;
    [TextArea][SerializeField] private string unlockText;
    private Collider objCollider;

    [SerializeField] private StringEventChannelSO messageBoxChannel;

    private void Start()
    {
        objCollider = GetComponent<Collider>();
        if(PlayerPrefs.GetInt(unlockString) == 1)
        {
            objCollider.enabled = false;
        }
    }
    public bool CanBeTriggered()
    {
        return true;
    }

    public bool CanInteractWith(MonoBehaviour user)
    {
        return true;
    }

    public string GetInteractionHint()
    {
        return "Pray";
    }

    public void Interact(MonoBehaviour user)
    {
        if (CanInteractWith(user))
        {
            Trigger(user, TriggerCommand.Toggle);
        }
    }

    public void Trigger(MonoBehaviour user, TriggerCommand cmd)
    {
        PlayerPrefs.SetInt(unlockString, 1);
        Player.Instance.RemoveInteractionObj(gameObject);

        objCollider.enabled = false;

        messageBoxChannel.OnEventRaised(unlockText);
        //GameManager.Instance.gameUI.GetComponent<PlayerUI>().ActivateMessageBox(unlockText);
    }
}
