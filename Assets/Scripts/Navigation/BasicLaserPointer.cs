using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class BasicLaserPointer : MonoBehaviour
{

    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean teleportAction;

    //Laser Variables
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    //Telport Variables
    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    private bool shouldTeleport;

    //Fade
    private bool fade = false;
    private float alphaFadeValue = 1;
    public Texture blackTexture;

    // Start is called before the first frame update
    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
    }

    // Update is called once per frame
    void Update()
    {
         //Check if Teleport button is pressed
        if (teleportAction.GetState(handType))
        {
            RaycastHit hit;
            //Store point hit by raycast (for teleport reticule)
            if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);

                //Show teleport reticule
                reticle.SetActive(true);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                shouldTeleport = true;
            }
        }
        else
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        //Teleport
        if (teleportAction.GetStateUp(handType) && shouldTeleport)
        {
            fade = true;
            Teleport();
            fade = false;
        }
        if (fade)
            FadeStuff();
    }

    //Show laser between controller and point of raycast
    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x,
                                                laserTransform.localScale.y,
                                                hit.distance);
    }

    //teleport the player to the location of the reticule
    private void Teleport()
    {
        shouldTeleport = false;
        reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        cameraRigTransform.position = hitPoint + difference;
    }

    //Fade
    void FadeStuff()
    {
        alphaFadeValue = Mathf.Clamp01(alphaFadeValue - (Time.deltaTime / 0.5f));
    }
    void OnGUI()
    {
        GUI.color = new Color(0, 0, 0, alphaFadeValue); ;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.blackTexture);
    }
}
