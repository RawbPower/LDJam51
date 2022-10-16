using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILevelSelectButton : Button
{
    public int level = 1;
    public string levelName;
    public Text text;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        switch (state)
        {
            case SelectionState.Highlighted:
            case SelectionState.Pressed:
            case SelectionState.Selected:
                text.text = levelName;
                break;
            default:
                text.text = "";
                break;
        }
    }

    public void StartLevel()
    {
        //StartCoroutine(LevelAnimation());
        SceneManager.LoadScene(level + 1);
    }

    IEnumerator LevelAnimation()
    {
        GetComponent<Animator>().SetTrigger("Spin");
        yield return new WaitForSecondsRealtime(0.42f);
        SceneManager.LoadScene(level + 1);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UILevelSelectButton))]
    public class UILevelSelectButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            UILevelSelectButton targetButton = (UILevelSelectButton)target;

            targetButton.level = EditorGUILayout.IntField("Level", targetButton.level);
            targetButton.levelName = EditorGUILayout.TextField("Level Name", targetButton.levelName);
            targetButton.text = (Text)EditorGUILayout.ObjectField("Text UI", targetButton.text, typeof(Text), true);

        // Show default inspector property editor
        base.OnInspectorGUI();
            //DrawDefaultInspector();
        }
    }
#endif
}
