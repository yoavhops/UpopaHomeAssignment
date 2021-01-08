using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    public class Cyclical : MonoBehaviour
    {
        public GameObject Mirror;
        public MeshRenderer Playground;
        public bool IsMirrorObject;
        [field: SerializeField]
        public bool Show { get; private set; }
        private Vector3 playgroundBox;
        private HashSet<Boundary> boundariesInContact = new HashSet<Boundary>();


        private void Awake()
        {
            Show = false;
        }


        void Start()
        {
            if (!IsMirrorObject)
            {
                Mirror.SetActive(Show);
                playgroundBox = Playground.bounds.size;
            }
        }


        void Update()
        {
            if (Show && !IsMirrorObject)
            {
                if (boundariesInContact.Count == 0 || boundariesInContact.Count > 2)
                {
                    throw new ArgumentOutOfRangeException($"Illeal amount of boundaries to mirror over the cyclical range: {boundariesInContact.Count}");
                }

                foreach (Boundary boundary in boundariesInContact)
                {
                    //Vector3 height = new Vector3(0, Screen.height, 0), width = new Vector3(Screen.width, 0, 0); Maybe?
                    Vector3 height = new Vector3(0, playgroundBox.y, 0), width = new Vector3(playgroundBox.x, 0, 0);
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
            if (!IsMirrorObject)
            {
                Border border = other.gameObject.GetComponent<Border>();
                if (border != null)
                {
                    if (boundariesInContact.Count >= 2)
                    {
                        switch (border.Boundary)
                        {
                            case Boundary.Left:
                                boundariesInContact.Remove(Boundary.Right);
                                break;
                            case Boundary.Right:
                                boundariesInContact.Remove(Boundary.Left);
                                break;
                            case Boundary.Upper:
                                boundariesInContact.Remove(Boundary.Lower);
                                break;
                            case Boundary.Lower:
                                boundariesInContact.Remove(Boundary.Upper);
                                break;
                        }
                    }
                    Mirror.SetActive(true);
                    Show = true;
                    boundariesInContact.Add(border.Boundary);
                }
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (!IsMirrorObject)
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
    }
}