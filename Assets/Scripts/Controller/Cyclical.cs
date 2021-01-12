using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    /// <summary>
    /// Generic component which adds cyclical behaviour to objects that
    /// allows circumnaviating a referenced Playground object.
    /// Using a Mirror field, the Cyclical component will make sure to update whichever
    /// component T stands for and update it's Mirror field as well as it's own.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cyclical<T> : MonoBehaviour where T : MonoBehaviour, ICyclic<T>
    {
        public T Mirror;
        public Playground Playground;
        public bool IsMirrorObject;
        [field: SerializeField]
        public bool Show { get; private set; }

        [SerializeField]
        private Vector3 offset;
        private Vector3 height, width, margin, middle;
        private HashSet<Boundary> boundariesInContact = new HashSet<Boundary>();


        private void Awake()
        {
            Show = false;
            if (IsMirrorObject)
            {
                gameObject.name += "-Mirror";
            }
            ICyclic<T> cyclic = GetComponent<ICyclic<T>>();
            cyclic.IsMirror = IsMirrorObject;
            cyclic.Mirror = Mirror;
        }


        void Start()
        {
            if (!IsMirrorObject)
            {
                Mirror.gameObject.SetActive(Show);
            }
            height = new Vector3(0, Playground.Size.y, 0);
            width = new Vector3(Playground.Size.x, 0, 0);
            margin = Playground.Margin;
            middle = Playground.Middle + offset;
        }


        void Update()
        {
            if (!IsMirrorObject)
            {
                if (UpdateBoundaries())
                {
                    transform.position = MirrorPosition();
                    Show = false;
                    boundariesInContact.Clear();
                }

                if (Show)
                {
                    if (boundariesInContact.Count == 0 || boundariesInContact.Count > 2)
                    {
                        throw new ArgumentOutOfRangeException($"Illeal amount of boundaries to mirror over the cyclical range: {boundariesInContact.Count}");
                    }
                    Mirror.gameObject.SetActive(true);
                    Mirror.transform.position = MirrorPosition();
                    Mirror.transform.rotation = transform.rotation;
                }
                else
                {
                    Mirror.gameObject.SetActive(false);
                }
            }
        }


        private bool UpdateBoundaries()
        {
            boundariesInContact.Clear();
            bool moveToMirror = false;
            Vector3 position = transform.position;

            if (position.x > middle.x + width.x / 2 + margin.x)
            {
                if (position.x > middle.x + width.x / 2)
                {
                    moveToMirror = true;
                }
                boundariesInContact.Add(Boundary.Right);
            }
            else if (position.x < middle.x - width.x / 2 - margin.x)
            {
                if (position.x < middle.x - width.x / 2)
                {
                    moveToMirror = true;
                }
                boundariesInContact.Add(Boundary.Left);
            }
            if (position.y > middle.y + height.y / 2 + margin.y)
            {
                if (position.y > middle.y + height.y / 2 )
                {
                    moveToMirror = true;
                }
                boundariesInContact.Add(Boundary.Upper);
            }
            else if (position.y < middle.y - height.y / 2 - margin.y)
            {
                if (position.y < middle.y - height.y / 2 )
                {
                    moveToMirror = true;
                }
                boundariesInContact.Add(Boundary.Lower);
            }
            Show = boundariesInContact.Count > 0;
            return moveToMirror;
        }


        private Vector3 MirrorPosition()
        {
            Vector3 position = transform.position;
            foreach (Boundary boundary in boundariesInContact)
            {
                switch (boundary)
                {
                    case Boundary.Upper:
                        position -= height;
                        break;
                    case Boundary.Lower:
                        position += height;
                        break;
                    case Boundary.Left:
                        position += width;
                        break;
                    case Boundary.Right:
                        position -= width;
                        break;
                    default:
                        throw new ArgumentException($"Illegal boundary");
                }
            }
            return position;
        }
    }
}