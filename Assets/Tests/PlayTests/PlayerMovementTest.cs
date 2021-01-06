using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerMovementTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void PlayerMovementSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PlayerMovementWithEnumeratorPasses()
        {
            SceneManager.LoadScene("AsteroidsScene");
            yield return null;

            var player = GameObject.FindObjectOfType<Player>();
            var speed = player.PlayerSettings.MovementSpeed;
            Vector3 startPosition = player.transform.position, direction = new Vector3(1, 0, 0);
            float time = 0.5f;

            player.Move(direction, time);

            Assert.AreEqual(player.transform.position, startPosition + direction * speed * time );


            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }

    public class PlayerMovementTestBehaviour : MonoBehaviour, IMonoBehaviourTest
    {
        private int frameCount;
        public bool IsTestFinished
        {
            get { return frameCount > 10; }
        }

        void Update()
        {
            frameCount++;
        }
    }

}
