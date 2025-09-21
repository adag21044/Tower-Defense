using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public AudioMixer audioMixer;

    public void ChangedGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        ToastManager.Instance.ShowToast($"Graphics Quality set to {QualitySettings.names[qualityIndex]}");
        Debug.Log($"Graphics Quality set to {QualitySettings.names[qualityIndex]}");
    }

    public void ChangeSoundVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20f; // 0.0001 → -80 dB, 1 → 0 dB
        audioMixer.SetFloat("MasterVolume", dB);

        ToastManager.Instance.ShowToast($"Volume set to {Mathf.RoundToInt(dB)} dB");
        Debug.Log($"Volume set to {Mathf.RoundToInt(dB)} dB");
    }
}