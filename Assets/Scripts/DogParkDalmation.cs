using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;

public class DogParkDalmation : MonoBehaviour
{
    public Transform sphere, approachPoint, idlePoint, diggingSpot;
    public GameObject targetObj;
    public Transform[] deathspots;
    public GameObject mouthBall;
    private Vector3 lastPosition;
    private float speed;
    private float barkTimer;
    float animationTime, deathTime, timeOfDeath;
    Animator anim;
    NavMeshAgent agent;
    AICharacterControl charController;
    bool deathCheck, carCheck, whineCheck, digCheck, loadCheck;
    public Light lighting;
    public float barkDelay, lastBark;
    public AudioClip[] barkAudio;
    AudioSource audioSource;
    bool startballfetchBark, startDeathBark;
    public Material[] dogSkin;
    public float dogLife;
    public GameObject dogMouth;

    private int objType;
    private Vector3[] mouthPositions = { new Vector3(-0.118000001f, -0.0209999997f, 0.00499999989f),    //Bouncy Ball
                                         new Vector3(-0.0967f, 0.0038f, 0.0406f),                       //Tennis Ball
                                         new Vector3(-0.114299998f, 0.00510000018f, -0.0148999998f),    //Bone
                                         new Vector3(-0.101000004f, -0.145999998f, 0.00899999961f)      //Teddy                                   
                                        };
    private Vector3[] mouthRotations = { new Vector3 (0,0,0),                                       //Bouncy Ball
                                         new Vector3(1.14008284f, 182.066574f, 17.4123516f),        //Tennis Ball
                                         new Vector3(84.8392334f, 198.522034f, 289.72522f),         //Bone
                                         new Vector3(48.8043404f, 359.46994f, 269.295593f)};        //Teddy

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        anim = GetComponent<Animator>();
        charController = GetComponent<AICharacterControl>();
        audioSource = GetComponent<AudioSource>();
        barkTimer = 0;
        deathCheck = false;
        carCheck = false;
        whineCheck = false;
        digCheck = false;
        loadCheck = false;
        mouthBall.SetActive(false);
        animationTime = Time.time;
        deathTime = Time.time;
        startballfetchBark = false;
        startDeathBark = false;
        timeOfDeath = 0;

    }
    //Used to play the dog bark sounds at specific times
    IEnumerator BarkWithDelay(float time, int clip) 
    {
        yield return new WaitForSeconds(time);
        if (clip == 2)
        {
            audioSource.volume = 0.06f;
            audioSource.spatialBlend = 0.98f;
        }
        audioSource.clip = barkAudio[clip];
        audioSource.Play();

    }

    //Used to load new scene Async (doesn't seem particularly effective)
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Vet", LoadSceneMode.Single);
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
    // Update is called once per frame
    void Update()
    {
        speed = agent.velocity.magnitude;
        //If the ball has been thrown and the dog is not already chasing it or running to his death start chasing the ball after a short delay
        if (charController.target != sphere && sphere.GetComponent<ObjectThrown>().thrown && Time.time - deathTime < dogLife)
        {
            anim.SetInteger("State",0); //Transition to running fast
            charController.SetTarget(sphere);
            agent.speed = 3.3f;
        }
        //Pick up the ball when the dog gets close enough
        else if (charController.target == sphere && Vector3.Distance(sphere.position, transform.position) < .8 && Time.time - deathTime < dogLife)
        {
            StopAllCoroutines();
            
            anim.SetInteger("State", 1); //Transition to running with ball
            anim.SetBool("DroppedBall", false);
            //mouthBall.SetActive(true);
            sphere.SetParent(dogMouth.transform);
            sphere.localPosition = mouthPositions[1 + objType];
            sphere.localRotation = Quaternion.Euler(mouthRotations[1 + objType]);
            charController.SetTarget(approachPoint);
            agent.speed = 2.8f;
            //sphere.gameObject.SetActive(false); 
            sphere.GetComponent<ObjectThrown>().thrown = false;
            sphere.GetComponent<Rigidbody>().isKinematic = true;
            sphere.GetComponent<Rigidbody>().useGravity = false;
        }
        //Slow to a walk when close to the user
        else if (charController.target == approachPoint && Vector3.Distance(approachPoint.position, transform.position) < .8 && Time.time - deathTime < dogLife)
        {
            anim.SetInteger("State", 6); //transition to walking smell
            digCheck = false;
            charController.SetTarget(idlePoint);           
            agent.speed = 1.5f;
            startballfetchBark = false;
        }
        //Drop the ball when we reach the player
        else if (charController.target == idlePoint && Vector3.Distance(idlePoint.position, transform.position) < 1.2f && Time.time - deathTime < dogLife)
        {
            anim.SetInteger("State", -1); //transition to idle
            anim.SetBool("DroppedBall", true);
            charController.SetTarget(null);
            //mouthBall.SetActive(false);
            //sphere.gameObject.SetActive(true);    
            sphere.SetParent(null);
            sphere.GetComponent<Rigidbody>().isKinematic = false;
            sphere.GetComponent<Rigidbody>().useGravity = true;
            animationTime = Time.time;
        }
        //If the dog isn't due to run off and die but has been idle for some time play a secondary animation
        else if (Time.time - deathTime < dogLife)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && Time.time - animationTime>6)
            {
                int nextState = Random.Range(0, 3);
                if (nextState == 0)
                {
                    anim.SetInteger("State", 2);
                }
                else if (nextState == 1)
                {
                    anim.SetInteger("State", 3);
                }
                else if (nextState == 2)
                {
                    anim.SetInteger("State", 4);
                    digCheck = true;
                }
                animationTime = Time.time;
            }

            //walk to digging spot
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking Smell") && digCheck)
            {
                charController.SetTarget(diggingSpot);
                agent.speed = 2f;
                digCheck = false;
            }
            //dig at digging spot
            else if (charController.target == diggingSpot && Vector3.Distance(diggingSpot.position, transform.position) < .8)
            {
                anim.SetInteger("State", 5); //transition to digging
                charController.SetTarget(null);
                animationTime = Time.time;
            }
            //return to user
            if (Time.time - animationTime > 8 && (anim.GetCurrentAnimatorStateInfo(0).IsName("Diging") || anim.GetCurrentAnimatorStateInfo(0).IsName("Scraching") || anim.GetCurrentAnimatorStateInfo(0).IsName("Enjoy Seating")))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Diging"))
                {
                    charController.SetTarget(approachPoint);
                    agent.speed = 1.5f;
                }
                anim.SetInteger("State", 6); //exit current animation loop
                animationTime = Time.time;
                
            }
        }
       

        // If the dog is running to fetch a ball start barking
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Runing Fast") && !startballfetchBark && Time.time - deathTime < dogLife)
        {
            StartCoroutine(BarkWithDelay(.7f, 0));
            StartCoroutine(BarkWithDelay(2, 0));
            StartCoroutine(BarkWithDelay(2.8f, 0));
            StartCoroutine(BarkWithDelay(3.4f, 0));
            StartCoroutine(BarkWithDelay(4.8f, 0));
            StartCoroutine(BarkWithDelay(5.6f, 0));
            StartCoroutine(BarkWithDelay(6.2f, 0));
            StartCoroutine(BarkWithDelay(7.5f, 0));
            startballfetchBark = true;
        }
        //If the dog should run to its death start barking and record the event in our text doc
        else if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Runing Fast") || anim.GetCurrentAnimatorStateInfo(0).IsName("RunningWithBall")) && Time.time - deathTime > dogLife && !startDeathBark)
        {

            //add truth check
            StopAllCoroutines();
            string path = "Times.txt";

            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Puppy Running away: " + Time.time);
            writer.Close();
            audioSource.volume = 0.1f;
            audioSource.pitch = 1;
            for (int i = 0; i < 50; i++)
            {
                StartCoroutine(BarkWithDelay((.9f*i+Random.Range(0,0.35f)), 0));
            }            
            startDeathBark = true;
        }

        //Behaviour once the dog should run to its death
        if (Time.time - deathTime > dogLife)
        {
            //run to the first spot and then continue along laid out path
            if (charController.target != deathspots[0] && charController.target != deathspots[1] && charController.target != deathspots[2])
            {
                charController.SetTarget(deathspots[0]);
                agent.speed = 4;
                anim.SetInteger("State", 0);
            }
            if (Vector3.Distance(deathspots[0].position, transform.position) < 1)
            {
                charController.SetTarget(deathspots[1]);
            }
            if (Vector3.Distance(deathspots[1].position, transform.position) < 1)
            {
                charController.SetTarget(deathspots[2]);
            }
            //When the dog arrives at the final spot play the crash sounds and record te event in our text doc
            if (Vector3.Distance(deathspots[2].position, transform.position) < 5 && timeOfDeath < 1)
            {
                timeOfDeath = Time.time;
                StopAllCoroutines();
                audioSource.volume = 0.4f;
                StartCoroutine(BarkWithDelay(0, 1));
                StartCoroutine(BarkWithDelay(1.7f, 2));
                string path = "Times.txt";

                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine("Crash: " + Time.time);
                writer.Close();
            }
            //fade to black after death
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 4)
            {
                lighting.intensity = 2 / (1 + ((Time.time - (timeOfDeath + 4)) * .5f));
            }
            //load next scene
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 7 && !loadCheck)
            {
                Debug.Log("NextScene");
                StartCoroutine(LoadYourAsyncScene());
                loadCheck = true;
            }
        }
        //make sure our animations sync with the dog's movement speed
        anim.SetFloat("Speed", speed);

    }

    private void onObjectThrown(GameObject thrownObj)
    {
        targetObj = thrownObj;
        objType = thrownObj.GetComponent<ObjectThrown>().objType;
        sphere = targetObj.GetComponent<Transform>();
    }
}
