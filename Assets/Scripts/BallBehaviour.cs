using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script governs the ball's behaviour in the parke scene when it is thrown
public class BallBehaviour : MonoBehaviour
{
    Rigidbody ballPhysics;
    Vector3 oldPos;
    float oldTime, oldVelocity, velocityMag, timeAtStart;
    public bool thrown = false;
    public float timeThrown, timePicked;
    bool begin = true;
    bool slow = false;
    bool start = false;
    bool held = true;
    Vector3 initVelocity;
    public GameObject throwStick, mouthBall;
    public Transform dogBody;

    void Start()
    {
        timeAtStart = Time.time;
        timeThrown = -1.0f;
        timePicked = Time.time;
    }
    void OnEnable()
    {
        // when the ball is enabled after being returned by the dog set it so it looks like the dog drops it towards the user
        if (!begin)
        {           
            transform.position = mouthBall.transform.position + dogBody.forward/4f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(.14f, .14f, .14f);
            ballPhysics.velocity = (throwStick.transform.position - transform.position)/2 + dogBody.forward/2;
            StartCoroutine(StopBall());
        }
        begin = false;
    }
    //We disbale the ball when it is picked up by the dog and enable a second ball in the dog's mouth
    void OnDisable()
    {
        thrown = false;
        begin = false;
    }
    
    // Used to pick up the ball with the throw stick
    void PickUp()
    {
        transform.SetParent(throwStick.transform);
        transform.localPosition = new Vector3(0.36f, 1.88f, 0.88f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(0.12f, .12f, .12f);
        oldVelocity = 0;
        timeThrown = -1.0f;
    }
    //used to prevent the ball rolling indefinitely
    IEnumerator StopBall()
    {
        held = false;
        Physics.IgnoreCollision(GetComponent<Collider>(), throwStick.GetComponent<SphereCollider>(),true);
        yield return new WaitForSeconds(.7f);
        slow = true;
        yield return new WaitForSeconds(1f);
        ballPhysics.isKinematic = true;
        slow = false;
        Physics.IgnoreCollision(GetComponent<Collider>(), throwStick.GetComponent<SphereCollider>(), false);

    }

    //detect the ball being picked up
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == throwStick && !thrown && !slow && !held)
        {
            held = true;
            PickUp();
            timePicked = Time.time;
        }

    }

    void Update()
    {
        //If the ball has somehow fallen through the terrain teleport it back up into the sky and drop it
        if (transform.position.y < 40)
        {
            transform.position = new Vector3(60,55,-120);
        }
        //Only enable the ball for any interaction after a hort delay. Prevents any issues while the scene completes loading
        if (Time.time - timeAtStart > 3 && !start)
        {
            ballPhysics = GetComponent<Rigidbody>();
            oldPos = transform.position;
            oldTime = Time.time;
            oldVelocity = 0;
            begin = true;
            start = true;
        }

        if (start)
        {
            //if the ball has been thrown and did not travel very far then push it in an arbitrary direction so the dog has to run after it
            if (thrown && Time.time-timeThrown > 0.5f && Time.time - timeThrown < 1.2f && Vector3.Distance(dogBody.position, transform.position)<1)
            {
                ballPhysics.velocity += dogBody.right * 2;
            }
            //Check if the ball should be slowing down
            if (slow)
            {
                ballPhysics.velocity = .95f * ballPhysics.velocity;
            }

            float dist = Vector3.Distance(oldPos, transform.position);
            float timePassed = (Time.time - oldTime);
            velocityMag = dist / timePassed;
            //when the balls velocity changes sufficiently quickly while attached to the throwstick throw it
            if ((oldVelocity - velocityMag) / (timePassed * 100) > 5 && Time.time - timePicked > 1&& !thrown && velocityMag > 0.1f && ballPhysics.isKinematic && initVelocity.x < 100)
            {
                gameObject.transform.parent = null;
                ballPhysics.isKinematic = false;
                ballPhysics.velocity = initVelocity;
                ballPhysics.velocity = .5f * ballPhysics.velocity;
                thrown = true;
                timeThrown = Time.time;
                timePicked = -1.0f;
            }
            //initVelocity is used to calulate the ball's intial velocity if it is thrown next frame
            initVelocity = new Vector3((oldPos.x - transform.position.x) / -timePassed, (oldPos.y - transform.position.y) / timePassed, (oldPos.z - transform.position.z) / -timePassed);
            oldPos = transform.position;
            oldTime = Time.time;
            oldVelocity = velocityMag;
        }
        
    }
}
