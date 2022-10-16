using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    public Text time;
    public float timerDelay;
    public Button[] buttons;
    public float buttonDelay;

    public void Awake()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive (false);
        }
        time.gameObject.SetActive(false);
    }

    public void SetTimeString(string timeString)
    {
        time.text = timeString;
    }

    public void OnEnable()
    {
        StartCoroutine(ShowButtons());
    }

    IEnumerator ShowButtons()
    {
        yield return new WaitForSecondsRealtime(timerDelay);
        time.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(buttonDelay);
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }
}
