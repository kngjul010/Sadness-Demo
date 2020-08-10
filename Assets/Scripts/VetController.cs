using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class VetController : MonoBehaviour
{

    float speed;
    bool start, talk;
    public Transform[] waypoints;
    public GameObject ball;
    public Light[] lights;
    float delay;
    // Initialise variables
    void Start()
    {
        speed = 10;
        start = true;
        talk = false;
        delay = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //Start after a set delay
        if (Time.time - delay > 15.0f && Time.time - delay < 15.1f)
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
            string path = "Times.txt";

            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Vet Speech: " + Time.time);
            writer.Close();
            talk = true;
        }
        //end scene once the user has taken the ball from the vet
        if (ball.GetComponent<MeshRenderer>().enabled == false)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = lights[i].intensity * .99f;
                if (lights[i].intensity < 0.07f)
                {
                    Application.Quit();
                }
            }

            RenderSettings.ambientLight = Color.black;

        }
        
    }
}
