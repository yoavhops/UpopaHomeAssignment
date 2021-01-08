using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    public class Lootable : Shootable
    {
        public Transform RewardParent;
        [SerializeField]
        private Loot loot;


        public override void OnShot(Shot shot)
        {
            base.OnShot(shot);
            Loot(shot);
        }


        private void Loot(Shot shot)
        {
            foreach (var rewardPrefab in loot.rewards)
            {
                Reward reward = Instantiate(rewardPrefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, RewardParent);
                reward.Cause = shot;
            }
        }
    }
}