using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Simple script which switches which hand the user can throw with in the park scene based on their chosen preference in HandSelector
public class ThrowArm_selector : MonoBehaviour
{
    
    public GameObject leftStick, righStick, leftball, rightball;
    public DogParkDalmation dog;
    void Start()
    {
        if (PlayerPrefs.GetInt("Hand") == -1)
        {
            leftStick.SetActive(true);
            dog.sphere = leftball.transform;
            righStick.SetActive(false);
        }
        else
        {
            righStick.SetActive(true);
            dog.sphere = rightball.transform;
            leftStick.SetActive(false);
        }

    }
}
