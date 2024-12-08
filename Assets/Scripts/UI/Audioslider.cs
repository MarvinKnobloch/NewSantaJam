using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Audioslider : MonoBehaviour
{
    public AudioMixer audiomixer;
    public string gamevalue;
    private Slider slider;

    [SerializeField] private TextMeshProUGUI slidertext;

    private bool skipFirstSound;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        skipFirstSound = true;
    }
    private void OnEnable()
    {

        float soundvalue = PlayerPrefs.GetFloat(gamevalue);
        float textvalue;
        if (gamevalue == "Sound")
        {
            slider.value = soundvalue + 80;
            textvalue = soundvalue + 80;
        }
        else 
        {
            slider.value = soundvalue + 100;
            textvalue = (soundvalue + 100); 
        }
        slidertext.text = Mathf.Round(textvalue).ToString();
    }
    public void MusicValueChange(float slidervalue)
    {
        float textvalue = slidervalue;
        slidertext.text = Mathf.Round(textvalue).ToString();

        slidervalue -= 100;
        PlayerPrefs.SetInt("audiohasbeenchange", 1);
        
        audiomixer.SetFloat(gamevalue, slidervalue);
        bool gotvalue = audiomixer.GetFloat(gamevalue, out float soundvalue);            //verhindert das der audiomixer mehr als 0db haben kann
        if (gotvalue == true)
        {
            if (soundvalue > 5)
            {
                Debug.Log(soundvalue);
                audiomixer.SetFloat(gamevalue, 0);
            }
        }
        
        PlayerPrefs.SetFloat(gamevalue, slidervalue);
    }
    public void soundeffectvaluechange(float slidervalue)
    {
        float textvalue = slidervalue;
        slidertext.text = Mathf.Round(textvalue).ToString();

        slidervalue -= 80;

        PlayerPrefs.SetInt("audiohasbeenchange", 1);

        audiomixer.SetFloat(gamevalue, slidervalue);
        bool gotvalue = audiomixer.GetFloat(gamevalue, out float soundvalue);            //verhindert das der audiomixer mehr als 10db haben kann
        if (gotvalue == true)
        {
            if (soundvalue > 20)
            {
                Debug.Log(soundvalue);
                audiomixer.SetFloat(gamevalue, 10);
            }
        }

        PlayerPrefs.SetFloat(gamevalue, slidervalue);

        if(skipFirstSound == true)
        {
            skipFirstSound = false;
            return;
        }
        else AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);


    }
    public void MasterValuechange(float slidervalue)
    {
        float textvalue = slidervalue;
        slidertext.text = Mathf.Round(textvalue).ToString();

        slidervalue -= 100;

        PlayerPrefs.SetInt("audiohasbeenchange", 1);

        audiomixer.SetFloat(gamevalue, slidervalue);
        bool gotvalue = audiomixer.GetFloat(gamevalue, out float soundvalue);            //verhindert das der audiomixer mehr als 0db haben kann
        if (gotvalue == true)
        {
            if (soundvalue > 0)
            {
                Debug.Log(soundvalue);
                audiomixer.SetFloat(gamevalue, 0);
            }
        }

        PlayerPrefs.SetFloat(gamevalue, slidervalue);
    }
}
