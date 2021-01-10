using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICyclic<T> where T : MonoBehaviour
{
    T Mirror { get; set; }
    bool IsMirror { get; set; }
    bool IsOppositeShown { get; set; }
}
