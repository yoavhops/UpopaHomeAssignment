using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    public class Explodable : Shootable
    {
        [SerializeField]
        private AsteroidSettings settings;


        public override void OnShot(Shot shot)
        {
            base.OnShot(shot);
            Explode(shot);
        }


        private void Explode(Shot shot)
        {
            var collidesWith = Physics.OverlapSphere(transform.position, settings.AsteroidExplosionRadius);
            for (int i = 0; i < collidesWith.Length; i++)
            {
                var shootable = collidesWith[i].gameObject.GetComponent<Shootable>();
                if (shootable != null)
                {
                    shootable.OnShot(shot);
                }
            }
        }
    }
}