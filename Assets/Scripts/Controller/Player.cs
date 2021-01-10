using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
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
        private Cyclical cyclical;
        private bool isMirror;
        [SerializeField]
        private Shot shotGameObject;
        private float health = 1000;
        private int points = 0;
        private PlayerSimulation playerSimulation;
        private float lifeLose;
        private float timeUntilLifeLoseIncrease;


        void Start()
        {
            isMirror = false;
            cyclical = GetComponent<Cyclical>();
            if (cyclical != null && cyclical.IsMirrorObject)
            {
                //enabled = false;
                isMirror = true;
                return;
            }

            playerSimulation = new PlayerSimulation(this, PlayerSettings);
            Health = PlayerSettings.StartingLife;
            Points = 0;
            lifeLose = PlayerSettings.LifeLosePerSecond;
            timeUntilLifeLoseIncrease = PlayerSettings.LifeLoseIncreaseTime;
        }


        void Update()
        {
            if (!isMirror)
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

                playerSimulation.UpdateRotation(deltaTime);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Shoot();
                }

                timeUntilLifeLoseIncrease -= deltaTime;
                if (timeUntilLifeLoseIncrease < 0)
                {
                    lifeLose += PlayerSettings.LifeLoseIncreaseRate;
                    timeUntilLifeLoseIncrease += PlayerSettings.LifeLoseIncreaseTime;
                }
                Health -= deltaTime * lifeLose;
            }
        }


        public void Move(Vector3 direction, float time)
        {

        }


        public void Shoot()
        {

            Shot shot = Instantiate(shotGameObject, transform.position, transform.rotation);
            Floatable floatingShot = shot.GetComponent<Floatable>();
            floatingShot.Direction = transform.up;
            shot.FiredBy = this;

        }


        public void OnTriggerEnter(Collider other)
        {
            Shootable willKillMe = other.gameObject.GetComponent<Shootable>();
            if (willKillMe != null)
            {
                // Player Destroyed
                gameObject.SetActive(false);
                if (isMirror)
                {
                    Player actual = cyclical.Mirror.GetComponent<Player>();
                    actual.Health = 0;
                    cyclical.Mirror.SetActive(false);
                }
                else
                {
                    Health = 0;

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
}