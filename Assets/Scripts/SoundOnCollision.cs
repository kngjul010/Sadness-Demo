using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Trigger a Sound when an object is thrown against something
public class SoundOnCollision : MonoBehaviour
{

    public AudioClip clip;
    public AudioSource source;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Left Hand" && collision.gameObject.tag != "Right Hand")
        {
            //Get velocity to set volume
            if (collision.relativeVelocity.y > 0.1) {
                source.clip = clip;
                source.volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 15);
                source.Play();
            }
            
        }

    }
}
