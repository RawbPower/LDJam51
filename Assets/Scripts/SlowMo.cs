using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Audio;

public class SlowMo : MonoBehaviour
{
    public Camera cam;
    public float slowMoRatio = 0.1f;
    public Text timerUI;
    public float maxTime = 10.0f;
    public Text timeUpUI;
    public float slowMoPitch;
    public float slowMoLowPass = 5000.0f;
    public AudioMixer audioMixer;
    public float pitchSmoothing = 1.0f;
    public float pitchSmoothingUp = 1.0f;
    public AudioReverbZone reverb;
    public GameObject postProcessingObject;

    private float desiredPitch = 1.0f;
    private bool completed;
    private bool timeUp = false;

    private float timer;

    private void Start()
    {
        timer = maxTime;
        completed = false;
    }

    public void SetCompleted(bool value)
    {
        completed = value;
    }

    private void Update()
    {
        if (!completed)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0.0f, maxTime);
            timerUI.text = timer.ToString("0.00");

            if (timer <= 0.0f && !timeUp)
            {
                timeUp = true;
                StartCoroutine(TimeUp()); ;
            }
        }

        if (desiredPitch == 1.0f)
        {
            float musicPitch = 1.0f;
            audioMixer.GetFloat("pitchMusic", out musicPitch);
            float smoothPitch = Mathf.Lerp(musicPitch, desiredPitch, pitchSmoothingUp);
            audioMixer.SetFloat("pitchMusic", smoothPitch);
            postProcessingObject.SetActive(true);
            postProcessingObject.SetActive(false);
        }
        else
        {
            reverb.enabled = true;
            float musicPitch = 1.0f;
            audioMixer.GetFloat("pitchMusic", out musicPitch);
            float smoothPitch = Mathf.Lerp(musicPitch, desiredPitch, pitchSmoothing);
            audioMixer.SetFloat("pitchMusic", smoothPitch);
            postProcessingObject.SetActive(true);
        }
    }

    IEnumerator TimeUp()
    {
        DoSlowMo();
        cam.GetComponent<CameraFollow>().enabled = true;
        cam.GetComponent<PixelPerfectCamera>().refResolutionX = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionX * 0.5f);
        cam.GetComponent<PixelPerfectCamera>().refResolutionY = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionY * 0.5f);
        timeUpUI.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        timeUpUI.gameObject.SetActive(false);
        FindObjectOfType<GameManager>().LoseGame();
    }

    public void DoSlowMo(float slowMoScale = -1.0f)
    {
        Time.timeScale = slowMoScale > 0.0 ? slowMoScale : slowMoRatio;
        Time.fixedDeltaTime = Time.timeScale * Time.fixedUnscaledDeltaTime;
        desiredPitch = slowMoPitch;
        audioMixer.SetFloat("lowpassMusic", slowMoLowPass);
    }

    public void ResumeNormalSpeed()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime;
        desiredPitch = 1.0f;
        audioMixer.SetFloat("lowpassMusic", 5000.0f);
    }
}
