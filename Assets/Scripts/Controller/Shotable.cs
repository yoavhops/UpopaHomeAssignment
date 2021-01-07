using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerShot shot = other.gameObject.GetComponent<PlayerShot>();
        if (shot != null)
        {
            Destroy(gameObject);
            return;
        }

    }
}
