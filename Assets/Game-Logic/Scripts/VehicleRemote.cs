using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleRemote : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentBreakForce;
    [SerializeField] private int currentSpeed;

    private int accelerationTrigger = 0;
    private int brakeTrigger = 0;

    public bool enableMobileControls = false;

    internal int GetSpeed()
    {
        return currentSpeed;
    }

    internal void InvokeBrakingSequence()
    {
        brakeOverwrite = true;
        isBraking = true;
    }

    private float currentSteeringAngle;
    private bool brakeOverwrite;
    private Rigidbody vehicleRigidbody;
    private AudioSource engineAudioSource;


    [SerializeField] private Vector2 torqueDistribution;
    [SerializeField] private float pitchMultiplier = 1.0F;
    [SerializeField] private float pitchOffset = 0.0F;
    [SerializeField] private float gearInterval = 0.0F;
    [SerializeField] private int currentGear = 0;
    [SerializeField] private float enginePitchRatio = 0.0F;

    [SerializeField] private int gearCount = 8;
    [SerializeField] private int topSpeed = 345;
    [SerializeField] private bool enableAI;
    [SerializeField] private bool isAllWheelDrive;
    [SerializeField] private bool isBraking;
    [SerializeField] private float accelerationSensitivity = 1.2F;
    [SerializeField] private float carMass = 750.0F;
    [SerializeField] private float connerGrip = 6.67F;
    [SerializeField] private float downforceCoeficient = 1.5F;


    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float deadendDistance = 5;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private float handheldSteeringMultiplier = 1.6F;

    [SerializeField] private Text[] speedoMeter;
    [SerializeField] private Text[] gearMeter;

    [SerializeField] private bool autoDrive;
    [SerializeField] private bool deadEnd;
    [SerializeField] private bool breakAssist;
    [SerializeField] private float graceSpeedLimit = 25;
    [SerializeField] private Vector3 sensorOffset;
    [SerializeField] private Transform forwardObject;
    [SerializeField] private Transform stearingWheel;




    public void Start()
    {
        //stearingWheel = gameObject.transform.Find("StearingWheel");
        engineAudioSource = gameObject.GetComponent<AudioSource>();
        vehicleRigidbody = gameObject.GetComponent<Rigidbody>();

        if (vehicleRigidbody != null)
        {
            carMass = vehicleRigidbody.mass;
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!HandleDeadEnds()) {
            if (!enableAI)
            {
                GetInput();
            }
            else
            {

            }
            GetDownForce();
            HandleImageEffects();
            HandleMotor();
            HandleSteering();
            UpdateWheels();

            if (brakeTrigger == 1) {
                brakeTrigger = 0;
            }

            if (accelerationTrigger == 1)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    accelerationTrigger = 0;
                }
            }
        }
    }

    public void HandleImageEffects() {
    }

    float newYAngle = 0;
    public void Update()
    {
        HandleGearChage();
        HandleGroundDetection();
        newYAngle = ((maxSteeringAngle*2) * (horizontalInput*-1));
        Rotate(newYAngle);
    }

    void Rotate(float targetAngle)
    {
        if (stearingWheel == null) return;
        stearingWheel.transform.localRotation = Quaternion.Slerp(stearingWheel.transform.localRotation, Quaternion.Euler(0f, 0f, targetAngle), 10f * Time.deltaTime);
    }

    private bool HandleDeadEnds() {
        RaycastHit hit;
        RaycastHit hit3;

        if (currentSpeed < 5)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, deadendDistance))
            if (hit.distance < deadendDistance)
            {
                deadEnd = true;
                autoDrive = false;
            }
        } else if (breakAssist && currentSpeed > 50) {
            int layerMask = 1 << 7;

            // Does the ray intersect any objects which are in the AI layer.
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit3, Mathf.Infinity, layerMask))
            {
                forwardObject = hit3.transform;
                RadorAI hitAI = forwardObject.GetComponent<RadorAI>();

                if (hitAI != null)
                {
                    if (currentSpeed > hitAI.GetSpeed() + graceSpeedLimit)
                    {
                        brakeTrigger = 1;
                    }
                }
            }
        } 

        if (deadEnd)
        {
            verticalInput = -1;
            GetDownForce();
            HandleMotor();
            HandleSteering();
            UpdateWheels();

            RaycastHit hit2;
            if (Physics.Raycast(transform.position, transform.forward, out hit2, Mathf.Infinity))
            {
                if (hit2.distance > deadendDistance)
                {
                    deadEnd = false;
                    autoDrive = true;
                }
            }
            else {
                deadEnd = false;
                autoDrive = true;
            }

            return true;
        }
        else {
            return false;
        }
    }

    private void HandleGroundDetection()
    {
        RaycastHit hit; int layerMask = 1 << 8;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(frontLeftWheelTransform.position, frontLeftWheelTransform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log(hit.collider.material.name);
        }
    }

    public void setBrakeTrigger(int val)
    {
        brakeTrigger = val;
    }

    public void setAccelerationTrigger(int val)
    {
        accelerationTrigger = val;
    }

    private void HandleGearChage()
    {
        gearInterval = topSpeed / gearCount;
        pitchOffset = (3.0F / gearCount) * currentGear;

        currentGear = Convert.ToInt32(currentSpeed / gearInterval);

        currentGear = (currentGear > gearCount) ? gearCount : currentGear;

        if (currentSpeed > 0) {
            currentGear += 1;
        }

        if (gearMeter.Length != 0) {
            foreach (var m in gearMeter)
                m.text = ((currentGear != 0) ? currentGear.ToString() : "N");
        }

        enginePitchRatio = 1 - (((gearInterval * currentGear) - currentSpeed) / gearInterval);
        enginePitchRatio = (enginePitchRatio <= 0) ? 0.05F : (enginePitchRatio * pitchMultiplier);

        if (engineAudioSource != null) {
            engineAudioSource.pitch = pitchOffset + enginePitchRatio;
        }
    }

    void OnCollisionEnter() {
        if (currentSpeed > 190.0F) {
            Handheld.Vibrate();
        }
    }

    private void GetDownForce()
    {
        currentSpeed = 1;
        if (vehicleRigidbody != null)
        {
            currentSpeed = Convert.ToInt32(vehicleRigidbody.velocity.magnitude * (7.2));


            if (speedoMeter.Length != 0)
            {
                foreach (var m in speedoMeter)
                    m.text = currentSpeed + " kph"; ;
            }
        }

        float downforce = connerGrip * currentSpeed;
        vehicleRigidbody.mass = carMass + downforce;
        var df = (((currentSpeed > 0) ? currentSpeed : 1) / topSpeed);
        vehicleRigidbody.AddForce(((df * carMass) * downforceCoeficient) * Vector3.down);
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void HandleSteering()
    {

        if (SystemInfo.deviceType == DeviceType.Handheld || enableMobileControls)
        {
            horizontalInput = Input.acceleration.x * handheldSteeringMultiplier; // * accelerationSensitivity;
            horizontalInput = (horizontalInput > 1) ? 1 : ((horizontalInput < -1) ? -1 : horizontalInput);
            currentSteeringAngle = maxSteeringAngle * horizontalInput;
        }
        else {
            currentSteeringAngle = maxSteeringAngle * horizontalInput;
        }
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }

    private void HandleMotor()
    {
        if (frontLeftWheelCollider.isGrounded)
            frontLeftWheelCollider.motorTorque = verticalInput * (motorForce * torqueDistribution.x);
        if (frontRightWheelCollider.isGrounded)
            frontRightWheelCollider.motorTorque = verticalInput * (motorForce * torqueDistribution.x);

        if (isAllWheelDrive) {
            if (rearLeftWheelCollider.isGrounded)
                rearLeftWheelCollider.motorTorque = verticalInput * (motorForce * torqueDistribution.y);
            if (rearRightWheelCollider.isGrounded)
                rearRightWheelCollider.motorTorque = verticalInput * (motorForce * torqueDistribution.y);
        }

        currentBreakForce = isBraking ? breakForce : 0f;

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        if (frontLeftWheelCollider.isGrounded)
            frontLeftWheelCollider.brakeTorque = currentBreakForce;
        if (frontRightWheelCollider.isGrounded)
            frontRightWheelCollider.brakeTorque = currentBreakForce;
        if (rearLeftWheelCollider.isGrounded)
            rearLeftWheelCollider.brakeTorque = currentBreakForce;
        if (rearRightWheelCollider.isGrounded)
            rearRightWheelCollider.brakeTorque = currentBreakForce;
    }

    private void GetInput()
    {
        if (autoDrive || (SystemInfo.deviceType == DeviceType.Handheld && brakeTrigger != 1)) {
            verticalInput = 1;
        } else
        {
            verticalInput = Input.GetAxis(VERTICAL);
        }
        horizontalInput = Input.GetAxis(HORIZONTAL);
        //verticalInput = Input.GetAxis(VERTICAL) * -1;
        if (brakeOverwrite)
        {
            brakeOverwrite = false;
        }
        else if (brakeTrigger == 1)
        {
            isBraking = true;
        }
        else {
            isBraking = Input.GetKey(KeyCode.Space);
        }
    }

    private float GetZRotation()
    {
        Quaternion referenceRotation = Quaternion.identity;
        Quaternion deviceRotation = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
        Quaternion eliminationOfXY = Quaternion.Inverse(
            Quaternion.FromToRotation(referenceRotation * Vector3.forward,
                                  deviceRotation * Vector3.forward)
            );
        Quaternion rotationZ = eliminationOfXY * deviceRotation;
        float roll = rotationZ.eulerAngles.z;

        return roll;
    }

    public void enableAutoDrive()
    {
        autoDrive = true;
    }

    public void disableAutoDrive()
    {
        autoDrive = false;
    }

    public void enableBreaks() {
        brakeOverwrite = true;
        isBraking = true;
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }
}
