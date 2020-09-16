using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class DogParkDalmation : MonoBehaviour
{
    public Transform sphere, approachPoint, idlePoint;
    public Transform[] diggingSpots;
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
    public GameObject teddyBear;
    public int interactionStage;
    public GameObject levelLoader;
    public Transform groveLocation;
    public GameObject butterflies;
    public int numInteractions;
    public Transform [] bones;
    public GameObject barrier;

    [Header("Emotion Values")]
    public float bond;
    public float inquisitive;
    public float obedience;
    public float playfulness;

    private int objType;
    private bool digTeddy;
    private Vector3[] mouthPositions = { new Vector3(-0.118000001f, -0.0209999997f, 0.00499999989f),    //Bouncy Ball
                                         new Vector3(-0.0967f, 0.0038f, 0.0406f),                       //Tennis Ball
                                         new Vector3(-0.114299998f, 0.00510000018f, -0.0148999998f),    //Bone
                                         new Vector3(-0.101000004f, -0.145999998f, 0.00899999961f)      //Teddy                                   
                                        };
    private Vector3[] mouthRotations = { new Vector3 (0,0,0),                                       //Bouncy Ball
                                         new Vector3(1.14008284f, 182.066574f, 17.4123516f),        //Tennis Ball
                                         new Vector3(84.8392334f, 198.522034f, 289.72522f),         //Bone
                                         new Vector3(48.8043404f, 359.46994f, 269.295593f)};        //Teddy
    private bool strokeTouch;
    public string state;
    private bool play;
    private bool longExpl, ignoreThrow, findGrove;
    private bool disobedient;
    private bool groveFound;
    private bool deathStart;
    private bool deathMove;
    private bool gestureDetect;
    private string path = "Times.txt";
    private StreamWriter writer;
    private float boneTimer;
    private bool chanceBone;
    private bool deadStroke;
    private bool deathFade;
    private Transform closest;
    private bool pickupball;
    private string stateToSet;
    private bool setState;
    private Transform tDigSpot;

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
        digTeddy = false;
        strokeTouch = false;
        state = "idle";
        PlayerPrefs.SetInt("Item", 0);
        play = false;
        longExpl = false;
        ignoreThrow = false;
        findGrove = false;
        disobedient = false;
        groveFound = false;
        deathStart = false;
        numInteractions = 0 + PlayerPrefs.GetInt("NumInteractions");
        bond = 1 + PlayerPrefs.GetFloat("Bond");
        gestureDetect = false;
        approachPoint = GameObject.FindGameObjectWithTag("MainCamera").transform;
        idlePoint = approachPoint;
        boneTimer = Time.time;
        chanceBone = false;
        deadStroke = true;
        deathFade = false;
        closest = null;
        pickupball = false;
        stateToSet = "";
        setState = false;
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

    // Update is called once per frame
    void Update()
    {
        speed = agent.velocity.magnitude;
        
        //Run through State Machine
        if (Time.time - deathTime > dogLife && !deathStart)
        {
            state = "startDeath";
            StateDeathStart(true);
            deathStart = true;
        }
        if (state == "idle")
        {
            StateIdle();
        }
        else if(state == "chase")
        {
            StateChase();
        }
        else if (state == "fetch")
        {
            StateFetch();
        }
        else if (state == "explore")
        {
            StateExplore();
        }
        else if (state == "startDeath")
        {
            StateDeathStart();
        }
        else if (state == "showGrove")
        {
            StateShowGrove();
        }
        else if (state == "return")
        {
            StateReturn();
        }
        else if (state == "dig")
        {
            StateDig();
        }   
        else if (state == "dead")
        {
            StateDead();
        }

        //Check for AI Director State Changes
        if (setState)
        {
            state = stateToSet;
            setState = false;
            OutsideChangeState();
        }
        //make sure our animations sync with the dog's movement speed
        if (speed < 0.5) speed = 1.5f;
        anim.SetFloat("Speed", speed);
        //print(state);
    }

    //Called when an object is thrown 
    private void onObjectThrown(GameObject thrownObj)
    {
        targetObj = thrownObj;
        objType = thrownObj.GetComponent<ObjectThrown>().objType;
        sphere = targetObj.GetComponent<Transform>();
    }

    //Check when the dog is stroked
    void OnCollisionEnter(Collision other)
    {
        if (interactionStage == 2)
        {
            GameObject player = other.gameObject.transform.parent.gameObject;
            if (player.tag == "Player")
            {
                strokeTouch = true;
            }
        }
    }

    // = = = States = = = //
    private void StateIdle(bool first = false)
    {
        if (first)
        {
            animationTime = Time.time;
        }
        //Check if the player has used a gesture
        else if (gestureDetect)
        {
            //Return to Player if far away
            if (Vector3.Distance(transform.position, idlePoint.position) > 3)
            {
                WriteString("Gesture - Returned to Player: ");
                numInteractions += 1;
                bond += 0.05f;
                gestureDetect = false;

                state = "return";
                StateReturn(true);
            }
            //Dog sits down
            else
            {
                int chanceSit = Random.Range(0, 100);
                float totalSit = obedience * bond * 30;

                if (chanceSit <= totalSit)
                {
                    anim.SetInteger("State", 2);
                    animationTime = Time.time;

                    WriteString("Gesture - Sit: ");
                    numInteractions += 1;
                    bond += 0.05f;
                    gestureDetect = false;

                    obedience += 0.05f;
                }
                

            }
        }
        //Check for throw
        else if (CheckThrow())
        {
            state = "chase";
            StateChase(true);
        }
        //Switch to return to player from playing
        else if (play && Time.time - animationTime > 4)
        {
            state = "return";
            play = false;
            StateReturn(true);
        }
        //Stroke animation
        else if (strokeTouch)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Stroking"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    anim.SetInteger("State", 1);
                    animationTime = Time.time;
                    //Start Barking
                    StartCoroutine(BarkWithDelay(.8f, 0));
                    StartCoroutine(BarkWithDelay(2, 0));
                    StartCoroutine(BarkWithDelay(3, 0));

                }
                else
                {
                    anim.SetInteger("State", 6); //exit current animation loop
                }
            }
            else if (Time.time - animationTime > 3){
                anim.SetInteger("State", 6);
                animationTime = Time.time;
                strokeTouch = false;
                //Log Stroke
                WriteString("Dog Stroked: ");
                numInteractions += 1;
                bond += 0.05f;
                StopAllCoroutines();
            }
        }
        //Other Idle Animations
        else if (Time.time - animationTime > 6)
        {

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                //Chance to play around
                int chancePlay = Random.Range(0, 100);
                float totalEmot = inquisitive * 30 + playfulness * 25;
                float inq = 30 * inquisitive / totalEmot * Mathf.Clamp(totalEmot, 0, 80);
                float playful = 25 * playfulness / totalEmot * Mathf.Clamp(totalEmot, 0, 80);
                //chance to explore
                if (chancePlay <= inq)
                {
                    state = "explore";
                    StateExplore(true);
                }
                else if (chancePlay <= inq + playful)
                {
                    play = true;
                    anim.SetInteger("State", 5);
                    charController.SetTarget(null);
                    WriteString("AI - Play: ");
                }
                
                //Other Idle Animations
                else
                {
                    int nextState = Random.Range(0, 2);
                    //Sit Down
                    if (nextState == 0)
                    {

                        anim.SetInteger("State", 2);
                    }
                    //Scratch
                    else if (nextState == 1)
                    {
                        anim.SetInteger("State", 3);
                    }
                }
                animationTime = Time.time;
            }

            else
            {
                anim.SetInteger("State", 6); //exit current animation loop
                animationTime = Time.time;
            }

        }

    }

    private void StateChase(bool first = false)
    {
        //Only set Target when it is running to avoid skating
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Runing Fast")) charController.SetTarget(sphere);

        if (first)
        {
            anim.SetInteger("State", 0); //Transition to running fast
            
            sphere.GetComponent<ObjectThrown>().thrown = false;
            agent.speed = 3.3f;

            //Start Barking
            StartCoroutine(BarkWithDelay(.7f, 0));
            StartCoroutine(BarkWithDelay(2, 0));
            StartCoroutine(BarkWithDelay(2.8f, 0));
            StartCoroutine(BarkWithDelay(3.4f, 0));
            StartCoroutine(BarkWithDelay(4.8f, 0));
            StartCoroutine(BarkWithDelay(5.6f, 0));
            StartCoroutine(BarkWithDelay(6.2f, 0));
            StartCoroutine(BarkWithDelay(7.5f, 0));

            WriteString("Start Chase Ball: ");
            numInteractions += 1;
            bond += 0.05f;
        }
        //Check if the player has used a gesture
        else if (gestureDetect)
        {
            int chancePlay = Random.Range(0, 100);
            if (chancePlay <= ((playfulness - (obedience) * bond) * 30) + (5 * objType))
            {
                WriteString("AI: Gesture Ignored: ");
            }
            else
            {
                WriteString("AI: Gesture Obeyed: ");
                numInteractions += 1;
                bond += 0.05f;
                gestureDetect = false;

                state = "return";
                StateReturn(true);
            }
            
        }
        
        //Pick up the ball when the dog gets close enough
        else if (Vector3.Distance(sphere.position, transform.position) < 1 || pickupball)
        {

            pickupball = true;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Runing Fast"))
            {
                anim.SetInteger("State", 1); //Transition to running with ball
                anim.SetBool("DroppedBall", false);
            }
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Runing Fast"))
            {
                //Set the ball in the mouth
                sphere.SetParent(dogMouth.transform);
                sphere.localPosition = mouthPositions[1 + objType];
                sphere.localRotation = Quaternion.Euler(mouthRotations[1 + objType]);
                sphere.GetComponent<ObjectThrown>().thrown = false;
                sphere.GetComponent<Rigidbody>().isKinematic = true;
                sphere.GetComponent<Rigidbody>().useGravity = false;
            }
           if (anim.GetCurrentAnimatorStateInfo(0).IsName("RunningWithBall"))
            {
                //charController.SetTarget(approachPoint);
                agent.speed = 2.8f;

                pickupball = false;
                state = "fetch";
                StateFetch(true);
            }

            StopAllCoroutines();

        }

        
        //print(Vector3.Distance(sphere.position, transform.position));
    }

    private void StateDeathStart(bool first = false)
    {
        if (first)
        {
            //If the dog should run to its death start barking and record the event in our text doc
            StopAllCoroutines();
            WriteString("Start Running Away: ");
            audioSource.volume = 0.1f;
            audioSource.pitch = 1;
            for (int i = 0; i < 50; i++)
            {
                StartCoroutine(BarkWithDelay((.9f * i + Random.Range(0, 0.35f)), 0));
            }
            startDeathBark = true;

            //run to the first spot and then continue along laid out path
            charController.SetTarget(deathspots[0]);
            agent.speed = 4;
            anim.SetInteger("State", 0);
            Teleport.instance.reset = 5;
        }
        else if (Vector3.Distance(deathspots[0].position, transform.position) < 1)
        {
            charController.SetTarget(deathspots[1]);
        }
        else if (Vector3.Distance(deathspots[1].position, transform.position) < 1)
        {
            charController.SetTarget(deathspots[2]);
            barrier.SetActive(true);
        }
        //When the dog arrives at the final spot play the crash sounds and record te event in our text doc
        else if (Vector3.Distance(deathspots[2].position, transform.position) < 5 && timeOfDeath < 1)
        {
            state = "dead";
            StateDead(true);
        }
    }

    private void StateDead(bool first = false)
    {
        if (first)
        {
            timeOfDeath = Time.time;
            StopAllCoroutines();
            audioSource.volume = 0.4f;
            StartCoroutine(BarkWithDelay(0, 1));
            StartCoroutine(BarkWithDelay(1.7f, 2));
            WriteString("Crash: ");

            agent.speed = 0.5f;
            anim.SetInteger("State", -5);
            deathMove = false;
            charController.SetTarget(transform);
            

        }
        else if (interactionStage == 2)
        {
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 4 && !deathFade)
            {

                SteamVR_Fade.Start(Color.clear, 0);
                SteamVR_Fade.Start(Color.black, 1);
                deathFade = true;

            }
            if (!deathMove && timeOfDeath > 1 && Time.time - timeOfDeath > 5)
            {
                charController.SetTarget(null);
                transform.position = (new Vector3(8.19498634f, 50.1063004f, -134.298294f));
                GameObject.FindGameObjectWithTag("Player").transform.position = (new Vector3(8.19498634f, 50.1063004f, -135.298294f));
                SteamVR_Fade.Start(Color.clear, 1);
                deathMove = true;
                WriteString("By Body: ");
                deadStroke = false;
                GetComponent<NavMeshAgent>().enabled = false;
                
            }
            //load next scene
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 12 && !loadCheck)
            {
                if (strokeTouch && !deadStroke)
                {
                    WriteString("Injured & Stroked: ");
                    numInteractions += 1;
                    deadStroke = true;
                }
                Debug.Log("NextScene");
                levelLoader.SetActive(true);
                loadCheck = true;

                string path = "Times.txt";
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine("Final Bond Value: " + bond);
                writer.WriteLine("Num Interactions: " + numInteractions);
                writer.Close();

                //Wipe all objects: Ensure thrown objects don't appear in next scene
                GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
                foreach (GameObject obj in interactables)
                {
                    Destroy(obj);
                }

            }

        }

        else
        {
            //fade to black after death
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 4)
            {
                lighting.intensity = 2 / (1 + ((Time.time - (timeOfDeath + 4)) * .5f));
            }
            //load next scene
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 7 && !loadCheck)
            {
                levelLoader.SetActive(true);
                loadCheck = true;

                string path = "Times.txt";
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine("Crash: " + Time.time);
                writer.WriteLine("Final Bond Value: " + bond);
                writer.WriteLine("Num Interactions: " + numInteractions);
                writer.Close();

            }
        }
    }

    private void StateReturn(bool first = false)
    {
        //Only set Target when it is walking to avoid skating
        if (charController.target != approachPoint && anim.GetCurrentAnimatorStateInfo(0).IsName("Walking Smell")) charController.SetTarget(approachPoint);

        if (first) {
            agent.speed = 2f;
            anim.SetInteger("State", 6); //exit current animation loop
            animationTime = Time.time;

            //See if dog will be obedient or not
            int chancePlay = Random.Range(0, 100);
            if (chancePlay <= 25 - (obedience * 25))
            {
                disobedient = true;
            }
            else disobedient = false;
        }

        else if (Vector3.Distance(idlePoint.position, transform.position) < 2)
        {
            anim.SetInteger("State", -1); //transition to idle
            charController.SetTarget(this.transform);

            //next state
            state = "idle";
            StateIdle(true);
        }
        else if (disobedient && Time.time - animationTime > 3)
        {
            WriteString("AI: Disobey Return: ");
            state = "explore";
            StateExplore(true);
        }
    }

    private void StateFetch(bool first = false)
    {
        //Only set Target when it is walking to avoid skating
        if (charController.target != approachPoint && anim.GetCurrentAnimatorStateInfo(0).IsName("RunningWithBall")) charController.SetTarget(approachPoint);

        if (first)
        {
            int chancePlay = Random.Range(0, 100);
            if (chancePlay <= inquisitive * 25 - (obedience * 25))
            {
                disobedient = true;
            }
            else disobedient = false;
        }
        //Drop the ball when we reach the player
        if (Vector3.Distance(approachPoint.position, transform.position) < 2)
        {
            anim.SetInteger("State", -1); //transition to idle
            anim.SetBool("DroppedBall", true);
            charController.SetTarget(this.transform);
            
            //Drop
            sphere.SetParent(null);
            sphere.GetComponent<Rigidbody>().isKinematic = false;
            sphere.GetComponent<Rigidbody>().useGravity = true;
            animationTime = Time.time;

            //next state
            state = "idle";
            StateIdle(true);
        }
        //Dog is disobedient
        else if (disobedient && Vector3.Distance(approachPoint.position, transform.position) < 2.2f)
        {
            //Drop
            sphere.SetParent(null);
            sphere.GetComponent<Rigidbody>().isKinematic = false;
            sphere.GetComponent<Rigidbody>().useGravity = true;
            animationTime = Time.time;

            WriteString("AI: Disobey return: ");

            //Next State
            state = "explore";
            StateExplore(true);
        }
        //Slow to a walk when close to the user
        else if (Vector3.Distance(approachPoint.position, transform.position) < 4.5f)
        {
            anim.SetInteger("State", 6); //transition to walking smell
            digCheck = false;
            charController.SetTarget(approachPoint);
            agent.speed = 1.5f;
            startballfetchBark = false;
        }
        
    }

    private void StateDig (bool first = false)
    {
        if (first) {
            anim.SetInteger("State", 5); //transition to digging
            charController.SetTarget(null);
            animationTime = Time.time;
        }
        //Check if the player has used a gesture
        else if (gestureDetect)
        {
            WriteString("Gesture Obeyed: ");
            numInteractions += 1;
            bond += 0.05f;
            gestureDetect = false;

            state = "return";
            StateReturn(true);
        }
        else if (Time.time - animationTime > 4)
        {
            if (interactionStage > 0 && !teddyBear.activeSelf)
            {
                teddyBear.transform.position = transform.position;
                teddyBear.SetActive(true);
                sphere = teddyBear.transform;
                charController.SetTarget(sphere);
                anim.SetInteger("State", 0); //Transition to running fast
                //Chase the teddy, which really just fetches it
                state = "chase";
                if (interactionStage == 2) {
                    PlayerPrefs.SetInt("Item", 1);
                }

                WriteString("Fetch Teddy: ");
                numInteractions += 1;
                bond += 0.25f;
            }
            else
            {
                StateReturn(true);
                state = "return";
            }
        }
        
    }

    private void StateExplore(bool first = false)
    {
        if (first)
        {
            WriteString("AI: Start Exploring: ");

            int chancePlay = Random.Range(0, 100);
            if (chancePlay <= inquisitive * 30) longExpl = true;
            else longExpl = false;
            chancePlay = Random.Range(0, 100);
            if (chancePlay <= ((inquisitive - (playfulness + obedience) * bond) * 30) - (5 * objType))
            {
                ignoreThrow = true;
            }
            else ignoreThrow = false;
            chancePlay = Random.Range(0, 100);
            if (interactionStage == 2 && chancePlay <= 35 * bond && !groveFound) findGrove = true;
            else findGrove = false;

            anim.SetInteger("State", 4);
            chancePlay = Random.Range(0, diggingSpots.Length);
            tDigSpot =  diggingSpots[chancePlay];
            agent.speed = 2f;
            animationTime = Time.time;

            
        }

        //Only set Target when it is walking to avoid skating
        if (charController.target != tDigSpot && anim.GetCurrentAnimatorStateInfo(0).IsName("Walking Smell")) charController.SetTarget(tDigSpot);

        //check closest digspot
        Transform closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (Transform go in diggingSpots)
        {
            Vector3 diff = go.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        //Check for throw
        if (CheckThrow())
        {
            int chancePlay = Random.Range(0, 100);
            if (chancePlay <= ((inquisitive - (playfulness + obedience) * bond) * 30) - (5 * objType))
            {
                ignoreThrow = true;
                sphere.GetComponent<ObjectThrown>().thrown = false;
                WriteString("Throw Ignored: ");
            }
            else
            {
                state = "chase";
                StateChase(true);
            }

        }
        //Check if the player has used a gesture
        else if (gestureDetect)
        {
            int chancePlay = Random.Range(0, 100);
            if (chancePlay <= ((inquisitive - (obedience) * bond) * 30))
            {
                WriteString("Gesture Ignored: ");
                gestureDetect = false;
            }
            else
            {
                numInteractions += 1;
                bond += 0.05f;
                gestureDetect = false;
                WriteString("Gesture Obeyed: ");
                state = "return";
                StateReturn(true);
            }
        }
        //transition to digging
        else if (Vector3.Distance(closest.position, transform.position) < .8)
        {
            tDigSpot = closest;
            charController.SetTarget(closest);
            state = "dig";
            StateDig(true);
        }
        //Start showing grove
        else if (findGrove && Time.time - animationTime > 5)
        {
            WriteString("Start Show Grove: ");

            state = "showGrove";
            groveFound = true;
            StateShowGrove(true);
        }
        //Short Time Stop
        else if (!longExpl && Time.time - animationTime > 10)
        {
            state = "return";
            StateReturn(true);
        }
        //Long Time Stop
        else if (Time.time - animationTime > 18)
        {
            WriteString("AI: Longer Exloring: ");
            state = "return";
            StateReturn(true);
        }
        //Look at nearby Bone Toys
        else if (Time.time - animationTime > 3 && interactionStage > 0) SearchBone();
    }

    private void StateShowGrove(bool first = false)
    {
        //Only set Target when it is running to avoid skating
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Runing Fast") && Vector3.Distance(idlePoint.position, transform.position) < 11) charController.SetTarget(groveLocation);

        if (first)
        {
            anim.SetInteger("State", 0); //Transition to running fast
            //charController.SetTarget(groveLocation);
            agent.speed = 3.3f;
            butterflies.SetActive(true);

            //Start Barking
            StartCoroutine(BarkWithDelay(.7f, 0));
            StartCoroutine(BarkWithDelay(2, 0));
            StartCoroutine(BarkWithDelay(2.8f, 0));
            StartCoroutine(BarkWithDelay(3.4f, 0));
            StartCoroutine(BarkWithDelay(4.8f, 0));
            StartCoroutine(BarkWithDelay(5.6f, 0));
            StartCoroutine(BarkWithDelay(6.2f, 0));
            StartCoroutine(BarkWithDelay(7.5f, 0));
        }
        else if (Vector3.Distance(idlePoint.position, transform.position) > 11)
        {
            anim.SetInteger("State", -2);
            charController.SetTarget(transform);
            agent.speed = 1.25f;

        }
        else if (charController.target != groveLocation)
        {
            anim.SetInteger("State", 0); //Transition to running fast
            charController.SetTarget(groveLocation);
            agent.speed = 3.3f;

        }
        else if (Vector3.Distance(groveLocation.position, transform.position) < 2)
        {
            WriteString("Grove Found: ");
            numInteractions += 1;
            bond += 0.5f;
            anim.SetInteger("State", -1);
            state = "idle";
            StateIdle(true);
        }
    }

    //Check if the target object has actually been thrown
    private bool CheckThrow()
    {
        return (charController.target != sphere && sphere.GetComponent<ObjectThrown>().thrown);
    }

    //Check if the player has done a gesture
    public void DetectGesture()
    { 
        gestureDetect = true;
    }

    // Our text doc for recording times
    static void WriteString(string ident)
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(ident + Time.time);
        writer.Close();
    }

    //Look for nearby bones and run to them if they are close andDog is playful, etc
    private void SearchBone()
    {
        //Dont need to do it the entirety of it
        
        if (Time.time - boneTimer > 1.5f)
        {
            //check closest digspot
            
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (Transform go in bones)
            {
                Vector3 diff = go.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            boneTimer = Time.time;

            //See if dog will fetch it
            int chancePlay = Random.Range(0, 100);
            if (chancePlay <= ((playfulness * bond) * 45)) chanceBone = true;
            else chanceBone = false;
            

        }
        if (chanceBone && Vector3.Distance(closest.position, transform.position) < 2)
        {
            sphere = closest;
            state = "chase";
            WriteString("Bone Toy Found: ");
            numInteractions += 1;
            bond += 0.05f;
            StateChase(true);
        }
        boneTimer += Time.deltaTime;


    }

    //Return Current State
    public string GetState()
    {
        return state;
    }

    //Set State of the Dog (For The AI Director)
    public void SetState(string state)
    {
        stateToSet = state;
        setState = true;
    }

    //Set the state (Based on an outside change - From AI Director)
    public void OutsideChangeState()
    {
        if (state == "idle")
        {
            StateIdle(true);
        }
        else if (state == "chase")
        {
            StateChase(true);
        }
        else if (state == "fetch")
        {
            StateFetch(true);
        }
        else if (state == "explore")
        {
            StateExplore(true);
        }
        else if (state == "startDeath")
        {
            StateDeathStart(true);
        }
        else if (state == "showGrove")
        {
            StateShowGrove(true);
        }
        else if (state == "return")
        {
            StateReturn(true);
        }
        else if (state == "dig")
        {
            StateDig(true);
        }
        else if (state == "dead")
        {
            StateDead(true);
        }
    }

    //For use by the AI Director
    public bool GetGroveStarted()
    {
        return groveFound;
    }

    //For use by the AI Director
    public void SetGroveStarted(bool started = true)
    {
        groveFound = started;
    }
}
