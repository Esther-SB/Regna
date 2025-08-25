using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicController : MonoBehaviour
{
    public enum Resolution
    {
        _360p,
        _720p,
        _1080p,
        _1440p,
    }

    [Header("Resolución pantalla")]
    public TMP_Dropdown resolutionDropdown;

    [Header("Brillo pantalla")]
    public Slider brightnessSlider;

    void Start()
    {
        // Desactivar listeners antes de asignar valores o por defecto coje el valor del inspector
        resolutionDropdown.onValueChanged.RemoveAllListeners();

        Resolution savedRes = (Resolution)PlayerPrefs.GetInt("Resolution", (int)Resolution._1080p);
        resolutionDropdown.value = (int)savedRes;
        ChangeResolution((int)savedRes);

        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        ChangeBrightness();
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    /*
     Metodo para cambiar la resolución de la pantalla
    */
    public void ChangeResolution(int resIndex)
    {
        Resolution selectedRes = (Resolution)resIndex;
        PlayerPrefs.SetInt("Resolution", (int)selectedRes);
    }

    /*
     Metodo para cambiar el brillo de la pantalla
    */
    public void ChangeBrightness()
    {
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
