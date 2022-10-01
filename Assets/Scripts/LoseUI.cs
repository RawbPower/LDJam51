using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseUI : MonoBehaviour
{
    // Start is called before the first frame update
    public void Awake()
    {
        FindObjectOfType<GameManager>().loseUI = gameObject;
        gameObject.SetActive(false);
    }
}
