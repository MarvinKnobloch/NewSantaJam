using UnityEngine;

namespace Santa
{
    [RequireComponent(typeof(Collider))]
    public class LevelTransition : MonoBehaviour
    {
        public SceneEnum levelIndex;
        public bool spawnAtEnd;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Layers.Spieler)
            {
                GameManager.Instance.spawnAtEnd = spawnAtEnd;
                GameManager.Instance.LoadScene(levelIndex);
            }
        }
    }
}
