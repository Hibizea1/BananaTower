using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }

    public int Money { get; private set; }

    [SerializeField] private int MoneyPerWave = 100;
    [SerializeField] private int ScoreThreshold = 1000;
    [SerializeField] private int MoneyPerThreshold = 125;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        EventMaster.GetInstance().CreateNewEvent("EndGame");
        EventMaster.GetInstance().GetEvent("EndGame").AddListener(EndGame);
    }

    public void SpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            Debug.Log($"Spent {amount} money. Remaining Money: {Money}");
        }
        else
        {
            Debug.Log("Not enough money to spend.");
        }
    }

    public void EndGame()
    {
        Score += WaveManager.GetInstance().WaveCount * (ScoreThreshold / 10); // Assuming 1000 score for 10 waves
        ConvertScoreToMoney();
    }

    public void ConvertScoreToMoney()
    {
        int moneyEarned = (Score / 1000) * 50; // 50 pieces for every 1000 score
        Money += moneyEarned;
    }
}