using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

public class VehicleController : MonoBehaviour
{
    [SerializeField] private Transform vehicleModel;
    [SerializeField] private Transform vehicleNormal;
    [SerializeField] private Rigidbody sphere;

    private List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    private List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();

    private float speed;
    private float currentSpeed;
    private float rotate;
    private float currentRotate;
    private int driftDirection;
    private float driftPower;
    private int driftMode = 0;
    private bool first;
    private bool second;
    private bool third;
    private Color c;

    //Input Variables
    private float horizontal;
    private bool drift;
    private bool accelerate;
    private bool turbo;

    [Header("Bools")]
    [SerializeField] private bool drifting;

    [Header("Parameters")]
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float steering = 80f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField, Range(0.1f, 1f)] private float driftTimeScale = 0.2f;

    [Header("Boost Drift Multipliers")]
    [SerializeField] private float boostMultiplier1 = 1.5f;
    [SerializeField] private float boostMultiplier2 = 2.0f;
    [SerializeField] private float boostMultiplier3 = 2.5f;

    [Header("Turbo Settings")]
    [SerializeField] private int maxTurboCount = 3;
    [SerializeField] private float turboMultiplier = 2.0f;
    [SerializeField] private float turboDuration = 2.0f;

    private int currentTurboCount;
    private bool isTurboActive;
    private float turboEndTime;

    [Header("Model Parts")]
    [SerializeField] private Transform frontWheels;
    [SerializeField] private Transform backWheels;

    [Header("Particles")]
    [SerializeField] private Transform leftWheelParticles;
    [SerializeField] private Transform rightWheelParticles;
    [SerializeField] private Transform flashParticles;
    [SerializeField] private Transform leftTurboExhaust;
    [SerializeField] private Transform rightTurboExhaust;
    [SerializeField] private Color[] turboColors;

    [Header("Other References")]
    [SerializeField] private CinemachineImpulseSource cameraImpulse;

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
        if (turboButton.phase == InputActionPhase.Performed && currentTurboCount > 0)
        {
            ActivateTurbo();
        }
        else if (turboButton.phase == InputActionPhase.Canceled)
        {
            turbo = false;
        }
    }

    //Unity Methods
    private void Start()
    {
        currentTurboCount = maxTurboCount;

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
    }

    private void Update()
    {
        // Ajustar el time scale solo cuando el estado de drift cambia
        if (drift == true && Time.timeScale != driftTimeScale)
        {
            Time.timeScale = driftTimeScale;
        }
        else if (drift == false && Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        //Accelerate
        if (accelerate == true)
        {
            speed = acceleration;
        }

        //Turbo
        if (isTurboActive == true)
        {
            if (Time.time > turboEndTime)
            {
                isTurboActive = false;
                currentSpeed /= turboMultiplier;

                // Desactivar el loop y detener las partículas de turbo
                ParticleSystem.MainModule rightTurboMain = rightTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
                rightTurboMain.loop = false;
                rightTurboExhaust.GetComponentInChildren<ParticleSystem>().Stop();

                ParticleSystem.MainModule leftTurboMain = leftTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
                leftTurboMain.loop = false;
                leftTurboExhaust.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }




        //Steer
        if (horizontal != 0)
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
        if (drift == true && drifting == false && horizontal != 0)
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
            driftPower += powerControl * (1 / driftTimeScale); // Ajustar la acumulación de driftPower

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
        frontWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        backWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);

    }

    private void FixedUpdate()
    {
        //Forward Acceleration
        if (drifting == false)
        {
            sphere.AddForce(-vehicleModel.transform.right * currentSpeed, ForceMode.Acceleration);
        }
        else
        {
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        }

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        vehicleNormal.up = Vector3.Lerp(vehicleNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        vehicleNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    //Custom Methods
    private void ActivateTurbo()
    {
        if (isTurboActive == false)
        {
            isTurboActive = true;
            turboEndTime = Time.time + turboDuration;
            currentSpeed *= turboMultiplier;
            currentTurboCount--;

            // Activar partículas de turbo
            ParticleSystem.MainModule rightTurboMain = rightTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
            rightTurboMain.loop = true;
            rightTurboExhaust.GetComponentInChildren<ParticleSystem>().Play();

            ParticleSystem.MainModule leftTurboMain = leftTurboExhaust.GetComponentInChildren<ParticleSystem>().main;
            leftTurboMain.loop = true;
            leftTurboExhaust.GetComponentInChildren<ParticleSystem>().Play();
        }
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
                    boostMultiplier = boostMultiplier1;
                    break;
                case 2:
                    boostMultiplier = boostMultiplier2;
                    break;
                case 3:
                    boostMultiplier = boostMultiplier3;
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
        rotate = (steering * direction) * amount;
    }

    public void ColorDrift()
    {
        if (first == false)
        {
            c = Color.clear;
        }

        if (driftPower > 50 && driftPower < 100 - 1 && first == false)
        {
            first = true;
            c = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
        }

        if (driftPower > 100 && driftPower < 150 - 1 && second == false)
        {
            second = true;
            c = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);
        }

        if (driftPower > 150 && third == false)
        {
            third = true;
            c = turboColors[2];
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
}