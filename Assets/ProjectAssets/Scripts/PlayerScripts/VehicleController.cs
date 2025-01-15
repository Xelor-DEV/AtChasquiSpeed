using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

public class VehicleController : RaceTracker
{
    [Expandable]
    [BoxGroup("Settings")]
    [SerializeField] private VehicleSettings settings;

    [BoxGroup("Debug Settings")]
    [SerializeField] private bool showDebugVariables = false; //Show debug variables

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float timeScale;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float speed;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float currentSpeed;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float rotate;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float currentRotate;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private int driftDirection;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float driftPower;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private int driftMode = 0;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool first;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool second;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool third;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private Color c;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool isOffTrack = false;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool drifting;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private int currentTurboCount;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool isTurboActive;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float turboEndTime;

    //Input Variables

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private float horizontal;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool drift;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool accelerate;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool turbo;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool brake;

    [ShowIf("showDebugVariables"), BoxGroup("Debug Variables")]
    [SerializeField] private bool reverse;

    [Foldout("References")]
    [SerializeField] private Transform vehicleModel;

    [Foldout("References")]
    [SerializeField] private Transform vehicleNormal;

    [Foldout("References")]
    [SerializeField] private Rigidbody sphere;

    [Foldout("Model Parts")]
    [SerializeField] private Transform frontWheels;

    [Foldout("Model Parts")]
    [SerializeField] private Transform backWheels;

    [Foldout("Particles")]
    [SerializeField] private Transform leftWheelParticles;

    [Foldout("Particles")]
    [SerializeField] private Transform rightWheelParticles;

    [Foldout("Particles")]
    [SerializeField] private Transform flashParticles;

    [Foldout("Particles")]
    [SerializeField] private Transform leftTurboExhaust;

    [Foldout("Particles")]
    [SerializeField] private Transform rightTurboExhaust;

    [Foldout("Skid Marks")]
    [SerializeField] private TrailRenderer[] skidMarks;

    [Foldout("Skid Marks")]
    [SerializeField] private ParticleSystem[] skidSmoke;

    [Foldout("Other References")]
    [SerializeField] private CinemachineImpulseSource cameraImpulse;

    private List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    private List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();

    //Input Methods
    public void GetMovementInput(InputAction.CallbackContext horizontalAxis)
    {
        horizontal = horizontalAxis.ReadValue<float>();
    }

    public void GetAccelerateInput(InputAction.CallbackContext accelerateButton)
    {
        if (accelerateButton.phase == InputActionPhase.Performed)
        {
            accelerate = true;
        }
        else if (accelerateButton.phase == InputActionPhase.Canceled)
        {
            accelerate = false;
        }
    }

    public void GetDriftInput(InputAction.CallbackContext driftButton)
    {
        if (driftButton.phase == InputActionPhase.Performed)
        {
            drift = true;
        }
        else if (driftButton.phase == InputActionPhase.Canceled)
        {
            drift = false;
        }
    }

    public void GetTurboInput(InputAction.CallbackContext turboButton)
    {
        if (turboButton.phase == InputActionPhase.Performed && currentTurboCount > 0 && isTurboActive == false && reverse == false)
        {
            ActivateTurbo();
        }
        else if (turboButton.phase == InputActionPhase.Canceled)
        {
            turbo = false;
        }
    }

    public void GetBrakeInput(InputAction.CallbackContext brakeButton)
    {
        if (brakeButton.phase == InputActionPhase.Performed)
        {
            brake = true;
            Brake();
        }
        else if (brakeButton.phase == InputActionPhase.Canceled)
        {
            brake = false;
            if (isTurboActive == false && isOffTrack == false)
            {
                RestoreAcceleration();
            }
        }
    }

    public void GetReverseInput(InputAction.CallbackContext reverseButton)
    {
        if (reverseButton.phase == InputActionPhase.Performed)
        {
            reverse = true;
        }
        else if (reverseButton.phase == InputActionPhase.Canceled)
        {
            reverse = false;
        }
    }

    //Unity Methods
    private void Start()
    {
        currentTurboCount = settings.maxTurboCount;

        for (int i = 0; i < leftWheelParticles.childCount; ++i)
        {
            primaryParticles.Add(leftWheelParticles.GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < rightWheelParticles.childCount; ++i)
        {
            primaryParticles.Add(rightWheelParticles.GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < flashParticles.childCount; ++i)
        {
            secondaryParticles.Add(flashParticles.GetChild(i).GetComponent<ParticleSystem>());
        }
        Debug.Log(transform.parent);
    }

    private void Update()
    {
        timeScale = Time.timeScale;
        //Adjust the time scale only when the drift status changes
        if (settings.canDrift == true && drift == true && Time.timeScale != settings.driftTimeScale)
        {
            Time.timeScale = settings.driftTimeScale;
        }
        else if (drift == false && Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        //Accelerate or Reverse
        if ((accelerate == true || reverse == true) && brake == false)
        {
            if (reverse == true)
            {
                Reverse();
            }
            else
            {
                speed = settings.currentAcceleration;
            }
        }

        //Turbo
        if (isTurboActive == true)
        {
            if (Time.time > turboEndTime)
            {
                DeactivateTurbo();
            }
        }

        //Steer
        if (horizontal != 0 && brake == false)
        {
            int dir;
            if (horizontal > 0)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }
            float amount = Mathf.Abs(horizontal);
            Steer(dir, amount);
        }

        //Drift
        if (settings.canDrift == true && drift == true && drifting == false && horizontal != 0 && brake == false && reverse == false)
        {
            drifting = true;
            if (horizontal > 0)
            {
                driftDirection = 1;
            }
            else
            {
                driftDirection = -1;
            }

            for (int i = 0; i < primaryParticles.Count; ++i)
            {
                ParticleSystem.MainModule mainModule = primaryParticles[i].main;
                mainModule.startColor = Color.clear;
                primaryParticles[i].Play();
            }

            vehicleModel.parent.DOComplete();
            vehicleModel.parent.DOPunchPosition(transform.up * .2f, .3f, 5, 1);
        }

        if (drifting == true)
        {
            float control;
            if (driftDirection == 1)
            {
                control = ExtensionMethods.Remap(horizontal, -1, 1, 0, 2);
            }
            else
            {
                control = ExtensionMethods.Remap(horizontal, -1, 1, 2, 0);
            }
            float powerControl;
            if (driftDirection == 1)
            {
                powerControl = ExtensionMethods.Remap(horizontal, -1, 1, .2f, 1);
            }
            else
            {
                powerControl = ExtensionMethods.Remap(horizontal, -1, 1, 1, .2f);
            }
            Steer(driftDirection, control);
            driftPower += powerControl * (1 / settings.driftTimeScale); //Adjust the driftPower accumulation

            ColorDrift();
        }

        if (drift == false && drifting == true)
        {
            Boost();
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //Animations    

        //a) Kart
        if (drifting == false)
        {
            vehicleModel.localEulerAngles = Vector3.Lerp(vehicleModel.localEulerAngles, new Vector3(0, 90 + (horizontal * 15), vehicleModel.localEulerAngles.z), .2f);
        }
        else
        {
            float control;
            if (driftDirection == 1)
            {
                control = ExtensionMethods.Remap(horizontal, -1, 1, .5f, 2);
            }
            else
            {
                control = ExtensionMethods.Remap(horizontal, -1, 1, 2, .5f);
            }
            vehicleModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(vehicleModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
        }

        //b) Wheels
        frontWheels.localEulerAngles = new Vector3(0, (horizontal * 15), frontWheels.localEulerAngles.z);
        frontWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude);
        backWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        //Forward Acceleration
        if (drifting == false && brake == false)
        {
            sphere.AddForce(-vehicleModel.transform.right * currentSpeed, ForceMode.Acceleration);
        }
        else if (brake == false)
        {
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        }

        //Gravity
        sphere.AddForce(Vector3.down * settings.gravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        bool hitOnTrack = Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f);
        bool hitNearTrack = Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f);

        if (hitOnTrack == true)
        {
            //Verify if the layer of the struck object is in the track layers
            if ((settings.trackLayers.value & (1 << hitOn.collider.gameObject.layer)) != 0)
            {
                //The vehicle is on the track
                if (isOffTrack == true)
                {
                    ApplyOffTrackPenalty();
                }
            }
            else
            {
                //The vehicle is off the track
                if (isOffTrack == false)
                {
                    RestoreTrackSpeed();
                }
            }
        }

        vehicleNormal.up = Vector3.Lerp(vehicleNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        vehicleNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    //Custom Methods
    private void ActivateTurbo()
    {
        if (isTurboActive == false && currentTurboCount > 0)
        {
            isTurboActive = true;
            turboEndTime = Time.time + settings.turboDuration;
            currentTurboCount--;

            //Directly multiplies the acceleration
            settings.currentAcceleration = settings.currentAcceleration * settings.turboMultiplier;

            //Activate turbo particles
            ParticleSystem.MainModule rightTurboMain = rightTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
            rightTurboMain.loop = true;
            rightTurboExhaust.GetComponentInChildren<ParticleSystem>().Play();

            ParticleSystem.MainModule leftTurboMain = leftTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
            leftTurboMain.loop = true;
            leftTurboExhaust.GetComponentInChildren<ParticleSystem>().Play();
        }
    }

    private void DeactivateTurbo()
    {
        if (isTurboActive == true && Time.time > turboEndTime)
        {
            isTurboActive = false;
            settings.currentAcceleration = settings.currentAcceleration / settings.turboMultiplier; //Restore base acceleration

            //Stop turbo particles
            ParticleSystem.MainModule rightTurboMain = rightTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
            rightTurboMain.loop = false;
            rightTurboExhaust.GetComponentInChildren<ParticleSystem>().Stop();

            ParticleSystem.MainModule leftTurboMain = leftTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
            leftTurboMain.loop = false;
            leftTurboExhaust.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    private void ApplyOffTrackPenalty()
    {
        settings.currentAcceleration = settings.currentAcceleration / settings.offTrackMultiplier;
        isOffTrack = false;
    }

    private void RestoreTrackSpeed()
    {
        settings.currentAcceleration = settings.currentAcceleration * settings.offTrackMultiplier;
        isOffTrack = true;
    }

    private void RestoreAcceleration()
    {
        settings.currentAcceleration = settings.originalAcceleration;
    }

    public void Boost()
    {
        drifting = false;

        if (driftMode > 0)
        {
            float boostMultiplier = 1.0f;
            switch (driftMode)
            {
                case 1:
                    boostMultiplier = settings.boostMultiplier1;
                    break;
                case 2:
                    boostMultiplier = settings.boostMultiplier2;
                    break;
                case 3:
                    boostMultiplier = settings.boostMultiplier3;
                    break;
            }

            DOVirtual.Float(currentSpeed * boostMultiplier, currentSpeed, .3f * driftMode, Speed);
            rightTurboExhaust.GetComponentInChildren<ParticleSystem>().Play();
            leftTurboExhaust.GetComponentInChildren<ParticleSystem>().Play();
        }

        driftPower = 0;
        driftMode = 0;
        first = false;
        second = false;
        third = false;

        for (int i = 0; i < primaryParticles.Count; ++i)
        {
            ParticleSystem.MainModule mainModule = primaryParticles[i].main;
            mainModule.startColor = Color.clear;
            primaryParticles[i].Stop();
        }

        vehicleModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);

    }

    public void Steer(int direction, float amount)
    {
        rotate = (settings.steering * direction) * amount;
    }

    public void ColorDrift()
    {
        if (first == false)
        {
            c = Color.clear;
        }

        if (driftPower > settings.driftPower1 && driftPower < settings.driftPower2 - 1 && first == false)
        {
            first = true;
            c = settings.turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
        }

        if (driftPower > settings.driftPower2 && driftPower < settings.driftPower3 - 1 && second == false)
        {
            second = true;
            c = settings.turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);
        }

        if (driftPower > settings.driftPower3 && third == false)
        {
            third = true;
            c = settings.turboColors[2];
            driftMode = 3;

            PlayFlashParticle(c);
        }

        for (int i = 0; i < primaryParticles.Count; ++i)
        {
            ParticleSystem.MainModule mainModule = primaryParticles[i].main;
            mainModule.startColor = c;
        }

        for (int i = 0; i < secondaryParticles.Count; ++i)
        {
            ParticleSystem.MainModule mainModule = secondaryParticles[i].main;
            mainModule.startColor = c;
        }
    }

    private void PlayFlashParticle(Color c)
    {
        cameraImpulse.GenerateImpulse();

        for (int i = 0; i < secondaryParticles.Count; ++i)
        {
            ParticleSystem.MainModule mainModule = secondaryParticles[i].main;
            mainModule.startColor = c;
            secondaryParticles[i].Play();
        }
    }

    private void Speed(float x)
    {
        currentSpeed = x;
    }

    private void Brake()
    {
        settings.currentAcceleration = 0f;
    }

    private void Reverse()
    {
        speed = -settings.currentAcceleration * settings.reverseMultiplier;
    }
}