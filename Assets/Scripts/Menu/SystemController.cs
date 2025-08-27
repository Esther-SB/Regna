using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemController : MonoBehaviour
{
    public enum Language
    {
        Español,
        Inglés,
    }

    public enum DialogueSpeed
    {
        Lento,
        Normal,
        Rápido,
        MuyRápido,
    }

    [Header("Idioma")]
    public TMP_Dropdown languageDropdown;

    [Header("Velocidad de diálogos")]
    public TMP_Dropdown dialogueSpeedDropDown;

    [Header("Temblor de pantalla")]
    public Toggle screenShakeToggle;

    void Start()
    {
        // Desactivar listeners antes de asignar valores o por defecto coje el valor del inspector
        languageDropdown.onValueChanged.RemoveAllListeners();
        dialogueSpeedDropDown.onValueChanged.RemoveAllListeners();
        screenShakeToggle.onValueChanged.RemoveAllListeners();

        Language savedLang = (Language)PlayerPrefs.GetInt("Language", (int)Language.Español);
        languageDropdown.value = (int)savedLang;
        ChangeLanguage((int)savedLang);

        DialogueSpeed savedSpeed = (DialogueSpeed)
            PlayerPrefs.GetInt("DialogueSpeed", (int)DialogueSpeed.Normal);
        dialogueSpeedDropDown.value = (int)savedSpeed;
        ChangeDialogueSpeed((int)savedSpeed);

        bool shakeEnabled = PlayerPrefs.GetInt("ScreenShake", 1) == 1;
        screenShakeToggle.isOn = shakeEnabled;
        SetScreenShake(shakeEnabled);

        // Reactivar listeners
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);
        dialogueSpeedDropDown.onValueChanged.AddListener(ChangeDialogueSpeed);
        screenShakeToggle.onValueChanged.AddListener(SetScreenShake);
    }

    /*
    Metodo para cambiar el idioma
    */
    public void ChangeLanguage(int langIndex)
    {
        Language selectedLang = (Language)langIndex;
        PlayerPrefs.SetInt("Language", (int)selectedLang);
    }

    /*
    Metodo para cambiar la velocidad de los diálogos
    */
    public void ChangeDialogueSpeed(int speedIndex)
    {
        DialogueSpeed selectedSpeed = (DialogueSpeed)speedIndex;
        PlayerPrefs.SetInt("DialogueSpeed", (int)selectedSpeed);
    }

    /*
    Metodo para activar/desactivar el temblor de pantalla
    */
    public void SetScreenShake(bool shakeEnabled)
    {
        PlayerPrefs.SetInt("ScreenShake", shakeEnabled ? 1 : 0);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
