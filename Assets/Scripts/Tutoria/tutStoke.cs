using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutStoke : MonoBehaviour
{
    public TutorialSystem tutSystem;

    void OnCollisionEnter(Collision other)
    {
        GameObject player = other.gameObject.transform.parent.gameObject;
        if (player.tag == "Player")
        {
            tutSystem.stroked = true;
        }
    }
}
