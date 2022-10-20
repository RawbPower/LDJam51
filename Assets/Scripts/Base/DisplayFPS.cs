using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFPS : MonoBehaviour
{
    private Text fpsText;

    // Start is called before the first frame update
    void Start()
    {
        fpsText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = fps.ToString();
    }
}
