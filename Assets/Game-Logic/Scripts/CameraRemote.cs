using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRemote : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private float lookSpeed = 10.0F;
    [SerializeField] private float followSpeed = 10.0F;
    [SerializeField] private Vector3 offsite;


    private void LookAtTarget() {
        Vector3 lookDirection = targetObject.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, lookSpeed * Time.deltaTime);
    }
    // Update is called once per frame

    private void MoveToTarget() {
        Vector3 targetPos = targetObject.position +
                            targetObject.forward * offsite.z +
                            targetObject.right * offsite.x +
                            targetObject.up * offsite.y;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
    void FixedUpdate()
    {
        LookAtTarget();
        MoveToTarget();
    }
}
