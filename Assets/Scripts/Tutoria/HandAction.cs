using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandAction : MonoBehaviour
{
    public Hand hand;
    public SteamVR_Action_Boolean leftSnap;
    public SteamVR_Action_Boolean rightSnap;
    public SteamVR_Action_Boolean grabAction;

    private bool grabbed;
    private bool lookLeft;
    private bool lookRight;

    // Start is called before the first frame update
    void Start()
    {
        grabbed = false;
        lookLeft = false;
        lookRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (getGrab())
        {
            print("Grab " + hand.handType);
        }

        if (getLeft())
        {
            print("Left " + hand.handType);
        }

        if (getRight()){
            print("Right " + hand.handType);
        }
    }

    public bool getGrab() 
    {
        return grabAction.GetStateDown(hand.handType);
    }

    public bool getLeft()
    {
        return leftSnap.GetStateDown(hand.handType);
    }

    public bool getRight()
    {
        return rightSnap.GetStateDown(hand.handType);
    }
}
