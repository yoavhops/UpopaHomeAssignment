using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointReward : Reward
{
    [field: SerializeField]
    public int PointsAward { get; private set; }
    [field: SerializeField]
    public Player DroppedFor { get; set; }
    public override void Award()
    {
        DroppedFor.AwardPoints(this);
        Destroy(gameObject);
    }

    
}
