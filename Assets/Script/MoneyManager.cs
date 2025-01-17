using TMPro;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField] int _currentMoney = 100;

    void Start()
    {
        EventMaster.GetInstance().CreateNewEventInt("AddMoney");
        EventMaster.GetInstance().CreateNewEventInt("RemoveMoney");
        EventMaster.GetInstance().GetEventInt("AddMoney").AddListener(IncreaseMoney);
        EventMaster.GetInstance().GetEventInt("RemoveMoney").AddListener(DecreaseMoney);
    }

    public void LoadData(int money)
    {
        _currentMoney = money;
    }
    
    
    private void IncreaseMoney(int value)
    {
        _currentMoney += value;
    }
    private void DecreaseMoney(int value)
    {
        _currentMoney -= value;
    }

    public bool CheckMoneyCount(int cost)
    {
        if (_currentMoney < cost)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    
    public int GetMoneyCount()
    {
        return _currentMoney;
    }
}
