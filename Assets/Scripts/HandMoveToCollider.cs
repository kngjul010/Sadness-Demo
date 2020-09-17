using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move the hand models to the Hand colliders - Ensures they do not move through objects
public class HandMoveToCollider : MonoBehaviour
{
    public Transform hand;
    public bool isRightHand;
    public Transform collider;
    public Vector3 rotationOffset;
    public Vector3 positionOffset;
    public bool isStartHand;

    // Update is called once per frame
    void Update()
    {
        if (collider == null)
        {
            if (isRightHand) collider = GameObject.FindGameObjectWithTag("rCollider").transform;
            else collider = GameObject.FindGameObjectWithTag("lCollider").transform;
            transform.SetParent(collider);
            hand.localPosition = positionOffset;
            hand.localRotation = Quaternion.Euler(rotationOffset);
            if (!isStartHand) gameObject.SetActive(false);

        }
    }
}
