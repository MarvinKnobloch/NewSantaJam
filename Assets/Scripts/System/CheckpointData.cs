using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santa
{
    public class CheckpointData
    {
        public bool active;
        public SceneEnum sceneIndex;
        public float[] _pos = new float[3];
        public float rotation;

        public Dictionary<int, Memento> mementos;
        public Savestate progress;

        public Vector3 position => new Vector3(_pos[0], _pos[1], _pos[2]);

        public CheckpointData()
        {
            active = false;
            sceneIndex = SceneEnum.Keine;
            rotation = 0;
            mementos = new Dictionary<int, Memento>();
        }

        public CheckpointData(Scene scene, Transform t, Savestate progress)
        {
            this.active = true;
            this.sceneIndex = (SceneEnum)scene.buildIndex;
            t.position.ToFloatVec(ref _pos);
            rotation = t.rotation.eulerAngles.y;
            mementos = new Dictionary<int, Memento>();

            this.progress = progress.Clone();
        }

        public void UpdateCheckpoint(Scene scene, Transform t, Savestate progress)
        {
            this.active = true;
            this.sceneIndex = (SceneEnum)scene.buildIndex;
            t.position.ToFloatVec(ref _pos);
            rotation = t.rotation.eulerAngles.y;
            this.progress = progress.Clone();

            PlayerPrefs.SetInt("SceneNumber", scene.buildIndex);
            PlayerPrefs.SetFloat("SavePlayerXPosition", t.position.x);
            PlayerPrefs.SetFloat("SavePlayerYPosition", t.position.y);
            PlayerPrefs.SetFloat("SavePlayerZPosition", t.position.z);
            PlayerPrefs.SetFloat("SavePlayerRotation", t.rotation.eulerAngles.y);
        }
    }
}
