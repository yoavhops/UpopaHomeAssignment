using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Supersonic
{
    public enum GameState { Start, Run, Pause, Over }
    public class Asteroids : MonoBehaviour
    {
        public GameState State { get; private set; }
        public AsteroidSettings Settings;
        [field: SerializeField]
        public List<Player> Players { get; private set; }
        public MeshRenderer Playground;

        private Vector3 playgroundBox;
        [SerializeField]
        private Transform rewardParent;
        [SerializeField]
        private LootablePool lootables;
        [SerializeField]
        private ExplodablePool explodables;
        private float timeUntilNextSpawn;


        void Start()
        {
            if (!Settings.Assert(out string error))
            {
                throw new ArgumentException($"Asteroid settings error: {error}.");
            }

            State = GameState.Run;
            timeUntilNextSpawn = Settings.AsteroidSpawnRate;
            playgroundBox = Playground.bounds.size;

            foreach (var shootable in transform.GetComponentsInChildren<Shootable>())
            {
                SetupShootable(shootable, false);
            }

            foreach (var player in Players)
            {
                player.HealthChangedEvent += (health) =>
                {
                    if (health <= 0)
                    {
                        GameOverForPlayer(player);
                    }
                };
            }
        }


        void Update()
        {
            if (State == GameState.Run)
            {
                if (Settings.SpawnAsteroid)
                {
                    timeUntilNextSpawn -= Time.deltaTime;
                    if (timeUntilNextSpawn < 0)
                    {
                        Spawn();
                        timeUntilNextSpawn += Settings.AsteroidSpawnRate;
                    }
                }
            }
        }


        private void Spawn()
        {
            Shootable asteroidPrefab = Settings.RandomShootable();
            if (asteroidPrefab == null)
            {
                return;
            }
            Vector3 position = new Vector3(Random.Range(0, playgroundBox.x), Random.Range(0, playgroundBox.y), Random.Range(0, 0));
            SetupShootable(asteroidPrefab).transform.position = position;
        }


        private Shootable SetupShootable(Shootable shootable, bool deploy = true)
        {
            switch (shootable)
            {
                case Lootable lootable:
                    if (deploy)
                    {
                        lootable = lootables.Deploy();
                        Cyclical cylic = lootable.GetComponent<Cyclical>();
                        if (cylic != null)
                        {
                            cylic.IsMirrorObject = false;
                            cylic.Playground = Playground;
                            Lootable mirror = lootables.Deploy();
                            cylic.Mirror = mirror.gameObject;
                            Cyclical mirrorCyclic = mirror.GetComponent<Cyclical>();
                            mirrorCyclic.Mirror = lootable.gameObject;
                            mirrorCyclic.IsMirrorObject = true;
                            mirrorCyclic.Playground = Playground;
                            mirror.OnShotEvent += (wasShot, shot) =>
                            {
                                lootables.Undeploy(wasShot as Lootable);
                            };
                            mirror.RewardParent = rewardParent;
                        }
                    }
                    else
                    {
                        lootables.AddDeployed(lootable);
                    }
                    lootable.RewardParent = rewardParent;
                    lootable.OnShotEvent += (wasShot, shot) =>
                    {
                        lootables.Undeploy(wasShot as Lootable);
                    };
                    return lootable;

                case Explodable explodable:
                    if (deploy)
                    {
                        explodable = explodables.Deploy();
                        Cyclical cylic = explodable.GetComponent<Cyclical>();
                        if (cylic != null)
                        {
                            cylic.Playground = Playground;
                            cylic.IsMirrorObject = false;
                            Explodable mirror = explodables.Deploy();
                            cylic.Mirror = mirror.gameObject;
                            Cyclical mirrorCyclic = mirror.GetComponent<Cyclical>();
                            mirrorCyclic.Mirror = explodable.gameObject;
                            mirrorCyclic.IsMirrorObject = true;
                            mirrorCyclic.Playground = Playground;
                            mirror.OnShotEvent += (wasShot, shot) =>
                            {
                                explodables.Undeploy(wasShot as Explodable);
                            };
                        }
                    }
                    else
                    {
                        explodables.AddDeployed(explodable);
                    }
                    explodable.OnShotEvent += (wasShot, shot) =>
                    {
                        explodables.Undeploy(wasShot as Explodable);
                    };
                    return explodable;

                default:
                    throw new ArgumentException($"Unreconized type of Shootable.");
            }
        }

        private void GameOverForPlayer(Player lost)
        {
            State = GameState.Over;
        }
    }
    public class Explodables : Cache<Explodable> { }
}