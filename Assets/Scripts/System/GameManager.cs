using Events;
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

            var currentScene = SceneManager.GetActiveScene().buildIndex;
            gameUI.SetActive(currentScene > 0);

            var sceneValues = System.Enum.GetValues(typeof(SceneEnum));
            var scenes = SceneManager.sceneCountInBuildSettings;
            if (!testMode && (sceneValues.Length - 1) != scenes)
            {
                Debug.LogError("Szene Enum ist nicht aktuell! Enums: " + (sceneValues.Length - 1) + " / Szenen: " + scenes);
                Application.Quit();
                return;
            }
        }

        // Wird beim Start jeder neuen Szene aufgerufen
        public void LevelStartControllerAwake(LevelStartController levelStart)
        {
            // TODO: Implement this method
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