using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Sliders de Volumen")]
    public Slider masterVol;
    public Slider musicVol;
    public Slider sfxVol;
    public Slider effectsVol;

    [Header("Audio Mixer")]
    public AudioMixer mainAudioMixer;

    private const float minSliderValue = 0.0001f;

    private void Start()
    {
        // Cargar valores guardados o usar 1 como predeterminado
        masterVol.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVol.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVol.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        effectsVol.value = PlayerPrefs.GetFloat("EffectsVolume", 1f);

        // Aplicar inmediatamente los valores al AudioMixer
        ChangeMasterVolume();
        ChangeMusicVolume();
        ChangeSFXVolume();
        ChangeEffectsVolume();
    }

    /*
        Metodo para cambiar el volumen maestro
    */
    public void ChangeMasterVolume()
    {
        SetVolume("MasterVolume", masterVol.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVol.value);
    }

    /*
        Metodo para cambiar el volumen de la musica
    */
    public void ChangeMusicVolume()
    {
        SetVolume("MusicVolume", musicVol.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVol.value);
    }

    /*
        Metodo para cambiar el volumen de los SFX
    */
    public void ChangeSFXVolume()
    {
        SetVolume("SFXVolume", sfxVol.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVol.value);
    }

    /*
        Metodo para cambiar el volumen de los efectos
    */
    public void ChangeEffectsVolume()
    {
        SetVolume("EffectsVolume", effectsVol.value);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVol.value);
    }

    /*
        Metodo para cambiar el volumen del parametro dado
    */
    private void SetVolume(string parameterName, float sliderValue)
    {
        float volume = Mathf.Max(sliderValue, minSliderValue);
        mainAudioMixer.SetFloat(parameterName, Mathf.Log10(volume) * 20f);
    }

    /*
        Metodo para guardar los valores al salir de la escena / desactivar objeto
    */
    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
