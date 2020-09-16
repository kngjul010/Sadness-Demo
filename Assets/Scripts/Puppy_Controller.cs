using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;
using System.IO;
using UnityEditor;
using Valve.VR;
//Controls the puppy behaviour in the pet stor (PSA I didn't name the animations and never got around to renaming them :| )
public class Puppy_Controller : MonoBehaviour
{

    float speed;
    float timer, timeDelay;
    float endScene;
    public AudioClip[] yapping;
    AudioSource source;
    public Transform puppySpot;
    public Light lighting;
    bool startLyingBark, startStandBark, startfightIdle, startStandingBark, loadCheck;
    public GameObject door;
    public int dogChosen;
    public GameObject loadObject;
    public bool chosen;
    public int interactionStage;

    private float animationTime;
    private Animator anim;
    private bool strokeTouch;
    private int numInteractions;
    private float bond;

    //Set initial values
    void Start()
    {
        timer = Time.time;
        timeDelay = Random.Range(3.0f, 5);
        source = GetComponent<AudioSource>();
        endScene = 10000.0f;
        speed = 0;
        startLyingBark = false;
        startStandBark = false;
        startfightIdle = false;
        startStandingBark = false;
        loadCheck = false;
        chosen = false;
        strokeTouch = false;
        puppySpot = GameObject.FindGameObjectWithTag("MainCamera").transform;
        anim = GetComponent<Animator>();
        numInteractions = 0;
        bond = 0;
        
    }
    // Used to play puppy barking noises
    IEnumerator BarkWithDelay(float time, float length, int clip)
    {
        yield return new WaitForSeconds(time);
        source.clip = yapping[clip];
        source.time = length;
        source.Play();
    }
    // loads next scene
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Park", LoadSceneMode.Single);
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
    // Our text doc for recording times
    public static void WriteString(string ident)
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(ident + Time.time);
        writer.Close();
    }

    void Update()
    {
         //Stroke animation
        if (strokeTouch && interactionStage == 2 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Stroking"))
            {

                anim.SetInteger("Next", -1);
                animationTime = Time.time;
                

            }
            else if (Time.time - animationTime > 3)
            {
                anim.SetInteger("Next", 0);
                animationTime = Time.time;
                strokeTouch = false;
                //Log Stroke
                WriteString("Puppy Stroked: ");
                numInteractions += 1;
                bond += 0.05f;
            }
        }
        // State machine to govern a set of idle behaviours
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enjoy Stunding") && !startStandBark)
        {
            StartCoroutine(BarkWithDelay(.85f, 23.1f, 0));
            StartCoroutine(BarkWithDelay(1.0f, 23.1f, 0));
            StartCoroutine(BarkWithDelay(2.10f, 23.1f, 0));
            StartCoroutine(BarkWithDelay(2.50f, 23.1f, 0));
            startStandBark = true;
        }
        
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enjoy Stunding") && startStandBark)
        {
            startStandBark = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enjoy Lieing") && !startLyingBark)
        {
            StartCoroutine(BarkWithDelay(.2f, 1.9f, 0));
            StartCoroutine(BarkWithDelay(3.9f, 1.9f, 0));
            startLyingBark = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("End Rest") && startLyingBark)
        {
            source.Stop();
            startLyingBark = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fight Idle") && !startfightIdle)
        {
            StartCoroutine(BarkWithDelay(.2f, 15.9f, 0));
            startfightIdle = true;
        }
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Fight Idle") && startfightIdle)
        {
            source.Stop();
            startfightIdle = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Barking Stunding") && !startStandingBark)
        {
            StartCoroutine(BarkWithDelay(.2f, 1.9f, 0));
            StartCoroutine(BarkWithDelay(3.9f, 1.9f, 0));
            startStandingBark = true;
        }
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Barking Stunding") && startStandingBark)
        {
            source.Stop();
            startStandingBark = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") && GetComponent<AICharacterControl>().target == null)
        {
            GetComponent<AICharacterControl>().SetTarget(GameObject.FindGameObjectWithTag("MainCamera").transform);
        }
        //set animation speed if we have a character controller
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") && GetComponent<NavMeshAgent>() != null && GetComponent<AICharacterControl>() != null)
        {
            speed = GetComponent<NavMeshAgent>().velocity.magnitude;
            anim.SetFloat("Speed", speed);
        }
        //delay between animations
        if (Time.time - timer > timeDelay)
        {
            if (anim.GetInteger("Next") != 5)
            {
                anim.SetInteger("Next", (int)Random.Range(0.0f, 4.99f));
            }
            timeDelay = Random.Range(2, 5);
            timer = Time.time;

        }
        //end the scene once the pup gets close to the user
        if (endScene > 9999.0f && Vector3.Distance(transform.position, new Vector3(puppySpot.position.x,0, puppySpot.position.z)) < .7f && chosen)
        {
            endScene = Time.time;
            GetComponent<AICharacterControl>().SetTarget(null);
            anim.SetInteger("Next", -5);
        }
        //load next scene
        if (endScene < 9999.0f && !loadCheck)
        {
            SteamVR_Fade.Start(Color.clear, 0);
            SteamVR_Fade.Start(Color.black, 1);
            PlayerPrefs.SetInt("Dog", dogChosen);
            PlayerPrefs.SetInt("NumInteractions", numInteractions);
            PlayerPrefs.SetFloat("Bond", bond);
            loadObject.SetActive(true);
            //StartCoroutine(LoadYourAsyncScene());
            loadCheck = true;

            //Wipe all objects: Ensure thrown objects don't appear in next scene
            GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
            foreach (GameObject obj in interactables)
            {
                Destroy(obj);
            }
        }
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
}
