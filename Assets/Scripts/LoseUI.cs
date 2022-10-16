using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseUI : MonoBehaviour
{
    public Button[] buttons;
    public float buttonDelay;

    // Start is called before the first frame update
    public void Awake()
    {
        FindObjectOfType<GameManager>().loseUI = gameObject;
        gameObject.SetActive(false);

        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void OnEnable()
    {
        StartCoroutine(ShowButtons());
    }

    IEnumerator ShowButtons()
    {
        yield return new WaitForSecondsRealtime(buttonDelay);
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }
}
