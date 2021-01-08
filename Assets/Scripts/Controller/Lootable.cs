using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supersonic
{
    public class Lootable : Shootable
    {
        [SerializeField]
        private Asteroids asteroids;
        public Loot loot;
        protected override void OnShot(Shot shot)
        {
            base.OnShot(shot);
            Loot();
        }

        private void Loot()
        {
            foreach (var reward in loot.rewards)
            {
                Instantiate(reward, transform.position, transform.rotation, asteroids.transform);
            }
        }
    }
}