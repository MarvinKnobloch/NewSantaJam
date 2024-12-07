using System;
using UnityEngine;

namespace Santa
{
    [Serializable]
    public class Savestate
    {
        public SceneEnum sceneIndex;

        public Savestate Clone()
        {
            var clone = new Savestate();
            clone.sceneIndex = sceneIndex;
            return clone;
        }

        public void Save()
        {
            PlayerPrefs.SetInt("saveScene", (int) sceneIndex);
        }

        public void Load()
        {
            sceneIndex = (SceneEnum) PlayerPrefs.GetInt("saveScene", (int) SceneEnum.Hauptmenü);
        }
    }
}
