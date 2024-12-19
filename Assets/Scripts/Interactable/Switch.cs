using UnityEngine;
using System.Collections;
using Santa;

public class Switch : MonoBehaviour, IInteractable, ITrigger
{
    public bool activ = false;
    public Animator animator;

    public AudioClip gearSound;
    public AudioClip dingSound;

    [SerializeField] private GameObject[] objsToActivate;
    [SerializeField] private Switch[] connectedSwitches;

    [SerializeField] private bool toggle;
    private AudioSource audio;

    private void Start()
    {
        toggle = activ;
        audio = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
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
        return "Use lever";
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
        switch (cmd)
        {
            case TriggerCommand.Toggle: 
                { 
                    toggle = !toggle;
                    foreach (Switch obj in connectedSwitches)
                    {
                        obj.toggle = toggle;
                    }
                    break;
                }
            case TriggerCommand.ForceOn: toggle = true; break;
            case TriggerCommand.ForceOff: toggle = false; break;
        }
        if (toggle)
        {
            foreach (GameObject obj in objsToActivate)
            {
                if (obj.TryGetComponent(out IActivate activate))
                {
                    activate.Activate();
                }
            }
            if (animator)
            {
                audio.PlayOneShot(gearSound);
                animator.SetTrigger("ON");
            }
            AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.lever);
        }
        else
        {
            foreach (GameObject obj in objsToActivate)
            {
                if (obj.TryGetComponent(out IActivate activate))
                {
                    activate.Deactivate();
                }
            }
            if (animator)
            {
                audio.PlayOneShot(gearSound);
                animator.SetTrigger("OFF");
            }
            AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.lever);
        }
    }

    public void OnDing()
    {
        audio.PlayOneShot(dingSound);
    }
}
