using UnityEngine;

namespace Santa
{
    [RequireComponent(typeof(Collider))]
    public class LevelTransition : MonoBehaviour
    {
        public SceneEnum levelIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Layers.Spieler)
            {
                GameManager.Instance.LoadScene(levelIndex);
            }
        }
    }
}
