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
        [Range(-100f, 100f)]
        private float torque;
        private float torqueDrag;

        public PlayerSimulation(ILocalTransformAdapter transformAdapter, PlayerSettings settings)
        {
            movementSpeed = settings.MovementSpeed;
            rotationSpeed = settings.RotationSpeed;
            torqueDrag = settings.TorqueDrag;
            transform = transformAdapter;
        }


        public void MoveForward(float time)
        {
            transform.LocalPosition = transform.LocalPosition + transform.Forward.normalized * movementSpeed * time;
        }


        public void Rotate(Vector3 direction, float time)
        {
            if (direction == Vector3.forward)
            {
                torque += time * rotationSpeed;
            }
            else if (direction == Vector3.back)
            {
                torque -= time * rotationSpeed;
            }
        }

        public void UpdateRotation(float time)
        {
            if (torque > 0)
            {
                transform.LocalRotation = Quaternion.AngleAxis(rotationSpeed * time, Vector3.forward) * transform.LocalRotation;
                torque = torque > time * rotationSpeed / torqueDrag ? torque - time * rotationSpeed / torqueDrag : 0;
            }
            else if (torque < 0)
            {
                transform.LocalRotation = Quaternion.AngleAxis(rotationSpeed * time, Vector3.back) * transform.LocalRotation;
                torque = torque < -time * rotationSpeed / torqueDrag ? torque + time * rotationSpeed / torqueDrag : 0;
            }
        }
    }
}