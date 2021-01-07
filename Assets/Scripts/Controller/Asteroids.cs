using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic
{


    public enum GameState { Start, Run, Pause }
    public class Asteroids : MonoBehaviour
    {

        public GameState State { get; private set; }
        public AsteroidSettings Settings;

        // Start is called before the first frame update
        void Start()
        {
            State = GameState.Run;
        }

        // Update is called once per frame
        void Update()
        {
            if (State == GameState.Run)
            {

            }
        }



    }

   

}