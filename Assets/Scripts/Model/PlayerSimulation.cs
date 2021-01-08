using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    public class PlayerSimulation
    {
        private ILocalTransformAdapter transform;
        private float movementSpeed;
        private float rotationSpeed;
        private Vector3 torque;


        public PlayerSimulation(ILocalTransformAdapter transformAdapter, PlayerSettings settings)
        {
            movementSpeed = settings.MovementSpeed;
            rotationSpeed = settings.RotationSpeed;
            transform = transformAdapter;
        }


        public void MoveForward(float time)
        {
            transform.LocalPosition = transform.LocalPosition + transform.Forward.normalized * movementSpeed * time;
        }


        public void Rotate(Vector3 direction, float time)
        {
            torque += direction * time;
            transform.LocalRotation = Quaternion.AngleAxis(rotationSpeed * time, direction) * transform.LocalRotation;
        }
    }
}