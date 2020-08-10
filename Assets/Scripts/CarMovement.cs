using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script causes cars to drive up and down in the background of the park scene
public class CarMovement : MonoBehaviour
{
    public Transform [] Waypoints;
    int position;
    public float speed;
    Vector3 startAngle, targetangle;
    Quaternion fromRot;
    float turnComp;

    //start at the first waypoint
    void Start()
    {
        transform.position = Waypoints[0].position;       
        position = 0;
    }

    void FixedUpdate()
    {
        transform.position = transform.position + (speed/1000)*(Waypoints[position + 1].position - Waypoints[position].position);
        //Turn to face the next waypoint
        if (position >= 1) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, turnComp - Vector3.Angle(targetangle,startAngle),0), Time.time * 0.01f);
        }
        //When arriving at a waypoint set our next target waypoint
        if (Vector3.Distance(transform.position, Waypoints[position + 1].position) <10)
        {
            position++;
            if (position == 1) { speed *= 5; }
            if (position == 3) {
                speed *= 0.2f;
                transform.rotation = transform.rotation * Quaternion.Euler(0, 45, 0);
            }
            if (position < 4)
            {
                turnComp = transform.rotation.eulerAngles.y;
                fromRot = transform.rotation;
                startAngle = transform.forward;
                targetangle = Waypoints[position + 1].position - Waypoints[position].position;
            }
        }
        //When arriving at the final position destroy myself
        if (position == 4)
        {
            Destroy(gameObject);
        }
    }
}
