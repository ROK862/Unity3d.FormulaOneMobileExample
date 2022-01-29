using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadorNAV : MonoBehaviour
{
    public int navIndex = 0;

    private const string HORIZONTAL = "Horizontal";
    private RadorAI ai;
    private bool avoiding = false;

    [SerializeField] private Vector3 frontSensorPosition;
    [SerializeField] private float sensorLength = 10.0F;
    [SerializeField] private float ratio;
    [SerializeField] private float angle;
    [SerializeField] private AIPathCreator path;
    [SerializeField] private float triggerDist;
    [SerializeField] private float breakDist = 5.0F;
    [SerializeField] private Vector3 rayOffset = new Vector3(0,1,0);
    [SerializeField] private float frontSideSensorPosition;
    [SerializeField] private float forwardSensorAngle = 25.0F;
    [SerializeField] private float avoidSteeringSmoothness = 0.5f;
    [SerializeField] public float lapComplitionStatus = 0.0F;
    [SerializeField] public int currentLap = 0;
    [SerializeField] public float stearingBufferSize = 0.25f;

    public void SetPercentage(float perc) { lapComplitionStatus = perc; }
    public float GetCircuitProgress() {return float.Parse(float.Parse(currentLap.ToString()) + lapComplitionStatus.ToString());}
    public void SetLap() { currentLap++; }
    // Start is called before the first frame update
    void Start()
    {
        if (path == null) 
            path = GameObject.Find("AI-Path").GetComponent<AIPathCreator>();

        ai = gameObject.GetComponent<RadorAI>();
    }

    // Update is called once per frame
    public void Update()
    {
        Vector3 p1 = transform.position;
        Vector3 p2 = path.GetNextPoint(p1,this);
        Vector3 relativePoint = transform.InverseTransformPoint(p2);
       

        Vector3 tp0 = p2 - p1;
        angle = Vector3.Angle(tp0, transform.forward);
        if (angle > ai.maxSteeringAngle)
            angle = ai.maxSteeringAngle;
        ratio = (angle / ai.maxSteeringAngle);

        Debug.DrawLine(p1, p2, Color.green) ;

        //Check if target is to the left or right of the vehicle?
        if (relativePoint.x < 0)
        {
            ratio *= -1;
        }
        ai.SetHorizontalInput(GetCarSensorsInfluence(ratio));

        //AvoidObsticalAhead();
    }

    float steeringCache = 0;

    private float GetCarSensorsInfluence(float ratio) {

        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        sensorStartPos += transform.right * frontSensorPosition.x;

        float avoidMultiplier = (steeringCache * stearingBufferSize);
        int hitCount = 0;
        float averageHitDistance = 0;
        avoiding = false;

        //Front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier += 0.65F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }

        //Front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(forwardSensorAngle, transform.up) * transform.forward, out hit, (sensorLength / 2)))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier += 0.45F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(forwardSensorAngle * 2, transform.up) * transform.forward, out hit, (sensorLength / 3)))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier += 0.25F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }

        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(forwardSensorAngle * 2, transform.up) * transform.forward, out hit, (sensorLength / 4)))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier += 0.10F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }

        //Forward left sensor
        sensorStartPos -= transform.right * (frontSideSensorPosition * 2);
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength ))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier -= 0.65F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }

        //Front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(forwardSensorAngle * -1, transform.up) * transform.forward, out hit, ((sensorLength / 2))))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier -= 0.45F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }

        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis((forwardSensorAngle * 2) * -1, transform.up) * transform.forward, out hit, ((sensorLength / 3))))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier -= 0.25F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis((forwardSensorAngle * 2) * -1, transform.up) * transform.forward, out hit, ((sensorLength / 3))))
        {
            if (!hit.collider.CompareTag("Drivable"))
            {
                avoiding = true;
                avoidMultiplier -= 0.10F;
                averageHitDistance += hit.distance;
                hitCount++;
                Debug.DrawLine(sensorStartPos, hit.point);
            }
        }

        if (avoidMultiplier == 0) {
            //Forward center sensor
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                if (!hit.collider.CompareTag("Drivable"))
                {
                    avoiding = true;
                    averageHitDistance += hit.distance;
                    hitCount++;

                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = -1;
                    }
                    else
                    {
                        avoidMultiplier = 1;
                    }

                    if (hit.collider.material.name.Contains("Car-Body")) {
                        avoidMultiplier *= -1;
                    }

                    if (hit.distance < breakDist)
                    {
                        ai.InvokeBrakingSequence();
                    }
                    else if (hit.distance >= 0.8F * sensorLength) {
                        ai.reverseSequence = false;
                    }
                    Debug.DrawLine(sensorStartPos, hit.point);
                } else if (hit.distance >= 0.8F * sensorLength) {
                    ai.reverseSequence = false;
                }
            }
            else {
                ai.reverseSequence = false;
            }
        }

        steeringCache = avoidMultiplier;
        var avoidanceStrength = ((averageHitDistance / hitCount) - sensorLength) / sensorLength;

        var avoidSteering = avoidanceStrength * avoidMultiplier;
        if (avoiding && avoidMultiplier != 0)
        {
            return avoidSteering * avoidSteeringSmoothness;
        }
        else {
            return ratio;
        }
    }

    private void AvoidObsticalAhead()
    {
        
    }

    private float GetDistanceToObstical(Vector3 direction, float distance = Mathf.Infinity) {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position + rayOffset, transform.TransformDirection(direction), out hit, distance, layerMask))
        {
            var dist = Mathf.Infinity;
            RadorAI hitAI = hit.transform.GetComponent<RadorAI>();

            if (hitAI != null)
            {
                dist = Vector3.Distance(transform.position, hitAI.transform.position);
            }

            return dist;
        }
        else
        {
            return Mathf.Infinity;
        }
    }
}
