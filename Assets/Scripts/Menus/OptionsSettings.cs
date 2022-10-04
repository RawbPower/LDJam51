using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public IntUISelector music;
    public IntUISelector sfx;
    public StringUISelector size;
    public Toggle fullscreen;

    void Awake()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.GetMusicVolumeIndex() >= 0)
            {
                music.initNumber = GameManager.instance.GetMusicVolumeIndex();
            }

            if (GameManager.instance.GetSFXVolumeIndex() >= 0)
            {
                sfx.initNumber = GameManager.instance.GetSFXVolumeIndex();
            }

            if (GameManager.instance.GetScreenSizeIndex() >= 0)
            {
                //size.initIndex = GameManager.instance.GetScreenSizeIndex();
            }

            if (GameManager.instance.GetFullScreenIndex() >= 0)
            {
                //fullscreen.isOn = Screen.fullScreen ? true : false;
            }
        }

        music.UpdateLabel();
        sfx.UpdateLabel();
        //size.UpdateLabel();
        //fullscreen.isOn = Screen.fullScreen ? true : false;
    }

   public void SaveOptionsSettings()
   {
        if (GameManager.instance != null)
        {
            GameManager.instance.SetMusicVolumeIndex(music.GetCurrentNumber());
            GameManager.instance.SetSFXVolumeIndex(sfx.GetCurrentNumber());
            GameManager.instance.SetScreenSizeIndex(size.GetCurrentIndex());
            GameManager.instance.SetFullScreenIndex(fullscreen.isOn ? 1 : 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetMusicVolume(float volumeRatio)
    {
        float clampedVolumeRatio = Mathf.Clamp(volumeRatio, 0.0f, 1.0f);
        float musicVolume = 0.0f;
        if (clampedVolumeRatio == 0.0f)
        {
            musicVolume = -80.0f;
        }
        else
        {
            musicVolume = Mathf.Log10(clampedVolumeRatio) * 20.0f;
        }
        audioMixer.SetFloat("volumeMusic", musicVolume);
    }

    public void SetSFXVolume(float volumeRatio)
    {
        float clampedVolumeRatio = Mathf.Clamp(volumeRatio, 0.0f, 1.0f);
        float sfxVolume = 0.0f;
        if (clampedVolumeRatio == 0.0f)
        {
            sfxVolume = -80.0f;
        }
        else
        {
            sfxVolume = Mathf.Log10(clampedVolumeRatio) * 20.0f;
        }
        audioMixer.SetFloat("volumeSFX", sfxVolume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetScreenSize(string screensize)
    {
        string[] sizes = screensize.Split("x", System.StringSplitOptions.None);
        int screenWidth = int.Parse(sizes[0]);
        int screenHeight = int.Parse(sizes[1]);
        Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen);
    }
}
