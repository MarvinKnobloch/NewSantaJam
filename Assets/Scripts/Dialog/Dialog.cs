using UnityEngine;
using UnityEngine.Playables;

namespace Santa
{
    [CreateAssetMenu(fileName = "Dialog", menuName = "Dialog/Simple")]
    public class Dialog : ScriptableObject
    {
        public Message[] messages;
    }

    [System.Serializable]
    public class Message
    {
        public string name;
        [Multiline(4)]
        public string text;
        //public Color color = Color.white;
    }
}