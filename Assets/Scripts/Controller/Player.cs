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
    public delegate void PointsChanged(int points);
    public delegate void HealthChanged(float health);
    public event PointsChanged PointsChangedEvent;
    public event HealthChanged HealthChangedEvent;
    public int Points { get => points; set { points = value; PointsChangedEvent(points); } }
    public float Health { get => health; set { health = value; HealthChangedEvent(health); } }
    [SerializeField]
    private Shot shotGameObject;
    private float health = 1000;
    private int points = 0;

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

        Health -= deltaTime * PlayerSettings.LifeLosePerSecond / 10;
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

    public void AwardPoints(PointReward reward)
    {
        Points += reward.PointsAward;
    }

    public void AwardHealth(HealthReward reward)
    {
        Health += reward.HealthAward;
    }
}
