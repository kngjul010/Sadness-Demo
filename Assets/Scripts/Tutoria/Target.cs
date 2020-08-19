using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    private bool hit;

    private static Target _instance;
    public static Target instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Target>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        hit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        hit = true;
    }

    public bool getHit()
    {
        return hit;
    }
}
