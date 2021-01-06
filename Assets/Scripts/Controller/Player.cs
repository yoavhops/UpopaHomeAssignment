using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ILocalTransformAdapter
{
    public Vector3 LocalPosition { get => transform.position; set => transform.position = value; }
    public Quaternion LocalRotation { get => transform.rotation; set => transform.rotation = value; }
    public Vector3 Forward { get => transform.up; }
    [field: SerializeField]
    public PlayerSettings PlayerSettings { get; private set; }
    private PlayerLogic playerLogic;
    private PlayerSimulation playerSimulation;


    // Start is called before the first frame update
    void Start()
    {
        playerSimulation = new PlayerSimulation(this, PlayerSettings);
        playerLogic = new PlayerLogic();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerSimulation.MoveForward(Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerSimulation.Rotate(Vector3.forward, Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerSimulation.Rotate(Vector3.back, Time.deltaTime);
        }
    }


    public void Move(Vector3 direction, float time)
    {

    }
}
