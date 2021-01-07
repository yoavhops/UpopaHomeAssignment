using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : Shootable
{
    protected override void OnShot(Shot shot)
    {
        base.OnShot(shot);
        Loot();
    }

    private void Loot()
    {

    }
}
