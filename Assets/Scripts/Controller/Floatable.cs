using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floatable : MonoBehaviour
{
    public Vector3 Direction;
    [SerializeField]
    private bool randomDirection;
    [SerializeField]
    private float floatSpeed;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (randomDirection)
        {
            Direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Direction * Time.deltaTime;
    }
}
