using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supersonic
{
    public class Shot : MonoBehaviour
    {
        public Player FiredBy;
        

        private void OnTriggerEnter(Collider other)
        {
            Shootable shootable = other.gameObject.GetComponent<Shootable>();
            if (shootable != null)
            {
                Destroy(gameObject);
                return;
            }
            Border border = other.gameObject.GetComponent<Border>();
            if (border != null)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}