using UnityEngine;

namespace Santa
{
    public interface IInteractable
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        // Lässt ein Objekt mit dem Interactable interagieren
        public void Interact(MonoBehaviour user);
        // Gibt einen beschreibenden Text zurück, der dem Spieler angezeigt wird
        public string GetInteractionHint();
        // Gibt zurück, ob ein Objekt mit dem Interactable interagieren kann
        public bool CanInteractWith(MonoBehaviour user);
    }
}
