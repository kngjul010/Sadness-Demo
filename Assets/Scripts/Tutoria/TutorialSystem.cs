using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class TutorialSystem : MonoBehaviour
{
    public int level;
    public TextMeshPro tutText;
    public Hand leftHand;
    public Hand rightHand;
    public SteamVR_Action_Boolean leftSnap;
    public SteamVR_Action_Boolean rightSnap;
    public SteamVR_Action_Boolean grab;
    public SteamVR_Action_Boolean teleport;
    public Target target;
    public GameObject teleObj;
    public GameObject targetObj;
    public GameObject handSelectObj;
    public HandAccept handAccept;
    public AudioSource audioSource;
    public AudioClip tutCompleteSound;

    private float timer;
    private int stage;
    private bool stagePart1, stagePart2, stagePart3;
    private bool[] snaps = { false, false };
    private bool grabbed;
    private Coroutine hintCoroutine;
    private Coroutine LCoroutine;
    private Coroutine RCoroutine;



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
            }
            else if (timer > 7)
            {
                stage = 1;
                stagePart1 = false;
                timer = 0;
                tutText.text = "Move Your Head to Look around";
            }
        }
        //Look Controls
        else if (stage == 1)
        {
            if (stagePart1 == false && timer > 4)
            {
                stagePart1 = true;
                tutText.text = "Click Your Right D-Pad to Flick to a Direction for Ease";
                ShowHint(rightHand,rightSnap,"Click Right Side to Snap Right", ref RCoroutine);
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
                tutText.text = "Use the Trigger to Pick up a Ball, Release it to Let Go";
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
            else if (stagePart2 && stagePart3 ==false)
            {
                tutText.text = "Throw a ball or cube at the Bricks";
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
                tutText.text = "Use the Left DPad to Teleport to a Location";
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
            stage = 5;
        }
        //TODO: Gesture Recognition
        else if (stage == 5)
        {
            stage = 6;
        }
        //Choose Hands
        else if (stage == 6)
        {
            if (stagePart1 == false && timer > 2)
            {
                tutText.text = "Go to the Right Side of the Room to Select Your Hand Style and End the Tutorial";
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
                    StartCoroutine(LoadYourAsyncScene());
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

}
