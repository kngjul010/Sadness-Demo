using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Rotation : MonoBehaviour
{
    public SteamVR_Action_Boolean SnapLeft;
    public SteamVR_Action_Boolean SnapRight;
    public GameObject CameraBox;
    public GameObject target;

    // Update is called once per frame
    void Update()
    {
        if (SnapLeft.lastStateDown == true)
        {
            CameraBox.transform.RotateAround(target.transform.position, new Vector3(0, 1, 0),-90);

        }
        else if (SnapRight.lastStateDown == true)
        {
            CameraBox.transform.RotateAround(target.transform.position, new Vector3(0, 1, 0), 90);

        }
    }
}
