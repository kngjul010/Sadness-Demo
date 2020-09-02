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

    private int level;
    private int dogChosen;
    private Hand leftHand;
    private Hand rightHand;
    private DogParkDalmation dogScript;
    private float dogSpan;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        level = PlayerPrefs.GetInt("Level");
        dogChosen = PlayerPrefs.GetInt("Dog");
        dogChosen = 1; //test Line
        level = 1; //test Line

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
        player.transform.position = new Vector3(53.37283f, 49.85787f, -126.5f);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand = GameObject.FindGameObjectWithTag("Left Hand").GetComponent<Hand>();
        rightHand = GameObject.FindGameObjectWithTag("Right Hand").GetComponent<Hand>();
        dogScript = dog.GetComponent<DogParkDalmation>();
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

            dogScript.interactionStage = 0;
        }
        else if (level == 1){
            dogScript.interactionStage = 1;
        }
        else if (level == 2)
        {
            dogScript.interactionStage = 2;
        }

        dogSpan = Time.time;
    }

    private void Update()
    {
        if (dogScript.numInteractions < 10 && Time.time - dogSpan > dogScript.dogLife / 2)
        {
            dogScript.bond += 0.2f;
            string path = "Times.txt";

            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Director Incrased Bond: " + Time.time);
        }
    }

}
