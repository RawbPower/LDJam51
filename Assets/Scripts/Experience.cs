using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Experience : MonoBehaviour
{
    public Action ExperienceChanged;

    private int currentExperience;
    // Start is called before the first frame update
    void Start()
    {
        currentExperience = 0;
        ExperienceChanged?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseExp(int experience)
    {
        currentExperience += experience;
        ExperienceChanged?.Invoke();
    }

    public int GetExperience()
    {
        return currentExperience;
    }
}
