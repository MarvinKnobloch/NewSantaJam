using UnityEngine;
using Santa;
using Events;

public class Hint : MonoBehaviour, IInteractable, ITrigger
{
    [TextArea][SerializeField] private string messageText;
    [SerializeField] private StringEventChannelSO messageBoxChannel;

    private void OnEnable()
    {
        
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
        return "Read";
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
        messageBoxChannel.OnEventRaised(messageText);
    }
}
