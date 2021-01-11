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
        public Playground Playground;

        [SerializeField]
        private Storage storage;
        private IStorageStrategy disk;
        [SerializeField]
        private Transform rewardParent;
        [SerializeField]
        private ShotPool shots;
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

            foreach (var shootable in transform.GetComponentsInChildren<Shootable>())
            {
                SetupShootable(shootable, false);
            }

            foreach (var player in Players)
            {
                player.ShotsPool = shots;
                player.HealthChangedEvent += (health) =>
                {
                    if (health <= 0)
                    {
                        GameOverForPlayer(player);
                    }
                };
            }

            disk = storage.SelectedStorage;
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
            Vector3 position = new Vector3(Random.Range(0, Playground.Size.x), Random.Range(0, Playground.Size.y), Random.Range(0, 0));
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
                        Cyclical<Shootable> cylic = lootable.GetComponent<Cyclical<Shootable>>();
                        if (cylic != null)
                        {
                            cylic.IsMirrorObject = false;
                            cylic.Playground = Playground;
                            Lootable mirror = lootables.Deploy();
                            cylic.Mirror = mirror;
                            Cyclical<Shootable> mirrorCyclic = mirror.GetComponent<Cyclical<Shootable>>();
                            mirrorCyclic.Mirror = lootable;
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
                        Cyclical<Shootable> cylic = explodable.GetComponent<Cyclical<Shootable>>();
                        if (cylic != null)
                        {
                            cylic.Playground = Playground;
                            cylic.IsMirrorObject = false;
                            Explodable mirror = explodables.Deploy();
                            cylic.Mirror = mirror;
                            Cyclical<Shootable> mirrorCyclic = mirror.GetComponent<Cyclical<Shootable>>();
                            mirrorCyclic.Mirror = explodable;
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


        public void SaveGame()
        {
            disk.SetBool("saved", true);
            disk.SetInt("players", Players.Count);
            for (int i = 0; i < Players.Count; i++)
            {
                Player player = Players[i];
                disk.SetFloat($"player{i}-health", player.Health);
                disk.SetFloat($"player{i}-points", player.Points);
                // position
                disk.SetFloat($"player{i}-positionX", player.transform.position.x);
                disk.SetFloat($"player{i}-positionY", player.transform.position.y);
                // rotation
                disk.SetFloat($"player{i}-rotationX", player.transform.rotation.x);
                disk.SetFloat($"player{i}-rotationY", player.transform.rotation.y);
                disk.SetFloat($"player{i}-rotationZ", player.transform.rotation.z);
                disk.SetFloat($"player{i}-rotationW", player.transform.rotation.w);
            }

            SavePool(lootables, "lootable");
            SavePool(explodables, "explodable");
            SavePool(shots, "shot");
        }




       




        public void LoadGame()
        {
            if (!IsGameSaved())
            {
                return;
            }
            
            int players = disk.GetInt("players");
            for (int i = 0; i < players; i++)
            {
                if (Players.Count <= i)
                {
                    Players.Add(Instantiate(Players[0]));//TODO: need to add mirror handling
                }
                Player player = Players[i];
                player.Health = disk.GetFloat($"player{i}-health");
                player.Points = disk.GetInt($"player{i}-points");
                // position
                Vector3 position = Vector3.zero;
                position.x = disk.GetFloat($"player{i}-positionX");
                position.y = disk.GetFloat($"player{i}-positionY");
                player.transform.position = position;
                // rotation
                Quaternion rotation = Quaternion.identity;
                rotation.x = disk.GetFloat($"player{i}-rotationX");
                rotation.y = disk.GetFloat($"player{i}-rotationY");
                rotation.z = disk.GetFloat($"player{i}-rotationZ");
                rotation.w = disk.GetFloat($"player{i}-rotationW");
                player.transform.rotation = rotation;
            }

            LoadPool(lootables, "lootable");
            LoadPool(lootables, "explodable");
            LoadPool(lootables, "shot");
        }


        public bool IsGameSaved()
        {
            return disk.DoesKeyExist("saved") && disk.GetBool("saved");
        }


        private void SavePool<T>(ICache<T> pool, string name) where T : MonoBehaviour
        {
            var deployed = new List<T>(pool.Deployed);
            disk.SetInt($"{name}s", deployed.Count);
            for (int i = 0; i < deployed.Count; i++)
            {
                disk.SetFloat($"{name}{i}-positionX", deployed[i].transform.position.x);
                disk.SetFloat($"{name}{i}-positionY", deployed[i].transform.position.y);
            }
        }


        private void LoadPool<T>(ICache<T> cache, string name) where T : MonoBehaviour
        {
            int count = disk.GetInt($"{name}s");
            var deploy = new List<T>(cache.Deploy(count));
            for (int i = 0; i < count; i++)
            {
                T deployItem = deploy[i];
                Vector3 position = Vector3.zero;
                position.x = disk.GetFloat($"{name}{i}-positionX");
                position.y = disk.GetFloat($"{name}{i}-positionY");
            }
        }
    }
}





   
