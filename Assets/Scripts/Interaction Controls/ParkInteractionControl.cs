using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.IO;

public class ParkInteractionControl : MonoBehaviour
{
    public GameObject dog;
    public GameObject player;
    public SkinnedMeshRenderer dogMesh;
    public Material dogMaterial;
    public GameObject[] interactableObjects;
    public GameObject teleportObj;
    public GameObject gestureObjs;
    public GameObject tennisball;

    private int level;
    private int dogChosen;
    private Hand leftHand;
    private Hand rightHand;
    private DogParkDalmation dogScript;
    private float dogSpan;
    private bool bondBoost;
    private float bondBoostTime;
    private float groveForceTime;
    private float tennisBallCheckDelay;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        level = PlayerPrefs.GetInt("Level");
        dogChosen = PlayerPrefs.GetInt("Dog");
        dogChosen = 1; //test Line
        level = 2; //test Line
        dogScript = dog.GetComponent<DogParkDalmation>();
        //Happy & Inquisitive
        if (dogChosen == 0)
        {
            dogMesh.material = dogMaterial;
            dogScript.playfulness = 1.4f;
            dogScript.inquisitive = 2;
            dogScript.obedience = 0.8f;
        }
        //Shy & Obedient
        else if (dogChosen == 1)
        {
            dogScript.playfulness = 0.8f;
            dogScript.inquisitive = 1.2f;
            dogScript.obedience = 2f;
        }
        //Playful
        else if (dogChosen == 2)
        {
            dogScript.playfulness = 2f;
            dogScript.inquisitive = 1.4f;
            dogScript.obedience = 0.8f;
        }
        Teleport.instance.CancelTeleportHint();
        player.transform.position = new Vector3(53.9599991f, 50.1300011f, -124.884338f);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand = GameObject.FindGameObjectWithTag("Left Hand").GetComponent<Hand>();
        rightHand = GameObject.FindGameObjectWithTag("Right Hand").GetComponent<Hand>();
        
        //Make it easy to pick items from the floor
        leftHand.fingerJointHoverRadius = 0.5f;
        leftHand.hoverSphereRadius = 0.4f;
        rightHand.fingerJointHoverRadius = 0.5f;
        rightHand.hoverSphereRadius = 0.4f;

        if (level == 0)
        {
            interactableObjects[0].SetActive(false);
            interactableObjects[1].SetActive(false);
            interactableObjects[2].SetActive(false);
            teleportObj.SetActive(false);
            dogScript.interactionStage = 0;
        }
        else if (level == 1){
            dogScript.interactionStage = 1;
            dogScript.dogLife += 20;
        }
        else if (level == 2)
        {
            gestureObjs.SetActive(true);
            dogScript.interactionStage = 2;
            dogScript.dogLife += 40;
        }

        dogSpan = Time.time;
        bondBoost = false;

        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("== Park Start: " + Time.time);
        writer.Close();

        //Sums for optimization
        bondBoostTime = dogScript.dogLife / 2;
        groveForceTime = (dogScript.dogLife * 3) / 4;

        tennisBallCheckDelay = Time.time;
    }

    private void Update()
    {
        //Boost bond to encourage positive interactions
        if (!bondBoost && dogScript.numInteractions < 10 && Time.time - dogSpan > bondBoostTime)
        {
            dogScript.bond += 0.2f;
            string path = "Times.txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Director Incrased Bond: " + Time.time);
            writer.Close();
            bondBoost = true;
        }
        //Show grove if it hasn't already (To force more interaction)
        else if (level == 2 && !dogScript.GetGroveStarted() && Time.time - dogSpan > groveForceTime && dogScript.bond > 1.5f)
        {
            string path = "Times.txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Director Forced Grove Interaction: " + Time.time);
            writer.Close();

            dogScript.SetGroveStarted();
            dogScript.SetState("showGrove");

        }
        //Fetch ball in stage 0 if it is out of reach
        else if (level == 0 && Time.time - tennisBallCheckDelay > 10)
        {
            tennisBallCheckDelay = Time.time;

            if (Vector3.Distance(tennisball.transform.position, player.transform.position) > 3 && dogScript.GetState() == "idle")
            {
                tennisball.GetComponent<ObjectThrown>().SendMessage();

                string path = "Times.txt";
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine("Director Forced DogFetch: " + Time.time);
                writer.Close();
            }
        }
            
    }

}
