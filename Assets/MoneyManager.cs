using TMPro;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField] TextMeshProUGUI MoneyText;
    int _currentMoney = 100;

    void Start()
    {
        EventMaster.GetInstance().CreateNewEventInt("AddMoney");
        EventMaster.GetInstance().CreateNewEventInt("RemoveMoney");
        EventMaster.GetInstance().GetEventInt("AddMoney").AddListener(IncreaseMoney);
        EventMaster.GetInstance().GetEventInt("RemoveMoney").AddListener(DecreaseMoney);
        MoneyText.text = _currentMoney.ToString();
    }

    private void IncreaseMoney(int value)
    {
        _currentMoney += value;
        MoneyText.text = _currentMoney.ToString();
    }
    private void DecreaseMoney(int value)
    {
        _currentMoney -= value;
        MoneyText.text = _currentMoney.ToString();
    }

    public int GetMoneyCount()
    {
        return _currentMoney;
    }
}
