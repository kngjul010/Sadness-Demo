using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple script that spawns cars every few seconds, time between cars and timing window is hardcoded for now
public class Carspawn : MonoBehaviour
{
    public GameObject [] cars;
    public Transform [] waypoints;
    float time;
    void Start()
    {
        time = Time.time - Random.Range(1,8);
    }
    void Update()
    {
        if (Time.time - time > 24)
        {
            time = Time.time - Random.Range(1, 4);
            GameObject newCar = GameObject.Instantiate(cars[(int)Random.Range(0,cars.Length)]);
            newCar.GetComponent<CarMovement>().Waypoints = waypoints;
        }
    }

    
}
