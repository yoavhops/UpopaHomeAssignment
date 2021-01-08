using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthReward : Reward
{
    [field: SerializeField]
    public float HealthAward { get; private set; }
    [field: SerializeField]
    public Player DroppedFor { get; set; }
    public override void Award()
    {
        DroppedFor.AwardHealth(this);
    }

    
}
