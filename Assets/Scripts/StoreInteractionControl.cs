using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class StoreInteractionControl : MonoBehaviour
{
    [Header("Level 0 Object to Disable")]
    public GameObject leftPen;
    public GameObject rightPen;
    public GameObject leftDog;
    public GameObject rightDog;
    
    public GameObject teleportObj;

    private int level = 0;
    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("Level");
        print(level);
        Teleport.instance.CancelTeleportHint();
        if (level == 0)
        {
            SetLevel0();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
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
