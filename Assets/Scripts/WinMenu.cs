using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    public Text time;
    
    public void SetTimeString(string timeString)
    {
        time.text = timeString;
    }
}
