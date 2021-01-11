using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supersonic
{
    public class Shot : MonoBehaviour
    {
        public delegate void ShotHit(Shot shot, GameObject objectHit);
        public event ShotHit ShotHitEvent;
        public Player FiredBy;
        public float TimeAlive;

        private Coroutine enforceTimeAlive;

        private void Awake()
        {
            Debug.Log($"{name} starts time alive");
            StartTimeAlive();
        }


        public void StartTimeAlive()
        {
            if (enforceTimeAlive != null)
            {
                StopCoroutine(enforceTimeAlive);
            }
            enforceTimeAlive = StartCoroutine(EnforceTimeAlive(TimeAlive));
        }

        IEnumerator EnforceTimeAlive(float time)
        {
            
            yield return new WaitForSeconds(time);
            ShotHitEvent?.Invoke(this, null);
            gameObject.SetActive(false);
            Debug.Log($"{name} dead");
        }



        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{name} hit {other.gameObject.name}");
            
            Shootable shootable = other.gameObject.GetComponent<Shootable>();
            if (shootable != null)
            {
                ShotHitEvent?.Invoke(this, other.gameObject);
                gameObject.SetActive(false);
                return;
            }
            Border border = other.gameObject.GetComponent<Border>();
            if (border != null)
            {
                ShotHitEvent?.Invoke(this, other.gameObject);
                gameObject.SetActive(false);
                return;
            }
        }
    }
}