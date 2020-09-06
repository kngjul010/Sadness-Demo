using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;
using System.IO;

public class StoreInteractionControl : MonoBehaviour
{
    [Header("Level 0 Object to Disable")]
    public GameObject leftPen;
    public GameObject rightPen;
    public GameObject leftDog;
    public GameObject rightDog;
    
    public GameObject lvl0Door;
    public GameObject teleportObj;

    [Header ("Dog Scripts")]
    public Puppy_Controller midDogC;
    public Puppy_Controller rDogC;
    public Puppy_Controller lDogC;

    private int level = 0;
    private float timer;
    private bool doggo0;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("Level");
        Teleport.instance.CancelTeleportHint();
        if (level == 0)
        {
            SetLevel0();
            doggo0 = true;
        }
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        midDogC.puppySpot = player.transform;
        lDogC.puppySpot = player.transform;
        rDogC.puppySpot = player.transform;
        player.transform.position = new Vector3(0.335f, player.transform.position.y, -1.774418f);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        lDogC.interactionStage = level;
        midDogC.interactionStage = level;
        rDogC.interactionStage = level;
    }

    // Update is called once per frame
    void Update()
    {
        if (level == 0 && doggo0 && timer > 10 )
        {
            DogDoor doorScript = lvl0Door.GetComponent<DogDoor>();
            Rigidbody body = lvl0Door.GetComponent<Rigidbody>();
            body.isKinematic = false;
            body.WakeUp();
            HingeJoint doorJoint = lvl0Door.GetComponent<HingeJoint>();
            doorJoint.useSpring = true;
            StartCoroutine(doorScript.SetMaster());

            string path = "Times.txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Puppy Chosen: " + Time.time);
            writer.Close();
            doggo0 = false;

            body.AddForce(new Vector3(-0.5f, 0, -1),ForceMode.Impulse);
        }
        timer += Time.deltaTime;
    }

    public void SetLevel0()
    {
        leftPen.SetActive(false);
        leftDog.SetActive(false);
        rightPen.SetActive(false);
        rightDog.SetActive(false);
        teleportObj.SetActive(false);
    }
}
