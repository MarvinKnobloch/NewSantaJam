using UnityEngine;

namespace Santa
{
    public class LevelTransition : MonoBehaviour
    {
        public SceneEnum levelIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Layers.Spieler)
            {
                SwitchLevel();
            }
        }

        public void SwitchLevel()
        {
            GameManager.Instance.LoadScene(levelIndex);
        }
    }
}
