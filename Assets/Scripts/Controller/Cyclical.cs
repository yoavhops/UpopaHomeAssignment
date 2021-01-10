using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    public class Cyclical : MonoBehaviour
    {
        public GameObject Mirror;
        public Playground Playground;
        public bool IsMirrorObject;
        [field: SerializeField]
        public bool Show { get; private set; }
        private Vector3 height, width, margin, middle;
        private HashSet<Boundary> boundariesInContact = new HashSet<Boundary>();


        private void Awake()
        {
            Show = false;
            if (IsMirrorObject)
            {
                gameObject.name += "-Mirror";
            }
        }


        void Start()
        {
            if (!IsMirrorObject)
            {
                Mirror.SetActive(Show);
            }
            height = new Vector3(0, Playground.Size.y, 0);
            width = new Vector3(Playground.Size.x, 0, 0);
            margin = Playground.Margin;
            middle = Playground.Middle;
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
                    Debug.Log($"{gameObject.name} {string.Join(" ", boundariesInContact)}");
                    Mirror.SetActive(true);
                    Mirror.transform.position = MirrorPosition();
                    Mirror.transform.rotation = transform.rotation;
                }
                else
                {
                    Mirror.SetActive(false);
                }
            }
        }


        private bool UpdateBoundaries()
        {
            boundariesInContact.Clear();
            bool moveToMirror = false;
            Vector3 position = transform.position;

            if (position.x > middle.x + width.x / 2)
            {
                if (position.x > middle.x + width.x / 2 + margin.x)
                {
                    moveToMirror = true;
                }
                boundariesInContact.Add(Boundary.Right);
            }
            else if (position.x < middle.x - width.x / 2)
            {
                if (position.x > middle.x - width.x / 2 - margin.x)
                {
                    moveToMirror = true;
                }
                boundariesInContact.Add(Boundary.Left);
            }
            if (position.y > middle.y + height.y / 2)
            {
                if (position.y > middle.y + height.y / 2 + margin.y)
                {
                    moveToMirror = true;
                }
                boundariesInContact.Add(Boundary.Upper);
            }
            else if (position.y < middle.y - height.y / 2)
            {
                if (position.y < middle.y - height.y / 2 - margin.y)
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