using Events;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Santa
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        private static GameSettings settings;
        public static bool testMode = false;

        [SerializeField] private AudioMixer audioMixer;

        private new Camera camera;

        [Header("UI")]
        [SerializeField] private GameObject gameUI;

        [Header("Events")]
        [SerializeField] private BoolEventChannelSO gameUIChannel;

        [Space]
        [Header("DEBUGGING")]
        public LevelStartController debugStartpoint;
        [Tooltip("Alle Türen können manuell geöffnet werden")]
        public bool unlockedDoors = false;
        public bool godmode = false;

        #region Properties
        public static GameSettings Settings => settings;
        public static CheckpointData Checkpoint => Instance.checkpoint;

        public static Camera Camera
        {
            get
            {
                if (Instance.camera == null)
                    Instance.camera = Camera.main;
                return Instance.camera;
            }
        }
        #endregion

        private Savestate savestate;
        private CheckpointData checkpoint;
        private bool awaitSceneLoading;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

#if !UNITY_EDITOR
            unlockedDoors = false;
            godmode = false;
#else
            testMode = true;
#endif

            DontDestroyOnLoad(gameObject);
            gameUIChannel.OnEventRaised += ActivateGameUI;
            settings = new GameSettings().Load();
            ResetProgress();

            var sceneValues = System.Enum.GetValues(typeof(SceneEnum));
            var scenes = SceneManager.sceneCountInBuildSettings;
            if (!testMode && (sceneValues.Length - 1) != scenes)
            {
                Debug.LogError("Szene Enum ist nicht aktuell! Enums: " + (sceneValues.Length - 1) + " / Szenen: " + scenes);
                Application.Quit();
                return;
            }
            awaitSceneLoading = true;
        }

        private void Start()
        {
            if(Player.Instance != null)
            {
                gameUI.SetActive(true);
            }
        }

        // Wird beim Start jeder neuen Szene aufgerufen
        public void LevelStartControllerAwake(LevelStartController levelStart)
        {
            if (awaitSceneLoading) SceneLoaded(levelStart);
        }

        private void SceneLoaded(LevelStartController levelStart)
        {
            awaitSceneLoading = false;
            //var currentScene = levelStart.GetScene();

            // Gibt es einen Checkpoint?
            if (checkpoint.active && checkpoint.sceneIndex == levelStart.GetScene())
            {
                LoadLastCheckpoint();
                levelStart.transform.SetLocalPositionAndRotation(checkpoint.position, Quaternion.Euler(0, checkpoint.rotation, 0));
            }
            else
            {
                Debug.Log("Registriere Rücksetzpunkt in Szene " + levelStart.gameObject.scene.name);
                CreateCheckpoint(levelStart.gameObject.scene, levelStart.transform);

                savestate.sceneIndex = levelStart.GetScene();
                savestate.Save();
            }

            levelStart.CreatePlayer();
            gameUI.SetActive(true);
        }

        public static void ReloadLevel(bool loadCheckpoint)
        {
            Instance.checkpoint.active = loadCheckpoint;
            if (!loadCheckpoint)
            {
                Instance.checkpoint.sceneIndex = Instance.savestate.sceneIndex;
            }
            Instance.awaitSceneLoading = true;

            Destroy(Player.Instance.gameObject);
            SceneManager.LoadScene((int)Instance.checkpoint.sceneIndex);
        }

        private void OnDestroy()
        {
            gameUIChannel.OnEventRaised -= ActivateGameUI;
        }

        public void ActivateGameUI(bool active)
        {
            gameUI.SetActive(active);
        }


        public static void ExitGame()
        {
            try
            {
                Settings.Save();
            }
            finally
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        #region Checkpoints
        public void CreateCheckpoint(Scene scene, Transform playerSpawnpoint)
        {
            var keys = new List<int>(checkpoint.mementos.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                try
                {
                    var m = checkpoint.mementos[keys[i]];
                    m.data = m.owner.GetData();
                    checkpoint.mementos[keys[i]] = m;
                }
                catch { /* ignore */ }
            }
            checkpoint.UpdateCheckpoint(scene, playerSpawnpoint, savestate);
            Settings.Save();
        }

        public void AddMementoObject(IMemento owner)
        {
            int id = owner.UUID;
            Memento m;
            if (checkpoint.mementos.TryGetValue(id, out m))
            {
                //Debug.Log("Überschreibe Memento " + owner.gameObject.name + " = " + id);
                m.owner = owner;
                checkpoint.mementos[id] = m;
            }
            else
            {
                //Debug.Log("Neues Memento " + owner.gameObject.name + " = " + id);
                checkpoint.mementos.Add(id, new Memento(owner));
            }
        }

        public void LoadLastCheckpoint()
        {
            Debug.Log("LoadLastCheckpoint mit " + checkpoint.mementos.Count + " Datensätzen.");
            foreach (var memento in checkpoint.mementos.Values)
            {
                try
                {
                    if (memento.owner != null)
                        memento.owner.SetData(memento.data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public void ResetProgress()
        {
            checkpoint = new CheckpointData();
            savestate = new Savestate();
        }
        #endregion

        public void SetBGMVolume(int volume)
        {
            audioMixer.SetFloat("bgmVolume", volume - 80);
        }
        public void SetSoundVolume(int volume)
        {
            audioMixer.SetFloat("soundVolume", volume - 80);
        }

        public void SetAudioKeysVolume(int volume)
        {
            audioMixer.SetFloat("audiokeysVolume", volume - 80);
        }
    }
}