using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPeformanceManager : MonoBehaviour
{
    int engineTemperature;
    int frontRightWheelTemperature;
    int frontLeftWheelTemperature;
    int rearRightWheelTemperature;
    int rearLeftWheelTemperature;


    [SerializeField] WheelCollider frontRightWheel;
    [SerializeField] WheelCollider frontLeftWheel;
    [SerializeField] WheelCollider rearRightWheel;
    [SerializeField] WheelCollider rearLeftWheel;
    [SerializeField] float DRScoefficient;

    // Start is called before the first frame update
    void Start()
    {
        SimulateStartingWheelTemperature();
    }

    private void SimulateStartingWheelTemperature()
    {
        System.Random rnd = new System.Random();

        frontRightWheelTemperature = rnd.Next(75, 85);
        frontLeftWheelTemperature = rnd.Next(75, 85);
        rearRightWheelTemperature = rnd.Next(75, 85);
        rearLeftWheelTemperature = rnd.Next(75, 85);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
