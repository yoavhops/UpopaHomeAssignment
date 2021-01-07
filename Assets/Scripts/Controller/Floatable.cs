using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floatable : MonoBehaviour
{
    [SerializeField]
    private bool randomDirection;
    [SerializeField]
    private float floatSpeed;
    [SerializeField]
    private Vector3 direction;
    // Start is called before the first frame update
    void Awake()
    {
        if (randomDirection)
        {
            direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * Time.deltaTime;
    }
}
