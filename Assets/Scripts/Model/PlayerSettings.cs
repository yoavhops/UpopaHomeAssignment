using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Asteroids/PlayerSettings", order = 1)]
public class PlayerSettings : ScriptableObject
{
    [field: SerializeField]
    public float MovementSpeed { get; private set; }
    [field: SerializeField]
    public float RotationSpeed { get; private set; }
    [field: SerializeField]
    public float TorqueDrag { get; private set; }
    [field: SerializeField]
    public float TorqueClamp { get; private set; }
    [field: SerializeField]
    public float LifeLosePerSecond { get; private set; }
    [field: SerializeField]
    public float StartingLife { get; private set; }
    [field: SerializeField]
    public float LifeLoseIncreaseRate { get; private set; }
    [field: SerializeField]
    public float LifeLoseIncreaseTime { get; private set; }

    public void MockSettings(float movementSpeed = 3, float rotationSpeed = 50, float torqueDrag = 1.5f, float torqueClamp = 50)
    {
        MovementSpeed = movementSpeed;
        RotationSpeed = rotationSpeed;
        TorqueDrag = torqueDrag;
        TorqueClamp = torqueClamp;
    }
}
