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
        

        void Start()
        {
            Cyclical = GetComponent<Cyclical>();
        }


        virtual public void OnShot(Shot shot)
        {
            gameObject.SetActive(false);
            if (Cyclical != null && Cyclical.gameObject.activeSelf)
            {
                Cyclical.Mirror.GetComponent<Shootable>().OnShot(shot);
                Cyclical.Mirror.SetActive(false);
            }
            OnShotEvent?.Invoke(this, shot);
            return;
        }


        private void OnTriggerEnter(Collider other)
        {
            Shot shot = other.gameObject.GetComponent<Shot>();
            if (shot != null)
            {
                OnShot(shot);
            }
        }
    }
}