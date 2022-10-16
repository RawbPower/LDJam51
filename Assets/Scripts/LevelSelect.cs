using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    private UILevelSelectButton[] levelButtons;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        levelButtons = FindObjectsOfType<UILevelSelectButton>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        foreach (UILevelSelectButton button in levelButtons)
        {
            if (button.level <= gameManager.levelsCompleted + 1)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (UILevelSelectButton button in levelButtons)
        {
            if (button.level <= gameManager.levelsCompleted + 1)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }
}
