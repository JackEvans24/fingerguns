using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSettings : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityText;

    void Start()
    {
        this.UpdateSensitivity(SettingsManager.Sensitivity);
    }

    public void SensitivityChanged()
    {
        var sensitivity = Mathf.RoundToInt(sensitivitySlider.value);

        this.UpdateSensitivity(sensitivity);
        SettingsManager.Sensitivity = sensitivity;
    }

    private void UpdateSensitivity(int sensitivity)
    {
        sensitivitySlider.value = sensitivity;
        sensitivityText.text = sensitivity.ToString();
    }
}
