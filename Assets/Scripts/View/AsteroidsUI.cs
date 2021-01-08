using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{
    public class AsteroidsUI : MonoBehaviour
    {
        [SerializeField]
        private Asteroids asteroids;
        [SerializeField]
        private List<PlayerUI> playerUIs;
        private List<Player> players;
        // Start is called before the first frame update
        void Start()
        {
            players = asteroids.Players;
            if (players.Count != playerUIs.Count)
            {
                throw new ArgumentException($"Player controllers{players.Count} and views{playerUIs.Count} amounts are not equal");
            }
            for (int i = 0; i < players.Count; i++)
            {
                playerUIs[i].Setup(players[i]);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}