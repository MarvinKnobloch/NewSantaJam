using Santa;
using UnityEngine;
using UnityEngine.Playables;

namespace Santa
{
    public class LevelStartController : MonoBehaviour
    {
        [SerializeField] private Player playerPrefab;
        public PlayableDirector intro;

        void Start()
        {
            GameManager.Instance.LevelStartControllerAwake(this);
        }

        public void CreatePlayer()
        {
            Instantiate(playerPrefab, transform.position, transform.rotation);
        }

        public SceneEnum GetScene()
        {
            return (SceneEnum)gameObject.scene.buildIndex;
        }
    }
}