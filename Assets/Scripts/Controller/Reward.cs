using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Supersonic
{
    public abstract class Reward : MonoBehaviour
    {
        public Shot Cause;
        [SerializeField]
        private Button clickToReward;


        protected virtual void Start()
        {
            clickToReward.onClick.AddListener(() =>
            {
                if (Cause.FiredBy.gameObject.activeInHierarchy)
                {
                    Award();
                }
            });
        }
        abstract public void Award();
    }
}