using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : Shootable
{

    protected override void OnShot(Shot shot)
    {
        base.OnShot(shot);
        Explode();
    }

    private void Explode()
    {
        
    }
}
