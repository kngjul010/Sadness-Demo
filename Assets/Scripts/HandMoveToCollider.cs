using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //hand.position = (collider.position + positionOffset);
        //hand.rotation = Quaternion.Euler(new Vector3(collider.rotation.eulerAngles.x + rotationOffset.x, collider.rotation.eulerAngles.y + rotationOffset.y, collider.rotation.eulerAngles.z + rotationOffset.z));
    }
}
