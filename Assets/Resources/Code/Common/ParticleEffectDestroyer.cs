using UnityEngine;
using System.Collections;
using System;

public class ParticleEffectDestroyer : MonoBehaviour
{

    private void Start()
    {

        ParticleSystem particle = gameObject.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            Destroy(gameObject, GetComponent<ParticleSystem>().duration);
        }

    }

}