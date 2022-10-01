using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonColorText : Button
{
    public Text text;

    // Update is called once per frame
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        Color textColor;

        switch (state)
        {
            case SelectionState.Normal:
                textColor = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                textColor = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                textColor = colors.pressedColor;
                break;
            case SelectionState.Selected:
                textColor = colors.selectedColor;
                break;
            case SelectionState.Disabled:
                textColor = colors.disabledColor;
                break;
            default:
                textColor = colors.normalColor;
                break;
        }

        text.color = textColor;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIButtonColorText))]
    public class UIButtonColorTextEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            UIButtonColorText targetText = (UIButtonColorText)target;

            targetText.text = (Text)EditorGUILayout.ObjectField("Text", targetText.text, typeof(Text), true);

            // Show default inspector property editor
            base.OnInspectorGUI();
            //DrawDefaultInspector();
        }
    }
#endif
}
