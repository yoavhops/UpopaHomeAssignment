using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cyclical : MonoBehaviour
{
    public GameObject Mirror;
    public MeshRenderer Playground;
    public bool Show;
    private Vector3 playgroundBox;
    private List<Boundary> boundariesInContact = new List<Boundary>();

    private void Awake()
    {
        Show = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        Mirror.SetActive(Show);
    }

    // Update is called once per frame
    void Update()
    {
        if (Show)
        {
            for (int i = 0; i < boundariesInContact.Count; i++)
            {
                var boundary = boundariesInContact[i];

                //Vector3 height = new Vector3(0, Screen.height, 0), width = new Vector3(Screen.width, 0, 0); Maybe?
                Vector3 height = new Vector3(0, Playground.bounds.size.y, 0), width = new Vector3(Playground.bounds.size.x, 0, 0);
                switch (boundary)
                {
                    case Boundary.Upper:
                        Mirror.transform.position = transform.position - height;
                        break;

                    case Boundary.Lower:
                        Mirror.transform.position = transform.position + height;
                        break;
                    case Boundary.Left:
                        Mirror.transform.position = transform.position + width;
                        break;

                    case Boundary.Right:
                        Mirror.transform.position = transform.position - width;
                        break;

                    default:
                        throw new ArgumentException($"Illegal boundary");
                }
            }
            Mirror.transform.rotation = transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Border border = other.gameObject.GetComponent<Border>();
        if (border == null) return;
        Mirror.SetActive(true);
        Show = true;
        boundariesInContact.Add(border.Boundary);


    }


    private void OnTriggerExit(Collider other)
    {
        Border border = other.gameObject.GetComponent<Border>();
        if (border == null) return;

        boundariesInContact.Remove(border.Boundary);
        if (boundariesInContact.Count == 0)
        {
            Mirror.SetActive(false);
            Show = false;
            transform.position = Mirror.transform.position;
        }


    }
}
