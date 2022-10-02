using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppedParticle : MonoBehaviour
{
    public Sprite chopSprite;
    public ParticleSystem choppedParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        choppedParticleSystem.textureSheetAnimation.AddSprite(chopSprite);
    }

    // Update is called once per frame
    void Update()
    {
        if (!choppedParticleSystem.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
