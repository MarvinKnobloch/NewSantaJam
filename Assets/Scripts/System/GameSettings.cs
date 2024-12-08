using System;
using UnityEngine;

namespace Santa
{
    public class GameSettings
    {
        [Range(0, 100)]
        public int musicVolume = 80;
        [Range(0, 100)]
        public int soundVolume = 80;
        [Range(0, 100)]
        public int audioKeyVolume = 80;

        public float mouseSensitivityX = 0.5f;
        public float mouseSensitivityY = 0.33f;

        // Speicher Flag
        [NonSerialized]
        public bool isDirty;

        #region Properties
        public void SetFloat(string name, float value)
        {
            if (this == GameManager.Settings)
            {
                Debug.LogError("Live Settings können nicht geändert werden");
                return;
            }
            var field = GetType().GetField(name);
            if (field.FieldType == typeof(float))
                field.SetValue(this, value);
            else
                field.SetValue(this, Mathf.RoundToInt(value));
            isDirty = true;
        }

        public void SetInt(string name, int value)
        {
            if (this == GameManager.Settings)
            {
                Debug.LogError("Live Settings können nicht geändert werden");
                return;
            }
            GetType().GetField(name).SetValue(this, value);
            isDirty = true;
        }

        public void SetBool(string name, bool value)
        {
            if (this == GameManager.Settings)
            {
                Debug.LogError("Live Settings können nicht geändert werden");
                return;
            }
            GetType().GetField(name).SetValue(this, value);
            isDirty = true;
        }

        public float GetFloat(string name)
        {
            var field = GetType().GetField(name);
            if (field.FieldType == typeof(float))
                return (float)field.GetValue(this);
            else
                return (int)field.GetValue(this);
        }

        public int GetInt(string name)
        {
            return (int)GetType().GetField(name).GetValue(this);
        }

        public bool GetBool(string name)
        {
            return (bool)GetType().GetField(name).GetValue(this);
        }
        #endregion

        public void Save()
        {
            PlayerPrefs.SetInt("MusicVolume", musicVolume);
            PlayerPrefs.SetInt("SoundVolume", soundVolume);
            PlayerPrefs.SetInt("AudioKeyVolume", audioKeyVolume);
            PlayerPrefs.SetFloat("MouseSensitivityX", mouseSensitivityX);
            PlayerPrefs.SetFloat("MouseSensitivityY", mouseSensitivityY);
            PlayerPrefs.Save();
            isDirty = false;
        }

        public GameSettings Load()
        {
            Save();
            musicVolume = PlayerPrefs.GetInt("MusicVolume", musicVolume);
            soundVolume = PlayerPrefs.GetInt("SoundVolume", soundVolume);
            audioKeyVolume = PlayerPrefs.GetInt("AudioKeyVolume", audioKeyVolume);
            mouseSensitivityX = PlayerPrefs.GetFloat("MouseSensitivityX", mouseSensitivityX);
            mouseSensitivityY = PlayerPrefs.GetFloat("MouseSensitivityY", mouseSensitivityY);
            isDirty = false;
            return this;
        }
    }
}