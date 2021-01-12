﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Supersonic
{
    public enum GameState { Start, Run, Pause, Over }
    public class Asteroids : MonoBehaviour
    {
        public delegate void StateChanged(GameState state);
        public event StateChanged StateChangedEvent;
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

            ChangeState(GameState.Run);
            
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
            ChangeState(GameState.Over);
        }

        public void ChangeState(GameState state)
        {
            switch(state)
            {
                case GameState.Over:
                    break;
                    
            }
            State = state;
            
            StateChangedEvent?.Invoke(state);
        }

        private void ResetGame()
        {
            lootables.Undeploy(lootables.Deployed);
            explodables.Undeploy(explodables.Deployed);
            shots.Undeploy(shots.Deployed);
            var rewards = rewardParent.GetComponentsInChildren<Reward>();
            foreach (var reward in rewards)
            {
                Destroy(reward.gameObject);
            }
        }


        public void SaveGame()
        {
            if (State != GameState.Run)
            {
                return;
            }

            disk.SetBool("saved", true);
            disk.SetInt("players", Players.Count);
            for (int i = 0; i < Players.Count; i++)
            {
                Player player = Players[i];
                disk.SetFloat($"player{i}-health", player.Health);
                disk.SetInt($"player{i}-points", player.Points);
                // position
                disk.SetFloat($"player{i}-positionX", player.transform.position.x);
                disk.SetFloat($"player{i}-positionY", player.transform.position.y);
                // rotation
                disk.SetFloat($"player{i}-rotationX", player.transform.rotation.x);
                disk.SetFloat($"player{i}-rotationY", player.transform.rotation.y);
                disk.SetFloat($"player{i}-rotationZ", player.transform.rotation.z);
                disk.SetFloat($"player{i}-rotationW", player.transform.rotation.w);
            }

            SaveCyclicPool<Lootable,Shootable>(lootables, "lootable");
            SaveCyclicPool<Explodable, Shootable>(explodables, "explodable");
            SavePool(shots, "shot");
        }




       




        public void LoadGame()
        {
            if (!IsGameSaved())
            {
                return;
            }

            ResetGame();
            ChangeState(GameState.Run);

            int players = disk.GetInt("players");
            for (int i = 0; i < players; i++)
            {
                if (Players.Count <= i)
                {
                    Players.Add(Instantiate(Players[0]));//TODO: need to add mirror handling
                }
                Player player = Players[i];
                player.gameObject.SetActive(true);
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

            LoadCyclicPool<Lootable, CyclicalShootable, Shootable>(lootables, "lootable");
            LoadCyclicPool<Explodable, CyclicalShootable, Shootable>(explodables, "explodable");
            LoadPool(shots, "shot");

            foreach (var lootable in lootables.Deployed)
            {
                lootable.RewardParent = rewardParent;
            }
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
                Floatable floatable = deployed[i].GetComponent<Floatable>();
                if (floatable != null)
                {
                    disk.SetFloat($"{name}{i}-floatX", floatable.Direction.x);
                    disk.SetFloat($"{name}{i}-floatY", floatable.Direction.y);
                }
            }
        }

        private void SaveCyclicPool<T,K>(ICache<T> pool, string name) where T : K, ICyclic<K> where K : MonoBehaviour
        {
            var poolDeployed = pool.Deployed;
            poolDeployed.RemoveWhere(cyclic => cyclic.IsMirror);
            var deployed = new List<T>(poolDeployed);
            Debug.Log($"saving {deployed.Count}");
            disk.SetInt($"{name}s", deployed.Count);
            for (int i = 0; i < deployed.Count; i++)
            {
                disk.SetFloat($"{name}{i}-positionX", deployed[i].transform.position.x);
                disk.SetFloat($"{name}{i}-positionY", deployed[i].transform.position.y);
                Floatable floatable = deployed[i].GetComponent<Floatable>();
                if (floatable != null)
                {
                    disk.SetFloat($"{name}{i}-floatX", floatable.Direction.x);
                    disk.SetFloat($"{name}{i}-floatY", floatable.Direction.y);
                }
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
                deployItem.transform.position = position;

                Floatable floatable = deployItem.GetComponent<Floatable>();
                if (floatable != null)
                {
                    Vector3 direction = Vector3.zero;
                    direction.x = disk.GetFloat($"{name}{i}-floatX");
                    direction.y = disk.GetFloat($"{name}{i}-floatY");
                    floatable.Direction = direction;
                }
            }
        }

        private void LoadCyclicPool<T,K,U>(ICache<T> cache, string name) where T : U, ICyclic<U> where K : Cyclical<U> where U : MonoBehaviour
        {
            LoadPool(cache, name);

            int count = disk.GetInt($"{name}s");
            List<T> deploy = new List<T>(cache.Deployed), mirrors = new List<T>(cache.Deploy(count));
            Debug.Log($"loading {deploy.Count}");
            for (int i = 0; i < count; i++)
            {
                T deployItem = deploy[i];
                K cyclical = deployItem.GetComponent<K>();
                cyclical.Mirror = mirrors[i];
                cyclical.IsMirrorObject = false;
                cyclical.Playground = Playground;

                K mirror = mirrors[i].GetComponent<K>();
                mirror.Mirror = deployItem;
                mirror.IsMirrorObject = true;
                mirror.Playground = Playground;
            }
        }
    }
}





   
