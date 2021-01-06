using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocalTransformAdapter 
{
    Vector3 LocalPosition { get; set; }
    Quaternion LocalRotation { get; set; }
    Vector3 Forward { get; }
}
