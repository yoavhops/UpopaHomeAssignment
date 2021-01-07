using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{

    public Cyclical Cyclical;
    // Start is called before the first frame update
    void Start()
    {
        Cyclical = GetComponent<Cyclical>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    virtual protected void OnShot(Shot shot)
    {

        gameObject.SetActive(false);
        if (Cyclical != null)
        {
            Cyclical.Mirror.SetActive(false);
        }
        return;
    }


    private void OnTriggerEnter(Collider other)
    {
        Shot shot = other.gameObject.GetComponent<Shot>();
        if (shot != null)
        {
            OnShot(shot);
        }

    }
}
