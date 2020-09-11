using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.IO;

public class VetInteractionControl : MonoBehaviour
{
    public GameObject player;
    public GameObject ball;
    public GameObject teddy;

    private int level;
    private int item; //0 = ball, 1 = teddybear

    // Start is called before the first frame update
    void Start()
    {
        WriteString("== Vet Start: ");
        level = PlayerPrefs.GetInt("Level");
        if (Teleport.instance != null){
            Teleport.instance.CancelTeleportHint();
        }

        if (level == 0)
        {
            ball.SetActive(false);
        }
        else
        {
            item = PlayerPrefs.GetInt("Item");
            if (item == 0)
            {
                ball.SetActive(true);
                teddy.SetActive(false);
            }
            else if (item == 1)
            {
                teddy.SetActive(true);
                //ball.SetActive(false);
                ball.GetComponent<MeshRenderer>().enabled = false;
                ball.GetComponent<Interactable>().enabled = false;
                ball.GetComponent<SphereCollider>().enabled = false;
                WriteString("Teddy Used: ");
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        player.transform.position = new Vector3(-0.581f, 0.39f, 0.071f);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    static void WriteString(string ident)
    {
        string path = "Times.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(ident + Time.time);
        writer.Close();
    }
}
