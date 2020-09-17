using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections;
using System.IO;
using Valve.VR;
using Valve.VR.InteractionSystem;

//Make the dog start walking to the player when the door is opened
public class DogDoor : MonoBehaviour
{

    public Transform master;
    public Transform pup;
    public int DogChosen;

    //When the hand is by the door
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();

        if (startingGrabType != GrabTypes.None){
            //Set it to have proper physics
            Rigidbody body = GetComponentInParent<Rigidbody>();
            body.isKinematic = false;
            body.WakeUp();
            HingeJoint doorJoint = GetComponentInParent<HingeJoint>();
            doorJoint.useSpring = true;

            StartCoroutine(SetMaster());
            //Start recording the Times of user actions in a text file (This can be improved on)
            string path = "Times.txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Puppy Chosen: " + Time.time);
            writer.Close();
        }
    }

    void Update()
    {

    }

    //Method used to make the puppy move towards the user after a short delay
    public IEnumerator SetMaster()
    {
        yield return new WaitForSeconds(2.5f);
        pup.gameObject.AddComponent<ThirdPersonCharacter>();
        pup.gameObject.AddComponent<AICharacterControl>();
        pup.GetComponent<Animator>().SetInteger("Next", 5);
        pup.gameObject.GetComponent<Puppy_Controller>().dogChosen = DogChosen;
        pup.gameObject.GetComponent<Puppy_Controller>().chosen = true;
    }


    private void OnHandHoverEnd()
    {
        // Set angular velocity to zero if the hand stops hovering
        GetComponentInParent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}


