using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Santa;

public class MouseSlider : MonoBehaviour
{
    public string playerPrefString;
    private Slider slider;

    [SerializeField] private TextMeshProUGUI slidertext;
    [SerializeField] private Setting settingValue;

    private enum Setting
    {
        xMouseValue,
        yMouseValue,
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    private void OnEnable()
    {
        float value = PlayerPrefs.GetFloat(playerPrefString);
        slider.value = value;
        slidertext.text = Mathf.Round(value * 100).ToString();
    }
    public void ChangeValue(float slidervalue)
    {
        PlayerPrefs.SetInt("mouseSettingHasBeenChange", 1);

        slidertext.text = Mathf.Round(slidervalue * 100).ToString();

        PlayerPrefs.SetFloat(playerPrefString, slidervalue);

        switch (settingValue)
        {
            case Setting.xMouseValue:
                GameManager.Settings.mouseSensitivityX = slidervalue;
                break;
            case Setting.yMouseValue:
                GameManager.Settings.mouseSensitivityY = slidervalue;
                break;
        }
    }
}
