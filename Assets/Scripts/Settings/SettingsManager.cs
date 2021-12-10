using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance;
    private PlayerSettings settings;

    [SerializeField] private int defaultSensitivity = 10;

    public static int Sensitivity
    {
        get => instance.settings.Sensitivity;
        set
        {
            if (instance?.settings == null)
                return;

            instance.settings.Sensitivity = value;
            OnSettingsChanged?.Invoke(instance, null);
        }
    }

    public static EventHandler OnSettingsChanged;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        DontDestroyOnLoad(this);

        this.settings = new PlayerSettings() { Sensitivity = this.defaultSensitivity };
    }
}
