using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeartManager : Singleton<HeartManager>
{
    [SerializeField] int CurrentHealth;
    [SerializeField] int MaxHealth;

    void Start()
    {
        EventMaster.GetInstance().CreateNewEventInt("HeartDamage");
        EventMaster.GetInstance().GetEventInt("HeartDamage").AddListener(TakeDamage);

        CurrentHealth = MaxHealth;
    }

    private void TakeDamage(int damage)
    {
        if (CurrentHealth >= 0)
        {
            CurrentHealth -= damage;
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        EventMaster.GetInstance().InvokeEvent("EndGame");
        EventMaster.GetInstance().InvokeEvent("GameOver");
    }
}