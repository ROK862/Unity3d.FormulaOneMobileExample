using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadorAI : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentBreakForce;
    private int currentSpeed;
    private float invertedSteering = 0.0f;

    private int halfSecondInterval = 0;
    private void HalfSecondUpdate() {
        halfSecondInterval++;
        if (halfSecondInterval < 5) {
            HandleMotor();
            HandleSteering();
            UpdateWheels();
            return;
        }
        halfSecondInterval = 0;

        if (!enableAI)
        {
            GetInput();
        }
        GetDownForce();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        HandleGearChage();
        HandleGroundDetection();

        WheelHit w_rear_l_hit;
        WheelHit w_front_l_hit;
        WheelHit w_rear_r_hit;
        WheelHit w_front_r_hit;

        frontLeftWheelCollider.GetGroundHit(out w_front_l_hit);
        rearLeftWheelCollider.GetGroundHit(out w_rear_l_hit);
        rearRightWheelCollider.GetGroundHit(out w_rear_r_hit);
        frontRightWheelCollider.GetGroundHit(out w_front_r_hit);

        if (w_front_r_hit.sidewaysSlip >= skidVelocity || w_front_r_hit.sidewaysSlip <= (skidVelocity*-1))
        {
            frontRightWheelSmoke.Play();
            if (skidSound)
                if (!skidSound.isPlaying)
                    skidSound.Play();
        }
        else
        {
            frontRightWheelSmoke.Stop();
        }

        if (w_front_l_hit.sidewaysSlip >= skidVelocity || w_front_l_hit.sidewaysSlip <= (skidVelocity * -1))
        {
            frontLeftWheelSmoke.Play();
            if (skidSound)
                if (!skidSound.isPlaying)
                    skidSound.Play();
        }
        else
        {
            frontLeftWheelSmoke.Stop();
        }

        if (w_rear_r_hit.sidewaysSlip >= skidVelocity || w_rear_r_hit.sidewaysSlip <= (skidVelocity * -1))
        {
            rearRightWheelSmoke.Play();
            if (skidSound)
                if (!skidSound.isPlaying)
                    skidSound.Play();
        }
        else
        {
            rearRightWheelSmoke.Stop();
        }

        if (w_rear_l_hit.sidewaysSlip >= skidVelocity || w_rear_l_hit.sidewaysSlip <= (skidVelocity * -1))
        {
            rearLeftWheelSmoke.Play();
            if (skidSound)
                if (!skidSound.isPlaying)
                    skidSound.Play();
        }
        else
        {
            rearLeftWheelSmoke.Stop();
        }
    }

    public int GetSpeed()
    {
        return currentSpeed;
    }

    public void InvokeBrakingSequence()
    {
        enableBreaks();
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
    [SerializeField] private float steeringSpeed = 1.0F;

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
    [SerializeField] internal float maxSteeringAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private Text speedoMeter;
    [SerializeField] private Text gearMeter;

    [SerializeField] private bool autoDrive;

    [SerializeField] private ParticleSystem rearLeftWheelSmoke;
    [SerializeField] private ParticleSystem rearRightWheelSmoke;
    [SerializeField] private ParticleSystem frontRightWheelSmoke;
    [SerializeField] private ParticleSystem frontLeftWheelSmoke;
    [SerializeField] private ParticleSystem collisionEffect;

    [SerializeField] private float skidVelocity = 0.45f;

    [SerializeField] private AudioSource collisionSound;
    [SerializeField] private AudioSource skidSound;


    public bool reverseSequence = false;

    public void Start()
    {
        rearLeftWheelSmoke.Stop();
        rearRightWheelSmoke.Stop();
        frontRightWheelSmoke.Stop();
        frontLeftWheelSmoke.Stop();

        engineAudioSource = gameObject.GetComponent<AudioSource>();

        vehicleRigidbody = gameObject.GetComponent<Rigidbody>();

        if (vehicleRigidbody != null)
        {
            carMass = vehicleRigidbody.mass;
        }
    }
    // Update is called once per frame
    private void Update()
    {
        HalfSecondUpdate();
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

    private void HandleGearChage()
    {
        gearInterval = topSpeed / gearCount;
        pitchOffset = (3.0F / gearCount) * currentGear;

        currentGear = Convert.ToInt32(currentSpeed / gearInterval);

        currentGear = (currentGear > gearCount) ? gearCount : currentGear;

        if (currentSpeed > 0)
        {
            currentGear += 1;
        }

        if (gearMeter != null)
        {
            gearMeter.text = ((currentGear != 0) ? currentGear.ToString() : "N");
        }

        enginePitchRatio = 1 - (((gearInterval * currentGear) - currentSpeed) / gearInterval);
        enginePitchRatio = (enginePitchRatio <= 0) ? 0.05F : (enginePitchRatio * pitchMultiplier);

        if (engineAudioSource != null)
        {
            engineAudioSource.pitch = pitchOffset + enginePitchRatio;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentSpeed > 135.0F)
        {
            if (collisionSound != null) {
                if (!collisionSound.isPlaying && collision.relativeVelocity.magnitude > 2)
                    collisionSound.Play();
            }
            var contacts = collision.contacts;

            for (var x = 0; x < contacts.Length; x++) {
                if (collisionEffect != null)
                {
                    var instance = GameObject.Instantiate(collisionEffect.gameObject);
                    var pos = contacts[x].point;
                    var rot = Quaternion.FromToRotation(instance.transform.up, contacts[x].normal) * instance.transform.rotation;

                    instance.transform.position = pos;
                    instance.transform.rotation = rot;
                    instance.SetActive(true);
                }
            }
        }
    }

    private void GetDownForce()
    {
        currentSpeed = 1;
        if (vehicleRigidbody != null)
        {
            currentSpeed = Convert.ToInt32(vehicleRigidbody.velocity.magnitude * (7.2));
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
        float speed = GetSpeed();
        speed = ((speed <= topSpeed) ? speed : topSpeed);
        speed = 1 - (speed / topSpeed);
        speed = (speed < 0.35f) ? 0.35f : speed;

        currentSteeringAngle = (maxSteeringAngle * horizontalInput) * speed;
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(frontLeftWheelCollider.steerAngle, currentSteeringAngle, Time.deltaTime * steeringSpeed);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(frontRightWheelCollider.steerAngle, currentSteeringAngle, Time.deltaTime * steeringSpeed);
    }

    private void HandleMotor()
    {
        if (frontLeftWheelCollider.isGrounded)
            frontLeftWheelCollider.motorTorque = verticalInput * (motorForce * torqueDistribution.x);
        if (frontRightWheelCollider.isGrounded)
            frontRightWheelCollider.motorTorque = verticalInput * (motorForce * torqueDistribution.x);

        if (isAllWheelDrive)
        {
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
        if (autoDrive && !reverseSequence)
        {
            verticalInput = 1F;
        }
        else if (reverseSequence) {
            verticalInput = -1F;
        }
        else
        {
            verticalInput = Input.GetAxis(VERTICAL);
        }
        //verticalInput = Input.GetAxis(VERTICAL) * -1;
        if (brakeOverwrite)
        {
            brakeOverwrite = false;
        }
        else
        {
            isBraking = false;
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
    
    public void enableBreaks()
    {
        if (currentSpeed < 5.0 && !reverseSequence)
        {
            reverseSequence = true;
            invertedSteering = 0.0f;
            isBraking = false;
        }
        else if (!reverseSequence)
        {
            brakeOverwrite = true;
            isBraking = true;
        }
    }

    public void SetHorizontalInput (float h) {
        if (reverseSequence) {
            horizontalInput = (maxSteeringAngle / invertedSteering);
        } else {
            horizontalInput = h;
        }
    }
}
