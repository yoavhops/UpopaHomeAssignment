using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supersonic
{
    public class Shootable : MonoBehaviour
    {
        public delegate void ShootableShot(Shootable wasShot, Shot shot);
        public event ShootableShot OnShotEvent;
        public Cyclical Cyclical;
        protected bool IsMirror;

        void Start()
        {
            Cyclical = GetComponent<Cyclical>();
            IsMirror = Cyclical != null && Cyclical.IsMirrorObject;
        }


        virtual public void OnShot(Shot shot)
        {
            OnShotEvent?.Invoke(this, shot);
            return;
        }



        virtual public void WasShot(Shot shot)
        {
            gameObject.SetActive(false);
            OnShot(shot);
            if (Cyclical != null && Cyclical.gameObject.activeSelf)
            {
                Cyclical.Mirror.GetComponent<Shootable>().OnShot(shot);
                Cyclical.Mirror.SetActive(false);
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