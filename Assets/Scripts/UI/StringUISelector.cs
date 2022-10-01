using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StringUISelector : UISelector
{
    public string[] stringLabels;
    public int initIndex;
    public UnityEvent<string> OnChanged;

    private int currentIndex;

    void Start()
    {
        currentIndex = initIndex;
        UpdateLabel();
    }

    public override void ShiftRight()
    {
        currentIndex++;
        if (cyclic)
        {
            currentIndex = currentIndex % stringLabels.Length;
        }
        else
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, stringLabels.Length - 1);
        }
        UpdateLabel();
    }

    public override void ShiftLeft()
    {
        currentIndex--;
        if (cyclic)
        {
            if (currentIndex < 0)
            {
                currentIndex += stringLabels.Length;
            }
            currentIndex = currentIndex % stringLabels.Length;
        }
        else
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, stringLabels.Length - 1);
        }
        UpdateLabel();
    }

    public override void UpdateLabel()
    {
        label.text = stringLabels[currentIndex];
        OnChanged?.Invoke(stringLabels[currentIndex]);
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }
}
