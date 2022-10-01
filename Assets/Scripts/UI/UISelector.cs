using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UISelector : MonoBehaviour
{
    public bool cyclic;
    public Text label;

    public abstract void ShiftRight();

    public abstract void ShiftLeft();

    public abstract void UpdateLabel();
}
