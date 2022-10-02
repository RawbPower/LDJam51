using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public ChoppedParticle[] choppedParticles;

    private Animator animator;
    private bool chopped;

    private void Start()
    {
        animator = GetComponent<Animator>();
        chopped = false;
    }

    public void OnChopDecoration()
    {
        if (!chopped)
        {
            foreach (ChoppedParticle particle in choppedParticles)
            {
                Instantiate(particle.gameObject, transform.position, Quaternion.identity);
            }

            animator.SetBool("Chop", true);
            chopped = true;
        }
    }
}
