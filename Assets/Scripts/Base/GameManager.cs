using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loseUI;
    public GameObject winUI;
    private int musicVolumeIndex = -1;
    private int sfxVolumeIndex = -1;
    private int screenSizeIndex = -1;
    private int fullScreenIndex = -1;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Start()
    {
        ResumeGame();
    }

    public void LoseGame()
    {
        loseUI.SetActive(true);
        PauseGame();
    }

    public void WinGame()
    {
        winUI.SetActive(true);
        PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }

    public void StartPause(float pauseTime)
    {
        StartCoroutine(PauseGame(pauseTime));
    }

    public IEnumerator PauseGame(float pauseTime)
    {
        Time.timeScale = 0.0f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1.0f;
    }

    public int GetMusicVolumeIndex()
    {
        return musicVolumeIndex;
    }

    public void SetMusicVolumeIndex(int value)
    {
        musicVolumeIndex = value;
    }

    public int GetSFXVolumeIndex()
    {
        return sfxVolumeIndex;
    }

    public void SetSFXVolumeIndex(int value)
    {
        sfxVolumeIndex = value;
    }

    public int GetScreenSizeIndex()
    {
        return screenSizeIndex;
    }

    public void SetScreenSizeIndex(int value)
    {
        screenSizeIndex = value;
    }

    public int GetFullScreenIndex()
    {
        return fullScreenIndex;
    }

    public void SetFullScreenIndex(int value)
    {
        fullScreenIndex = value;
    }
}

