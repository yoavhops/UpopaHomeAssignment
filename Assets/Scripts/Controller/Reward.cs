using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Reward : MonoBehaviour
{
    [SerializeField]
    private Button clickToReward;
    

    protected virtual void Start()
    {
        clickToReward.onClick.AddListener(() =>  Award());
    }
    abstract public void Award();
}
