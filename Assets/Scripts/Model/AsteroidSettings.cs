using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AsteroidSettings", menuName = "Asteroids/AsteroidSettings", order = 1)]
public class AsteroidSettings : ScriptableObject
{
    [field: SerializeField]
    public List<Shootable> AsteroidObjects { get; private set; }
    [field: SerializeField]
    public List<int> SpawnProbabilities { get; private set; }
}
