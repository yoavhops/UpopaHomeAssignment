using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supersonic
{
    /// <summary>
    /// Base class to add to anything that might be shot.
    /// Provides basic event methods which can be easily overriden.
    /// </summary>
    public class Shootable : MonoBehaviour, ICyclic<Shootable>
    {
        public delegate void ShootableShot(Shootable wasShot, Shot shot);
        public event ShootableShot OnShotEvent;
        public Shootable Mirror { get; set; }
        public bool IsMirror { get; set; }
        public bool IsOppositeShown { get; set; }


        virtual public void OnShot(Shot shot)
        {
            OnShotEvent?.Invoke(this, shot);
            return;
        }


        virtual public void WasShot(Shot shot)
        {
            gameObject.SetActive(false);
            OnShot(shot);
            if (IsOppositeShown)
            {
                Mirror.OnShot(shot);
                Mirror.gameObject.SetActive(false);
            }
            
        }


        private void OnTriggerEnter(Collider other)
        {
            Shot shot = other.gameObject.GetComponent<Shot>();
            if (shot != null)
            {
                WasShot(shot);
            }
        }
    }
}