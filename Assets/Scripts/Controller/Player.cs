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
    [SerializeField]
    private Shot shotGameObject;
    [SerializeField]
    private HealthBar health;

    private PlayerSimulation playerSimulation;
    private Cyclical cyclical;
    // Start is called before the first frame update
    void Start()
    {
        cyclical = GetComponent<Cyclical>();
        if (cyclical != null && cyclical.IsMirrorObject)
        {
            enabled = false;
            return;
        }
        playerSimulation = new PlayerSimulation(this, PlayerSettings);

        health.BarValue = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        var deltaTime = Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
        {
            playerSimulation.MoveForward(deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerSimulation.Rotate(Vector3.forward, deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerSimulation.Rotate(Vector3.back, deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        health.BarValue -= deltaTime * PlayerSettings.LifeLosePerSecond / 10;
    }


    public void Move(Vector3 direction, float time)
    {

    }


    public void Shoot()
    {

        Shot shot = Instantiate(shotGameObject, transform.position, transform.rotation);
        Floatable floatingShot = shot.GetComponent<Floatable>();
        floatingShot.Direction = transform.up;

    }


    public void OnTriggerEnter(Collider other)
    {
        Shootable willKillMe = other.gameObject.GetComponent<Shootable>();
        if (willKillMe != null)
        {
            // Player Destroyed
            gameObject.SetActive(false);
            if (cyclical != null)
            {
                cyclical.Mirror.SetActive(false);
            }
        }
    }
}
