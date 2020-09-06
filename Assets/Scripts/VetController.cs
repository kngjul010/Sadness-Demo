using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using Valve.VR.InteractionSystem;
public class VetController : MonoBehaviour
{

    float speed;
    bool start, talk;
    public Transform[] waypoints;
    public GameObject ball;
    public Light[] lights;
    float delay;

    private int level;
    public GameObject camObj;
    public GameObject teleportObj;
    private bool endScene;
    private float timer;

    // Initialise variables
    void Start()
    {
        speed = 10;
        start = true;
        talk = false;
        delay = Time.time;
        endScene = false;
        timer = 0;

        level = PlayerPrefs.GetInt("Level");
        camObj = GameObject.FindGameObjectWithTag("MainCamera");
        waypoints[3] = camObj.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //Start after a set delay
        if (Time.time - delay > 10.0f && Time.time - delay < 10.1f)
        {
            GetComponentInChildren<AICharacterControl>().SetTarget(waypoints[0]);
        }
        // register that the vet sequence has begun
        if (start && GetComponent<NavMeshAgent>().velocity.magnitude>0.21f)
        {
            start = false;
        }
        //match our animation speed to our move speed
        if (!start){
            speed = GetComponent<NavMeshAgent>().velocity.magnitude;
            GetComponent<Animator>().SetFloat("Speed", speed + 0.2f);
            //Once the vet slows to a stop before the user transition into the next state
            if (speed < 0.1f)
            {
                GetComponent<Animator>().SetFloat("Speed", 1);
                GetComponent<Animator>().SetBool("Transition", true);
            }           
        }
        //Set our waypoints based on the user's position. This tries to ensure we end with the vet facing the user even if the user has moved somewhat
        if (Vector3.Distance (transform.position, waypoints[0].position) < .5f )
        {            
            waypoints[1].SetPositionAndRotation(new Vector3(waypoints[3].position.x, waypoints[1].position.y, waypoints[3].position.z+1.2f),waypoints[1].rotation);
            waypoints[2].SetPositionAndRotation(new Vector3(waypoints[3].position.x, waypoints[2].position.y, waypoints[3].position.z + .8f), waypoints[1].rotation);
            GetComponentInChildren<AICharacterControl>().SetTarget(waypoints[1]);
        }
        //walk to next waypoint
        if (Vector3.Distance(transform.position, waypoints[1].position) < .1f)
        {            
            GetComponentInChildren<AICharacterControl>().SetTarget(waypoints[2]);
        }
        // start speech once the vet has come to a halt
        if (!talk & speed<0.01f)
        {
            GetComponent<AudioSource>().time = 0.8f;
            GetComponent<AudioSource>().Play();
            WriteString("Vet Speech: ");
            talk = true;
        }
        //end scene once the user has taken the ball from the vet
        if (endScene)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = lights[i].intensity * .99f;
                if (timer > 2f)
                {
                    Application.Quit();
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }

            RenderSettings.ambientLight = Color.black;
            timer += Time.deltaTime;
        }
        else if (talk)
        {
            if (level == 0 )
            {
                if (timer > 6)
                {
                    TriggerEnd();
                    timer = 0;
                }
                timer += Time.deltaTime;
            }
            
        }
        
    }

    public void TriggerEnd()
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("==== End of Experience ====");
        writer.WriteLine("\n");
        writer.Close();
        endScene = true;
    }

    static void WriteString(string ident)
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(ident + Time.time);
        writer.Close();
    }
}
