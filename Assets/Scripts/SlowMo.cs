using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SlowMo : MonoBehaviour
{
    public Camera cam;
    public float slowMoRatio = 0.1f;
    public Text timerUI;
    public float maxTime = 10.0f;
    public Text timeUpUI;

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
    }

    public void ResumeNormalSpeed()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime;
    }
}
