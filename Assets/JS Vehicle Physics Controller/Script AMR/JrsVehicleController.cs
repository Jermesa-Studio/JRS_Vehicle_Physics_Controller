// MIT License
//
// Copyright (c) 2023 Samborlang Pyrtuh
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class JrsVehicleController : MonoBehaviour
{
    public float motorForce = 50f;
    public float maxSteerAngle = 30f;
    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    public Transform frontLeftWheelTransform, frontRightWheelTransform, rearLeftWheelTransform, rearRightWheelTransform;

    public GameObject centerOfMassObject;
    private Rigidbody rb;
    public WheelCollider[] wheelCollidersBrake;
    public float brakeForce = 500f;

    public float[] gearRatios; // Array to store the gear ratios for each gear
    public float shiftThreshold = 5000f; // Threshold value for shifting to a higher gear
    private int currentGear = 1; // Variable to track the current gear

    public bool enable4x4 = false; // Option to enable 4-wheel drive

    private float stopSpeedThreshold = 1f; // Speed threshold for considering the vehicle stopped

    public ParticleSystem frontLeftDustParticleSystem, frontRightDustParticleSystem, rearLeftDustParticleSystem, rearRightDustParticleSystem; // References to the dust particle systems for each wheel

    private Quaternion prevRotation; // Previous rotation of the wheel

    private JrsInputController mobileInputController;

    public AudioSource engineAudioSource; // Assign this in the Inspector
    private AudioClip engineSound;
    private float targetPitch;
    public AudioSource engineStartAudioSource; // Assign this in the Inspector

    private bool hasStartedMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        prevRotation = frontLeftWheelTransform.rotation;

        mobileInputController = FindObjectOfType<JrsInputController>();

        engineSound = Resources.Load<AudioClip>("EngineSound");
        targetPitch = engineAudioSource.pitch;

        StartCoroutine(DelayedEngineSound());
    }

        IEnumerator DelayedEngineSound()
        {
            while (!hasStartedMoving)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2f); // Delay for 2 seconds

            engineAudioSource.Play();
        }
        

    void Update()
    {
        if (centerOfMassObject)
        {
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMassObject.transform.position);
        }

        float v = mobileInputController != null ? mobileInputController.GetVerticalInput() : Input.GetAxis("Vertical") * motorForce;
        float h = mobileInputController != null ? mobileInputController.GetHorizontalInput() : Input.GetAxis("Horizontal") * maxSteerAngle;

        // Apply motor torque to the wheels
        frontLeftWheel.motorTorque = v;
        frontRightWheel.motorTorque = v;

        // Apply steering angle to the front wheels
        frontLeftWheel.steerAngle = h;
        frontRightWheel.steerAngle = h;

        // Update wheel poses
        UpdateWheelPoses();

        if (Input.GetKey(KeyCode.Space) || mobileInputController.brakeButton.IsButtonPressed())
        {
            foreach (WheelCollider wheelCollider in wheelCollidersBrake)
            {
                wheelCollider.brakeTorque = brakeForce;
            }
        }
        else
        {
            foreach (WheelCollider wheelCollider in wheelCollidersBrake)
            {
                wheelCollider.brakeTorque = 0;
            }
        }
    }

    void FixedUpdate()
    {
        float v = mobileInputController != null ? mobileInputController.GetVerticalInput() * motorForce : 0f;
        float h = mobileInputController != null ? mobileInputController.GetHorizontalInput() * maxSteerAngle : 0f;

        // Calculate the current wheel speed in km/h
        float currentSpeedKmph = frontLeftWheel.radius * Mathf.PI * frontLeftWheel.rpm * 60f / 1000f;
        Debug.Log("Current Speed: " + currentSpeedKmph + " Kmph");

        // Calculate the current engine RPM based on the wheel speed and gear ratio
        float currentRPM = frontLeftWheel.rpm * gearRatios[Mathf.Clamp(currentGear - 1, 0, gearRatios.Length - 1)];

        // Check if it's time to shift to a higher gear
        if (currentRPM > shiftThreshold && currentGear < gearRatios.Length)
        {
            currentGear++; // Shift to the next gear
        }
        else if (currentSpeedKmph < stopSpeedThreshold && currentGear > 1)
        {
            currentGear--; // Shift to the previous gear when slowing down
        }

        // Adjust the motor torque based on the current gear ratio
        float adjustedTorque = v * gearRatios[Mathf.Clamp(currentGear - 1, 0, gearRatios.Length - 1)];

        // Apply motor torque to the wheels
        if (enable4x4)
        {
            frontLeftWheel.motorTorque = adjustedTorque;
            frontRightWheel.motorTorque = adjustedTorque;
            rearLeftWheel.motorTorque = adjustedTorque;
            rearRightWheel.motorTorque = adjustedTorque;
        }
        else
        {
            frontLeftWheel.motorTorque = adjustedTorque;
            frontRightWheel.motorTorque = adjustedTorque;
            rearLeftWheel.motorTorque = 0f; // No torque applied to rear wheels
            rearRightWheel.motorTorque = 0f; // No torque applied to rear wheels
        }

        frontLeftWheel.steerAngle = h;
        frontRightWheel.steerAngle = h;

        UpdateWheelPoses();

        // Calculate the wheel's angular velocity
        Quaternion currentRotation = frontLeftWheelTransform.rotation;
        float angularVelocity = Quaternion.Angle(prevRotation, currentRotation) / Time.fixedDeltaTime;
        prevRotation = currentRotation;

        // Check if the vehicle is in motion
        bool isMoving = rb.velocity.magnitude > 0.1f;

        // Check if any of the wheels are slipping or drifting
        bool isFrontLeftSlipping = IsWheelSlipping(frontLeftWheel);
        bool isFrontRightSlipping = IsWheelSlipping(frontRightWheel);
        bool isRearLeftSlipping = IsWheelSlipping(rearLeftWheel);
        bool isRearRightSlipping = IsWheelSlipping(rearRightWheel);

        bool isFrontLeftDrifting = IsWheelDrifting(frontLeftWheel);
        bool isFrontRightDrifting = IsWheelDrifting(frontRightWheel);
        bool isRearLeftDrifting = IsWheelDrifting(rearLeftWheel);
        bool isRearRightDrifting = IsWheelDrifting(rearRightWheel);

        // Check if any of the wheels are sliding while the brake is applied and the vehicle is in motion
        bool isFrontLeftBraking = IsWheelBraking(frontLeftWheel) && isMoving;
        bool isFrontRightBraking = IsWheelBraking(frontRightWheel) && isMoving;
        bool isRearLeftBraking = IsWheelBraking(rearLeftWheel) && isMoving;
        bool isRearRightBraking = IsWheelBraking(rearRightWheel) && isMoving;

        // Enable/disable the dust particle systems based on wheel slip, drifting, braking, and vehicle motion
        bool shouldPlayDustParticles = (isFrontLeftSlipping || isFrontLeftDrifting || isFrontLeftBraking) ||
                                       (isFrontRightSlipping || isFrontRightDrifting || isFrontRightBraking) ||
                                       (isRearLeftSlipping || isRearLeftDrifting || isRearLeftBraking) ||
                                       (isRearRightSlipping || isRearRightDrifting || isRearRightBraking);

        SetDustParticleSystemState(frontLeftDustParticleSystem, shouldPlayDustParticles);
        SetDustParticleSystemState(frontRightDustParticleSystem, shouldPlayDustParticles);
        SetDustParticleSystemState(rearLeftDustParticleSystem, shouldPlayDustParticles);
        SetDustParticleSystemState(rearRightDustParticleSystem, shouldPlayDustParticles);

        // Calculate the target pitch based on the current speed and direction
        float targetPitch = currentSpeedKmph > 0.1f ? Mathf.Lerp(0.5f, 2f, currentSpeedKmph / 100f) : 0.5f;

        // Check if the vehicle is moving in reverse
        if (currentSpeedKmph < -0.1f)
        {
            targetPitch = Mathf.Lerp(0.5f, 2f, Mathf.Abs(currentSpeedKmph) / 100f);
        }
       
        // Smoothly adjust the pitch towards the target pitch
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, targetPitch, Time.deltaTime * 5f);


        // Play the engine start sound if the vehicle just starts moving
        if (!hasStartedMoving && currentSpeedKmph > 0.1f)
        {
            engineStartAudioSource.Play();
            hasStartedMoving = true;
        }
    }

    bool IsWheelSlipping(WheelCollider wheel)
    {
        WheelHit hit;
        return wheel.GetGroundHit(out hit) && hit.sidewaysSlip > 0.1f;
    }

    bool IsWheelDrifting(WheelCollider wheel)
    {
        WheelHit hit;
        return wheel.GetGroundHit(out hit) && hit.forwardSlip > 0.1f;
    }

    bool IsWheelBraking(WheelCollider wheel)
    {
        return wheel.isGrounded && Mathf.Abs(wheel.rpm) < 1f && wheel.brakeTorque > 0f;
    }

    void SetDustParticleSystemState(ParticleSystem dustParticleSystem, bool shouldPlay)
    {
        if (shouldPlay)
        {
            if (!dustParticleSystem.isPlaying)
            {
                dustParticleSystem.Play();
            }
        }
        else
        {
            if (dustParticleSystem.isPlaying)
            {
                dustParticleSystem.Stop();
            }
        }
    }

    void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheel, frontLeftWheelTransform);
        UpdateWheelPose(frontRightWheel, frontRightWheelTransform, true);
        UpdateWheelPose(rearLeftWheel, rearLeftWheelTransform);
        UpdateWheelPose(rearRightWheel, rearRightWheelTransform, true);
    }

    void UpdateWheelPose(WheelCollider collider, Transform transform, bool flip = false)
    {
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;

        collider.GetWorldPose(out pos, out quat);

        if (flip)
        {
            quat *= Quaternion.Euler(0, 180, 0);
        }

        transform.position = pos;
        transform.rotation = quat;
    }
}
