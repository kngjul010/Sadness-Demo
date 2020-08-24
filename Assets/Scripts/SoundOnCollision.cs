using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{

    public AudioClip clip;
    public AudioSource source;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Left Hand" && collision.gameObject.tag != "Right Hand")
        {
            source.clip = clip;
            source.volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10);
            source.Play();
        }

    }
}
