using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "VehicleSettings", menuName = "ScriptableObjects/VehicleSettings")]
public class VehicleSettings : ScriptableObject
{
    [BoxGroup("General Parameters")]
    [SerializeField, Range(0f, 100f)] public float currentAcceleration = 30f;

    [BoxGroup("General Parameters")]
    [SerializeField, Range(0f, 100f)] public float originalAcceleration = 30f;

    [BoxGroup("General Parameters")]
    [SerializeField, Range(0f, 100f)] public float steering = 80f;

    [BoxGroup("General Parameters")]
    [SerializeField, Range(0f, 50f)] public float gravity = 10f;

    [BoxGroup("General Parameters")]
    [SerializeField, Range(0.1f, 1f)] public float reverseMultiplier = 0.5f;

    [BoxGroup("General Parameters")]
    [SerializeField, Range(0.1f, 1f)] public float offTrackMultiplier = 0.5f;

    [BoxGroup("Track Settings")]
    [SerializeField] public LayerMask trackLayers;

    [BoxGroup("Turbo Settings")]
    [SerializeField] public int maxTurboCount = 3;

    [BoxGroup("Turbo Settings")]
    [SerializeField, Range(1f, 5f)] public float turboMultiplier = 2.0f;

    [BoxGroup("Turbo Settings")]
    [SerializeField, Range(0.5f, 5f)] public float turboDuration = 2.0f;

    [BoxGroup("Drift Settings")]
    [SerializeField] public bool canDrift = true;

    [BoxGroup("Drift Settings"), ShowIf(nameof(canDrift))]
    [SerializeField, Range(0.1f, 1f)] public float driftTimeScale = 0.2f;

    [BoxGroup("Boost Drift Multipliers"), ShowIf(nameof(canDrift))]
    [SerializeField] public float boostMultiplier1 = 1.5f;

    [BoxGroup("Boost Drift Multipliers"), ShowIf(nameof(canDrift))]
    [SerializeField] public float boostMultiplier2 = 2.0f;

    [BoxGroup("Boost Drift Multipliers"), ShowIf(nameof(canDrift))]
    [SerializeField] public float boostMultiplier3 = 2.5f;

    [BoxGroup("Drift Power Thresholds"), ShowIf(nameof(canDrift))]
    [SerializeField] public float driftPower1 = 50f;

    [BoxGroup("Drift Power Thresholds"), ShowIf(nameof(canDrift))]
    [SerializeField] public float driftPower2 = 100f;

    [BoxGroup("Drift Power Thresholds"), ShowIf(nameof(canDrift))]
    [SerializeField] public float driftPower3 = 150f;

    [BoxGroup("Boost Colors"), ShowIf(nameof(canDrift))]
    [SerializeField] public Color[] turboColors;
}