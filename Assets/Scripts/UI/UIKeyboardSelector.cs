using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIKeyboardSelector : MonoBehaviour
{
    public void ShiftRight()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        UISelector uiSelector = selectedObject.transform.parent.GetComponent<UISelector>();
        if (uiSelector != null)
        {
            uiSelector.ShiftRight();
            return;
        }

        Toggle toggle = selectedObject.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.isOn = !toggle.isOn;
            return;
        }
    }

    public void ShiftLeft()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        UISelector uiSelector = selectedObject.transform.parent.GetComponent<UISelector>();
        if (uiSelector != null)
        {
            uiSelector.ShiftLeft();
            return;
        }

        Toggle toggle = selectedObject.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.isOn = !toggle.isOn;
            return;
        }
    }

    public void NavigateInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 navigation = context.ReadValue<Vector2>();
            if (navigation.x > 0.0f)
            {
                ShiftRight();
            }
            else if (navigation.x < 0.0f)
            {
                ShiftLeft();
            }
        }
    }
}
