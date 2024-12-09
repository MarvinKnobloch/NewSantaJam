using Events;
using Santa;
using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private float useDist = 2f;
    [SerializeField] private LayerMask useMask;
    [SerializeField] private float deathFallHeight;
    [SerializeField] private float deathHeight = -24f;

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

    public bool IsRunning()
    {
        return controller.IsRunning();
    }

    private void ScanInteractables()
    {
        RaycastHit hit;
        Ray ray = GameManager.Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out hit, useDist * 2, useMask))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable) && Vector3.Distance(Center, hit.point) < useDist)
            {
                if (foundInteractable != interactable)
                {
                    foundInteractable = interactable;
                    interactableChannel.RaiseEvent(interactable.GetInteractionHint());
                }
                return;
            }
        }

        if (foundInteractable != null)
        {
            foundInteractable = null;
            interactableChannel.RaiseEvent(null);
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
        if (GameManager.Instance.godmode) return;
        controller.enabled = false;
        Invoke("Reload", 0.25f);
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
    }
}
