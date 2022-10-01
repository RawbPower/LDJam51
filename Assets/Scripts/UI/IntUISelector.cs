using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class IntUISelector : UISelector
{
    public int maxNumber;
    public int initNumber;
    public UnityEvent<float> OnChanged;

    private int currentNumber;

    // Start is called before the first frame update
    void Start()
    {
        currentNumber = initNumber;
        UpdateLabel();
    }

    public override void ShiftRight()
    {
        currentNumber++;
        if (cyclic)
        {
            currentNumber = currentNumber % (maxNumber + 1);
        }
        else
        {
            currentNumber = Mathf.Clamp(currentNumber, 0, maxNumber);
        }
        UpdateLabel();
    }

    public override void ShiftLeft()
    {
        currentNumber--;
        if (cyclic)
        {
            if (currentNumber < 0)
            {
                currentNumber += (maxNumber + 1);
            }
            currentNumber = currentNumber % (maxNumber + 1);
        }
        else
        {
            currentNumber = Mathf.Clamp(currentNumber, 0, maxNumber);
        }
        UpdateLabel();
    }

    public override void UpdateLabel()
    {
        label.text = currentNumber.ToString();
        float ratio = (float)currentNumber / (float)maxNumber;
        OnChanged?.Invoke(ratio);
    }

    public int GetCurrentNumber()
    {
        return currentNumber;
    }
}
