using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
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
        Shootable shotable = other.gameObject.GetComponent<Shootable>();
        if (shotable != null)
        {
            Destroy(gameObject);    

        }
    }
}
