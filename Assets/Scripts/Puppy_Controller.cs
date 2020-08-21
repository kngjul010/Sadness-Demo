using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;
using System.IO;
using UnityEditor;
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
        WriteString("Pet Store start: ");
        chosen = false;
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
    static void WriteString(string ident)
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(ident + Time.time);
        writer.Close();
    }

    void Update()
    {
        // State machine to govern a set of idle behaviours
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Enjoy Stunding") && !startStandBark)
        {
            StartCoroutine(BarkWithDelay(.85f, 23.1f, 0));
            StartCoroutine(BarkWithDelay(1.0f, 23.1f, 0));
            StartCoroutine(BarkWithDelay(2.10f, 23.1f, 0));
            StartCoroutine(BarkWithDelay(2.50f, 23.1f, 0));
            startStandBark = true;
        }
        
        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Enjoy Stunding") && startStandBark)
        {
            startStandBark = false;
        }
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Enjoy Lieing") && !startLyingBark)
        {
            StartCoroutine(BarkWithDelay(.2f, 1.9f, 0));
            StartCoroutine(BarkWithDelay(3.9f, 1.9f, 0));
            startLyingBark = true;
        }
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End Rest") && startLyingBark)
        {
            source.Stop();
            startLyingBark = false;
        }
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Fight Idle") && !startfightIdle)
        {
            StartCoroutine(BarkWithDelay(.2f, 15.9f, 0));
            startfightIdle = true;
        }
        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Fight Idle") && startfightIdle)
        {
            source.Stop();
            startfightIdle = false;
        }
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Barking Stunding") && !startStandingBark)
        {
            StartCoroutine(BarkWithDelay(.2f, 1.9f, 0));
            StartCoroutine(BarkWithDelay(3.9f, 1.9f, 0));
            startStandingBark = true;
        }
        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Barking Stunding") && startStandingBark)
        {
            source.Stop();
            startStandingBark = false;
        }

        //set animation speed if we have a character controller
        if (GetComponent<NavMeshAgent>() != null && GetComponent<AICharacterControl>() != null)
        {
            speed = GetComponent<NavMeshAgent>().velocity.magnitude;
            GetComponent<Animator>().SetFloat("Speed", speed);
        }
        //delay between animations
        if (Time.time - timer > timeDelay)
        {
            if (GetComponent<Animator>().GetInteger("Next") != 5)
            {
                GetComponent<Animator>().SetInteger("Next", (int)Random.Range(0.0f, 4.99f));
            }
            timeDelay = Random.Range(2, 5);
            timer = Time.time;

        }
        //end the scene once the pup gets close to the user
        if (endScene > 9999.0f && Vector3.Distance(transform.position, new Vector3(puppySpot.position.x,0, puppySpot.position.z)) < .7f && chosen)
        {
            endScene = Time.time;
            GetComponent<AICharacterControl>().SetTarget(null);
        }
        //fade to blaclk
        if (endScene < 9999.0f)
        {
            lighting.intensity = 1 / (1 + Time.time - endScene);
        }
        //load next scene
        if (Time.time - endScene > 2.0f && !loadCheck)
        {
            PlayerPrefs.SetInt("Dog", dogChosen);
            loadObject.SetActive(true);
            //StartCoroutine(LoadYourAsyncScene());
            loadCheck = true;               
        }
    }
}
