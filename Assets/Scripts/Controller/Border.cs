using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boundary { Upper, Lower, Left, Right }
public class Border : MonoBehaviour
{
    [field: SerializeField]
    public Boundary Boundary { get; private set;  }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
