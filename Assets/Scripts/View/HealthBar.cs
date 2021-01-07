using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : ProgressBar
{
    protected override void UpdateValue(float val)
    {
        base.UpdateValue(val);
        txtTitle.text = Title + " " + val * 10;
    }
}
