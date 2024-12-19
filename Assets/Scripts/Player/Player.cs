using Events;
using Santa;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private float useDist = 2f;
    [SerializeField] private LayerMask useMask;
    [SerializeField] private float deathFallHeight;
    [SerializeField] private float deathHeight = -24f;
    private bool isDead;

    private List<IInteractable> interactionObjs = new List<IInteractable>();
    private IInteractable oldClosetstInteractable;

    [Header("Event")]
    [SerializeField] private StringEventChannelSO interactableChannel;

    #region Properties
    public float Speed => controller.Speed;
    public Vector3 Center => transform.position + Vector3.up * 1f;

    public PlayerController Controller => controller;
    #endregion

    #region Privates
    private PlayerController controller;
    private new AudioSource audio;

    private RaycastHit castHit;

    private IInteractable foundInteractable;
    #endregion

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        controller = GetComponent<PlayerController>();
        audio = GetComponent<AudioSource>();
        controller.onUse += OnUse;
    }

    void Update()
    {
        if (transform.position.y < deathHeight)
        {
            die();
            return;
        }
        if (Time.frameCount % 4 == 1)
        {
            ScanInteractables();
        }
    }

    private void ScanInteractables()
    {
        if (interactionObjs.Count != 0)
        {
            getclosestinteraction();
            if (foundInteractable != null)
            {
                GameManager.Instance.interactionText.gameObject.SetActive(true);
                GameManager.Instance.interactionText.text = foundInteractable.GetInteractionHint() + " (<color=green>" + controller.controls.Player.Use.GetBindingDisplayString() + "</color>)";
            }
        }
        else
        {
            GameManager.Instance.interactionText.gameObject.SetActive(false);
            if (oldClosetstInteractable != null)
            {
                oldClosetstInteractable = null;
            }
            foundInteractable = null;
        }
    }
    private void getclosestinteraction()
    {
        float closestdistance = 10f;

        foreach (IInteractable obj in interactionObjs)
        {
            float currentdistance;
            currentdistance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
            if (currentdistance < closestdistance)
            {
                closestdistance = currentdistance;
                foundInteractable = obj;
                oldClosetstInteractable = foundInteractable;
            }
        }
    }

    private void OnUse()
    {
        if (foundInteractable != null && foundInteractable.CanInteractWith(this))
        {
            foundInteractable.Interact(this);
        }
    }

    public void die()
    {
        if (isDead) return;

        isDead = true;
        if (GameManager.Instance.godmode) return;
        controller.enabled = false;
        Invoke("Reload", 0.25f);

        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.death);
    }

    private void Reload()
    {
        GameManager.ReloadLevel(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.Trigger)
        {
            Array.ForEach(other.gameObject.GetComponents<ITrigger>(), (t) => t.Trigger(this, TriggerCommand.Toggle));
        }

        if (Layers.CheckLayer(other.gameObject.layer, useMask)) //other.gameObject.layer == Layers.InteractAble)
        {
            if (other.gameObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactionObjs.Contains(interactable) == false)
                {
                    interactionObjs.Add(interactable);
                    ScanInteractables();
                }
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (Layers.CheckLayer(other.gameObject.layer, useMask))
        {
            RemoveInteractionObj(other.gameObject);
        }
    }
    public void RemoveInteractionObj(GameObject other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactable))
        {
            if (interactionObjs.Contains(interactable))
            {
                interactionObjs.Remove(interactable);
                ScanInteractables();
            }
        }
    }
}


//RaycastHit hit;
//Ray ray = GameManager.Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
//if (Physics.Raycast(ray, out hit, useDist * 2, useMask))
//{
//    Debug.Log("hit");
//    if (hit.collider.TryGetComponent(out IInteractable interactable) && Vector3.Distance(Center, hit.point) < useDist)
//    {
//        if (foundInteractable != interactable)
//        {
//            foundInteractable = interactable;
//            interactableChannel.RaiseEvent(interactable.GetInteractionHint());
//        }
//        return;
//    }
//}

//if (foundInteractable != null)
//{
//    foundInteractable = null;
//    interactableChannel.RaiseEvent(null);
//}