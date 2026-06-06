using UnityEngine;
using UnityEngine.UI;

public class DesolationSettingsController : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider brightnessSlider;
    public Slider sensitivitySlider;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string BrightnessKey = "Brightness";
    private const string SensitivityKey = "Sensitivity";
    private const string GraphicsQualityKey = "GraphicsQuality";

    private void Start()
    {
        LoadSettings();
        HookEvents();
    }

    private void LoadSettings()
    {
        SetSlider(masterVolumeSlider, PlayerPrefs.GetFloat(MasterVolumeKey, 1.0f));
        SetSlider(musicVolumeSlider, PlayerPrefs.GetFloat(MusicVolumeKey, 0.8f));
        SetSlider(sfxVolumeSlider, PlayerPrefs.GetFloat(SfxVolumeKey, 0.9f));
        SetSlider(brightnessSlider, PlayerPrefs.GetFloat(BrightnessKey, 0.7f));
        SetSlider(sensitivitySlider, PlayerPrefs.GetFloat(SensitivityKey, 0.6f));
    }

    private void HookEvents()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(MasterVolumeKey, v));
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(MusicVolumeKey, v));
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(SfxVolumeKey, v));
        if (brightnessSlider != null) brightnessSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(BrightnessKey, v));
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(SensitivityKey, v));
    }

    private void SetSlider(Slider slider, float value)
    {
        if (slider != null)
        {
            slider.value = value;
        }
    }

    public void SetGraphicsLow()
    {
        SetGraphicsQuality(0);
    }

    public void SetGraphicsMedium()
    {
        SetGraphicsQuality(1);
    }

    public void SetGraphicsHigh()
    {
        SetGraphicsQuality(2);
    }

    private void SetGraphicsQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt(GraphicsQualityKey, qualityIndex);

        if (qualityIndex >= 0 && qualityIndex < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
        }
    }
}
