using UnityEngine;
using System.Collections;
using Santa;

public class Switch : MonoBehaviour, IInteractable, ITrigger
{
    public bool activ = false;
    public bool onlyOnce = false;
    public Animator animator;

    public AudioClip gearSound;
    public AudioClip dingSound;

    [SerializeField] private GameObject[] objsToActivate;
    [SerializeField] private Switch[] connectedSwitches;

    [SerializeField] private bool toggle;
    private AudioSource audioSource;

    private void Start()
    {
        toggle = activ;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
    }

    public bool CanBeTriggered()
    {
        return !onlyOnce || !activ;
    }

    public bool CanInteractWith(MonoBehaviour user)
    {
        return CanBeTriggered();
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
        if (!CanBeTriggered()) return;
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
                audioSource.PlayOneShot(gearSound);
                animator.SetBool("ON", true);
            }
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
                audioSource.PlayOneShot(gearSound);
                animator.SetBool("ON", false);
            }
        }
    }

    public void OnDing()
    {
        audioSource.PlayOneShot(dingSound);
    }
}
