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

        //make sure our animations sync with the dog's movement speed
        anim.SetFloat("Speed", speed);

    }

    private void onObjectThrown(GameObject thrownObj)
    {
        targetObj = thrownObj;
        objType = thrownObj.GetComponent<ObjectThrown>().objType;
        sphere = targetObj.GetComponent<Transform>();
    }

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

    private void StateIdle(bool first = false)
    {
        if (first)
        {
            animationTime = Time.time;
        }
        //Check if the player has used a gesture
        else if (gestureDetect)
        {
            WriteString("AI: Gesture Obeyed: ");
            numInteractions += 1;
            bond += 0.05f;
            gestureDetect = false;

            state = "return";
            StateReturn(true);
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
                WriteString("Dog Stroked");
                numInteractions += 1;
                bond += 0.05f;
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
        if (first)
        {
            anim.SetInteger("State", 0); //Transition to running fast
            charController.SetTarget(sphere);
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
        else if (Vector3.Distance(sphere.position, transform.position) < .8)
        {
            StopAllCoroutines();

            anim.SetInteger("State", 1); //Transition to running with ball
            anim.SetBool("DroppedBall", false);
            //Set the ball in the mouth
            sphere.SetParent(dogMouth.transform);
            sphere.localPosition = mouthPositions[1 + objType];
            sphere.localRotation = Quaternion.Euler(mouthRotations[1 + objType]);
            charController.SetTarget(approachPoint);
            agent.speed = 2.8f;
  
            sphere.GetComponent<ObjectThrown>().thrown = false;
            sphere.GetComponent<Rigidbody>().isKinematic = true;
            sphere.GetComponent<Rigidbody>().useGravity = false;

            state = "fetch";
            StateFetch(true);
        }
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
        }
        else if (Vector3.Distance(deathspots[0].position, transform.position) < 1)
        {
            charController.SetTarget(deathspots[1]);
        }
        else if (Vector3.Distance(deathspots[1].position, transform.position) < 1)
        {
            charController.SetTarget(deathspots[2]);
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
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 4 && Time.time - timeOfDeath < 5)
            {

                SteamVR_Fade.Start(Color.clear, 0);
                SteamVR_Fade.Start(Color.black, 1);
                //lighting.intensity = 2 / (1 + ((Time.time - (timeOfDeath + 4)) * .5f));

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
            if (timeOfDeath > 1 && Time.time - timeOfDeath > 10 && !loadCheck)
            {
                if (strokeTouch && !deadStroke)
                {
                    WriteString("Stroked: ");
                    numInteractions += 1;
                    deadStroke = true;
                }
                Debug.Log("NextScene");
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
                Debug.Log("NextScene");
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
        if (first) {
            charController.SetTarget(approachPoint);
            agent.speed = 1.5f;
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
        if (Vector3.Distance(idlePoint.position, transform.position) < 2)
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
        else if (disobedient && Vector3.Distance(approachPoint.position, transform.position) < 4.5f)
        {
            //Drop
            sphere.SetParent(null);
            sphere.GetComponent<Rigidbody>().isKinematic = false;
            sphere.GetComponent<Rigidbody>().useGravity = true;
            animationTime = Time.time;
            WriteString("AI: Disobey return: ");
            state = "explore";
            StateExplore(true);
        }
        //Slow to a walk when close to the user
        else if (Vector3.Distance(approachPoint.position, transform.position) < 4.5f)
        {
            anim.SetInteger("State", 6); //transition to walking smell
            digCheck = false;
            charController.SetTarget(idlePoint);
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
            charController.SetTarget(diggingSpots[chancePlay]);
            agent.speed = 2f;
            animationTime = Time.time;

            
        }

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
        else if (Time.time - animationTime > 3) SearchBone();
    }

    private void StateShowGrove(bool first = false)
    {
        if (first)
        {
            anim.SetInteger("State", 0); //Transition to running fast
            charController.SetTarget(groveLocation);
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
            anim.SetInteger("State", -1);
            charController.SetTarget(transform);
            agent.speed = 1f;

        }
        else if (charController.target != groveLocation)
        {
            anim.SetInteger("State", 0); //Transition to running fast
            charController.SetTarget(groveLocation);
            agent.speed = 3.3f;

        }
        else if (Vector3.Distance(groveLocation.position, transform.position) < 2)
        {
            WriteString("Grove Found");
            numInteractions += 1;
            bond += 0.5f;
            anim.SetInteger("State", -1);
            state = "idle";
            StateIdle(true);
        }
    }

    private bool CheckThrow()
    {
        return (charController.target != sphere && sphere.GetComponent<ObjectThrown>().thrown);
    }

    public void DetectGesture()
    {
        if (Vector3.Distance(idlePoint.position, transform.position) > 4)
        {
            gestureDetect = true;
        }
    }

    // Our text doc for recording times
    static void WriteString(string ident)
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(ident + Time.time);
        writer.Close();
    }

    private void SearchBone()
    {
        //Dont need to do it the entirety of it
        Transform closest = null;
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
       


    }
}

//Backup Legacy Code
/*
 * //If the ball has been thrown and the dog is not already chasing it or running to his death start chasing the ball after a short delay
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
        //Drop the ball when we reach the player
        else if (charController.target == idlePoint && Vector3.Distance(idlePoint.position, transform.position) < 2 && Time.time - deathTime < dogLife)
        {
            anim.SetInteger("State", -1); //transition to idle
            anim.SetBool("DroppedBall", true);
            charController.SetTarget(this.transform);
            //mouthBall.SetActive(false);
            //sphere.gameObject.SetActive(true);    
            sphere.SetParent(null);
            sphere.GetComponent<Rigidbody>().isKinematic = false;
            sphere.GetComponent<Rigidbody>().useGravity = true;
            animationTime = Time.time;
        }
        //Slow to a walk when close to the user
        else if (charController.target == approachPoint && Vector3.Distance(approachPoint.position, transform.position) < 4.5f && Time.time - deathTime < dogLife)
        {
            anim.SetInteger("State", 6); //transition to walking smell
            digCheck = false;
            charController.SetTarget(idlePoint);           
            agent.speed = 1.5f;
            startballfetchBark = false;
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
                charController.SetTarget(diggingSpots[0]);
                agent.speed = 2f;
                digCheck = false;
            }
            //dig at digging spot
            else if (charController.target == diggingSpots[0] && Vector3.Distance(diggingSpots[0].position, transform.position) < .8)
            {
                anim.SetInteger("State", 5); //transition to digging
                charController.SetTarget(null);
                animationTime = Time.time;
                if (interactionStage > 0)
                {
                    digTeddy = true;
                }
            }
            //return to user
            if (Time.time - animationTime > 8 && (anim.GetCurrentAnimatorStateInfo(0).IsName("Diging") || anim.GetCurrentAnimatorStateInfo(0).IsName("Scraching") || anim.GetCurrentAnimatorStateInfo(0).IsName("Enjoy Seating")))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Diging"))
                {
                    charController.SetTarget(approachPoint);
                    agent.speed = 1.5f;
                    if (digTeddy)
                    {
                        teddyBear.SetActive(true);
                        sphere = teddyBear.transform;
                        charController.SetTarget(sphere);
                    }
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
                levelLoader.SetActive(true);
                //StartCoroutine(LoadYourAsyncScene());
                loadCheck = true;
            }
        }
 * 
 */
