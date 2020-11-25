using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using Photon.Pun;
using UnityEngine.Rendering.PostProcessing;

public class SettingsMenu : MonoBehaviour
{

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle postProcessingToggle;
    public TMP_Dropdown qualityDropdown;
    public PostProcessLayer postProcessingLayer;
    public AudioMixer musicMixer;
    public AudioMixer soundEffectMixer;
    public Slider musicSlider;
    public Slider soundFxSlider;

    Resolution[] resolutions;


    void LoadPrefs()
    {
        if (PlayerPrefs.HasKey("quality"))
        {
            int quality = PlayerPrefs.GetInt("quality");
            SetQuality(quality);
            qualityDropdown.value = quality;
        }
        if (PlayerPrefs.HasKey("resolution"))
        {
            SetResolution(PlayerPrefs.GetInt("resolution"));
        }
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
            fullscreenToggle.isOn = isFullscreen;
            SetFullScreen(isFullscreen);
        }
        if (PlayerPrefs.HasKey("postprocessing"))
        {
            bool postProcessing = PlayerPrefs.GetInt("postprocessing") == 1;
            postProcessingToggle.isOn = postProcessing;
            SetPostProcessing(postProcessing);
        }
        if(PlayerPrefs.HasKey("musicVolume"))
        {

            SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {

            SetSoundEffectsVolume(PlayerPrefs.GetFloat("sfxVolume"));
            soundFxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        }
    }

    public void SetPostProcessing(bool value)
    {
        postProcessingLayer.enabled = value;
        PlayerPrefs.SetInt("postprocessing", value ? 1 : 0);
    }

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        LoadPrefs();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        int i = 0;

        foreach(Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            options.Add(option);

            if(resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
            i++;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();


        gameObject.SetActive(false);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitGame()
    {
        ApplicationManager.Instance.Quit();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectMixer.SetFloat("sfxVolume", volume);
        PlayerPrefs.SetFloat("sfxVolume", volume);

    }
}
