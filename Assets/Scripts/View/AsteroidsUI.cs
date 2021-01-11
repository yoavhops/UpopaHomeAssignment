using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Supersonic
{
    public class AsteroidsUI : MonoBehaviour
    {
        [SerializeField]
        private Asteroids asteroids;
        [SerializeField]
        private List<PlayerUI> playerUIs;
        private List<Player> players;
        [SerializeField]
        private Text gameOver;

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
            asteroids.StateChangedEvent += GameStateChanged;
        }


        void GameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Over:
                    gameOver.gameObject.SetActive(true);
                    break;
                case GameState.Run:
                    playerUIs.ForEach(playerUI => playerUI.ResetHealth());
                    gameOver.gameObject.SetActive(false);
                    break;

            }
        }
    }
}