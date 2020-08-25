using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ParkInteractionControl : MonoBehaviour
{
    public GameObject dog;
    public GameObject player;
    public SkinnedMeshRenderer dogMesh;
    public Material dogMaterial;


    private int level;
    private int dogChosen;
    private Hand leftHand;
    private Hand rightHand;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        level = PlayerPrefs.GetInt("Level");
        dogChosen = PlayerPrefs.GetInt("Dog");
        dogChosen = 1; //test Line
        if (dogChosen == 0)
        {
            dogMesh.material = dogMaterial;
        }
        Teleport.instance.CancelTeleportHint();
        player.transform.position = new Vector3(53.37283f, 49.85787f, -127.6087f);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        leftHand = GameObject.FindGameObjectWithTag("Left Hand").GetComponent<Hand>();
        rightHand = GameObject.FindGameObjectWithTag("Right Hand").GetComponent<Hand>();

        //Make it easy to pick items from the floor
        leftHand.fingerJointHoverRadius = 0.5f;
        leftHand.hoverSphereRadius = 0.4f;
        rightHand.fingerJointHoverRadius = 0.5f;
        rightHand.hoverSphereRadius = 0.4f;
    }


}
