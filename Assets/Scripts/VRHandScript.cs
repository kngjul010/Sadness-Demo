using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRHandScript : MonoBehaviour
{
    public SteamVR_Input_Sources hand;//gives us a way to select what hand our script is attached to.
    public Animator anim;//refrence to our animator
    public SteamVR_Action_Boolean grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

    public SteamVR_Action_Boolean grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

    void Update()//update our animator values
    {
        if (grabPinchAction.GetStateDown(hand))
        {
            anim.SetBool("Trigger Pulled", true);
        }
        if (grabPinchAction.GetStateUp(hand))
        {
            anim.SetBool("Trigger Pulled", false);
        }
        if (grabGripAction.GetStateDown(hand))
        {
            anim.SetBool("Grip Pressed", true);
        }
        if (grabGripAction.GetStateUp(hand))
        {
            anim.SetBool("Grip Pressed", false);
        }
    }
}