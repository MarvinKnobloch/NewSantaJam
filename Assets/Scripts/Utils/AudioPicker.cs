using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Audio/Audio Picker")]
    public class AudioPicker : ScriptableObject
    {
        public float pitchSpread = 0.2f;
        public AudioClip[] clips;

        private int last = -1;

        public AudioClip Next()
        {
            int idx;
            do
            {
                idx = Random.Range(0, clips.Length);
            }
            while (clips.Length > 1 && idx == last);
            last = idx;
            return clips[idx];
        }

        public void Play(AudioSource source, float volume = 1f)
        {
            source.loop = false;
            source.pitch = Pitch();
            source.PlayOneShot(Next(), volume);
        }

        public float Pitch()
        {
            return 1f + pitchSpread * Random.value - pitchSpread * Random.value;
        }
    }
}
