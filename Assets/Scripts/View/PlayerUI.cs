using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Text points;
    
    [SerializeField]
    private HealthBar health;
    public void Setup(Player player)
    {
        player.PointsChangedEvent += OnPointsChanged;
        player.HealthChangedEvent += OnHealthChanged;
        OnPointsChanged(player.Points);
        OnHealthChanged(player.Health);

    }

    private void OnPointsChanged(int amount)
    {
        points.text = $"Score\n{amount}";
    }

    private void OnHealthChanged(float amount)
    {
        health.BarValue = amount;
    }
    
}
