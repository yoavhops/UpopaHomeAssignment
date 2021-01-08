﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    public enum Boundary { Upper, Lower, Left, Right }
    public class Border : MonoBehaviour
    {
        [field: SerializeField]
        public Boundary Boundary { get; private set; }
    }
}