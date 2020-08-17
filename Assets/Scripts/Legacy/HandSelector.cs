using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Simple script to let the user select their dominant hand for throwing in the park scene
public class HandSelector : MonoBehaviour
{
    int handedness;
    void Start()
    {
        handedness = 0;
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Pet Store", LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

    }

    public void left() { handedness = -1; }
    public void right() { handedness = 1; }
    public void confirm() {
        if (handedness != 0)
        {
            PlayerPrefs.SetInt("Hand", handedness);
            StartCoroutine(LoadYourAsyncScene());
        }
    }
}
