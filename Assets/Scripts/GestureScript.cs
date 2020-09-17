using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AirSig;

//Manages the Gesture Recognition in the environment
public class GestureScript : MonoBehaviour
{
    
    public AirSigManager airsigManager;
    public bool tut;
    public TutorialSystem tutSystem;
    public DogParkDalmation dog;

    AirSigManager.OnDeveloperDefinedMatch developerDefined;

    // Start is called before the first frame update
    void Start()
    {
        // Configure AirSig by specifying target 
        developerDefined = new AirSigManager.OnDeveloperDefinedMatch(HandleOnDeveloperDefinedMatch);
        airsigManager.onDeveloperDefinedMatch += developerDefined;
        airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
        airsigManager.SetDeveloperDefinedTarget(new List<string> {"DOWN" });
        airsigManager.SetClassifier("SampleGestureProfile", "");
    }

    //Check the gesture matches the one shown in the tut scene
    void HandleOnDeveloperDefinedMatch(long gestureId, string gesture, float score)
    {
        if (gesture.Trim() == "DOWN" && score > 0.6f)
        {
            detectGestureMatch();
            print("gesture: " + gesture + "   score: " + score);
        }
    }
    
    //Send a message to the correct game object when the gesture succeeds
    public void detectGestureMatch()
    {
        if (tut)
        {
            tutSystem.DetectGesture();
        }
        else
        {
            dog.DetectGesture();
        }
    }
}
