using UnityEngine;
using UnityEngine.UI;

public class SlowMo : MonoBehaviour
{
    public float slowMoRatio = 0.1f;
    public Text timerUI;
    public float maxTime = 10.0f;

    private float timer;

    private void Start()
    {
        timer = maxTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0.0f, maxTime);
        timerUI.text = timer.ToString("0.00");

        if (timer <= 0.0f)
        {
            FindObjectOfType<GameManager>().LoseGame();
        }
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
