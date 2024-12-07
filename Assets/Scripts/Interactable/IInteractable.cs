using UnityEngine;

namespace Santa
{
    public interface IInteractable
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        // L�sst ein Objekt mit dem Interactable interagieren
        public void Interact(MonoBehaviour user);
        // Gibt einen beschreibenden Text zur�ck, der dem Spieler angezeigt wird
        public string GetInteractionHint();
        // Gibt zur�ck, ob ein Objekt mit dem Interactable interagieren kann
        public bool CanInteractWith(MonoBehaviour user);
    }
}
