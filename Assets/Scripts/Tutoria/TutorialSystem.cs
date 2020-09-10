using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using System.IO;
using UnityEngine.UI;

public class TutorialSystem : MonoBehaviour
{
    public int level;
    public Text tutText;
    public Hand leftHand;
    public Hand rightHand;
    public SteamVR_Action_Boolean leftSnap;
    public SteamVR_Action_Boolean rightSnap;
    public SteamVR_Action_Boolean grab;
    public SteamVR_Action_Boolean teleport;
    public SteamVR_Action_Boolean gesture;
    public Target target;
    public GameObject teleObj;
    public GameObject targetObj;
    public GameObject handSelectObj;
    public HandAccept handAccept;
    public AudioSource audioSource;
    public AudioClip tutCompleteSound;
    public GameObject levelLoader;
    public bool stroked;
    public GameObject strokeSphere;
    public GameObject gestureObjs;
    

    [Header("Voice Lines")]
    public AudioSource vlSource;
    public AudioClip welcomeA, lookAroundA, snapA, grabA, throwA, teleportA, strokeA, gestureA, handsA;


    private float timer;
    private int stage;
    private bool stagePart1, stagePart2, stagePart3;
    private bool[] snaps = { false, false };
    private bool grabbed;
    private Coroutine hintCoroutine;
    private Coroutine LCoroutine;
    private Coroutine RCoroutine;
    private bool gestureDone;



    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        stage = 0;
        tutText.text = "";
        stagePart1 = false;
        stagePart2 = false;
        stagePart3 = false;
        Teleport.instance.CancelTeleportHint();
        grabbed = false;
        teleObj.SetActive(false);
        level = PlayerPrefs.GetInt("Level");
        leftHand =  GameObject.FindWithTag("Left Hand").GetComponent<Hand>();
        rightHand = GameObject.FindWithTag("Right Hand").GetComponent<Hand>();
        gestureDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Intro
        if (stage == 0)
        {
            if (stagePart1 == false && timer > 2)
            {
                stagePart1 = true;
                tutText.text = "Welcome To The VREmote Tutorial";
                PlayAudioClip(vlSource, welcomeA);
                WriteString("Tutorial Start: ");
            }
            else if (timer > 7)
            {
                stage = 1;
                stagePart1 = false;
                timer = 0;
                tutText.text = "Move Your Head to Look around";
                PlayAudioClip(vlSource, lookAroundA);
            }
        }
        //Look Controls
        else if (stage == 1)
        {
            if (stagePart1 == false && timer > 4)
            {
                stagePart1 = true;
                tutText.text = "Click The Side of Your Right D-Pad to Snap to that Direction";
                PlayAudioClip(vlSource, snapA);
                ShowHint(rightHand, rightSnap, "Click Right Side to Snap Right", ref RCoroutine);
                ShowHint(rightHand, leftSnap, "Click Left Side to Snap Left", ref LCoroutine);
                snaps[0] = false;
                snaps[1] = false;
            }
            else if (stagePart1)
            {
                if (leftSnap.GetStateDown(rightHand.handType))
                {
                    CancelHint(rightHand, leftSnap, ref LCoroutine);
                    snaps[0] = true;
                }
                if (rightSnap.GetStateDown(rightHand.handType))
                {
                    CancelHint(rightHand, rightSnap, ref RCoroutine);
                    snaps[1] = true;
                }
                if (snaps[0] == true && snaps[1] == true)
                {
                    PlayAudioClip(audioSource, tutCompleteSound);
                    stage = 2;
                    timer = 0;
                    stagePart1 = false;
                }
            }
        }
        //Pick up and throw the Ball/Objects
        else if (stage == 2)
        {
            if (stagePart1 == false && timer > 0.5f)
            {
                tutText.text = "Use the Trigger to Pick up a Ball and Release it to Let the Ball Go";
                PlayAudioClip(vlSource, grabA);
                ShowHint(rightHand, grab, "Hold Trigger to Grab", ref RCoroutine);
                ShowHint(leftHand, grab, "Hold Trigger to Grab", ref LCoroutine);
                stagePart1 = true;
            }
            else if (stagePart2 == false && stagePart1 == true)
            {
                if (grab.GetStateDown(leftHand.handType) || grab.GetStateDown(rightHand.handType))
                {
                    CancelHint(rightHand, grab, ref RCoroutine);
                    CancelHint(leftHand, grab, ref LCoroutine);
                    stagePart2 = true;
                    timer = 0;
                }
            }
            else if (stagePart2 && stagePart3 == false)
            {
                tutText.text = "Throw a Ball or Cube at the Bricks in Front of You";
                PlayAudioClip(vlSource, throwA);
                stagePart3 = true;
                targetObj.SetActive(true);
            }
            else if (stagePart3)
            {

                if (Target.instance.getHit() == true)
                {
                    stage = 3;
                    timer = 0;
                    stagePart1 = false;
                    stagePart2 = false;
                    stagePart3 = false;
                    targetObj.SetActive(false);
                    PlayAudioClip(audioSource, tutCompleteSound);
                }
            }
        }
        //Teleport
        else if (stage == 3)
        {
            if (level == 0)
            {
                stage = 6;
                return;
            }
            if (stagePart1 == false && timer > 2)
            {
                teleObj.SetActive(true);
                tutText.text = "Hold the Left DPad Aim and Release it to Teleport to a Location - 3s Delay between Teleports";
                PlayAudioClip(vlSource, teleportA);
                ShowHint(leftHand, teleport, "Click and Hold to Aim, Release to Teleport", ref hintCoroutine);

                stagePart1 = true;
            }
            else if (stagePart1 == true)
            {
                if (teleport.GetStateUp(leftHand.handType))
                {
                    CancelHint(leftHand, teleport, ref hintCoroutine);
                    timer = 0;
                    stagePart1 = false;
                    stage = 4;
                    PlayAudioClip(audioSource, tutCompleteSound);
                }
            }
        }
        //TODO: Stroke Sphere
        else if (stage == 4)
        {
            if (level == 1)
            {
                stage = 6;
                return;
            }
            if (stagePart1 == false && timer > 2)
            {
                strokeSphere.SetActive(true);
                tutText.text = "Move Your Hand Along the Surface of the Green Sphere to Stroke It";
                PlayAudioClip(vlSource, strokeA);
                stagePart1 = true;
                stroked = false;
            }
            else if (stagePart1 == true && stagePart2 == false)
            {
                if (stroked)
                {
                    stagePart2 = true;
                    timer = 0;
                }

            }
            else if (stagePart1 && stagePart2 && timer > 2)
            {
                stage = 5;
                stagePart1 = false;
                stagePart2 = false;
                timer = 0;
                PlayAudioClip(audioSource, tutCompleteSound);
            }

        }
        //TODO: Gesture Recognition
        else if (stage == 5)
        {
            if (stagePart1 == false && timer > 2)
            {
                gestureObjs.SetActive(true);
                strokeSphere.transform.position = new Vector3(-3.01300001f, 0.5f, 2.86999989f);
                tutText.text = "Above Your Head - Hold the Grip Button to Start and Release to Complete the Gesture Indicated";
                PlayAudioClip(vlSource, gestureA);
                stagePart1 = true;
                stroked = false;
                ShowHint(rightHand, gesture, "Hold to Start, Release to Complete Gesture", ref hintCoroutine);
                gestureDone = false;
                stagePart1 = true;
            }
            else if (stagePart1 && !stagePart2)
            {
                if (gestureDone)
                {
                    CancelHint(rightHand, gesture, ref hintCoroutine);
                    strokeSphere.GetComponent<CapsuleCollider>().enabled = true;
                    strokeSphere.GetComponent<Animator>().enabled = true;
                    strokeSphere.GetComponent<ThirdPersonCharacter>().enabled = true;
                    strokeSphere.GetComponent<AICharacterControl>().enabled = true;
                    strokeSphere.GetComponent<NavMeshAgent>().enabled = true;

                    stagePart1 = false;
                    stagePart2 = false;
                    timer = 0;
                    PlayAudioClip(audioSource, tutCompleteSound);
                    stage = 6;
                }
            }
            
        }
        //Choose Hands
        else if (stage == 6)
        {
            if (stagePart1 == false && timer > 2)
            {
                tutText.text = "Go to the Right Side of the Room to Select Your Hand Style and End the Tutorial";
                PlayAudioClip(vlSource, handsA);
                if (level == 0)
                {
                    tutText.text = "Select Your Hand Style and End the Tutorial";
                    handSelectObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    handSelectObj.transform.localPosition = new Vector3(-0.886f, -0.6366583f, -3.217f);
                }
                stagePart1 = true;
                handSelectObj.SetActive(true);
            }
            else if (stagePart1 && stagePart2 == false)
            {
                if (handAccept.getConfirm())
                {
                    PlayAudioClip(audioSource, tutCompleteSound);
                    tutText.text = "The Experience Will Begin Shortly";
                    stagePart2 = true;
                    PlayerPrefs.SetInt("Level", level);
                    levelLoader.SetActive(true);
                    WriteString("Tutorial End: ");
                    //SteamVR_Fade.Start(Color.clear, 0);
                    // SteamVR_Fade.Start(Color.black, 1);
                    //StartCoroutine(LoadYourAsyncScene());
                }
            }
        }

        timer += Time.deltaTime;
    }

    public void ShowHint(Hand hand, SteamVR_Action_Boolean action, string msg, ref Coroutine cor)
    {
        CancelHint(hand, action, ref cor);
        cor = StartCoroutine(HintCoroutine(hand,action,msg));
    }

    public void CancelHint(Hand hand, SteamVR_Action_Boolean action, ref Coroutine cor)
    {
        if (cor != null)
        {
            ControllerButtonHints.HideTextHint(hand, action);

            StopCoroutine(cor);
            cor = null;
        }
    }

    private IEnumerator HintCoroutine(Hand hand, SteamVR_Action_Boolean action, string msg)
    {
        float prevBreakTime = Time.time;
        float prevHapticPulseTime = Time.time;

        while (true)
        {
            bool pulsed = false;
            bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, action));

            if (!isShowingHint)
            {
                ControllerButtonHints.ShowTextHint(hand, action, msg);
                ControllerButtonHints.ShowButtonHint(hand, action);
                prevBreakTime = Time.time;
                prevHapticPulseTime = Time.time;
            }

            if (Time.time > prevHapticPulseTime + 0.05f)
            {
                //Haptic pulse for a few seconds
                pulsed = true;

                hand.TriggerHapticPulse(500);
            }


            if (Time.time > prevBreakTime + 3.0f)
            {
                //Take a break for a few seconds
                yield return new WaitForSeconds(3.0f);

                prevBreakTime = Time.time;
            }

            if (pulsed)
            {
                prevHapticPulseTime = Time.time;
            }

            yield return null;
        }
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

    private void PlayAudioClip(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void DetectGesture()
    {
        gestureDone = true;
    }

    // Our text doc for recording times
    static void WriteString(string ident)
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(ident + Time.time);
        writer.Close();
    }

}
