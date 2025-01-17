using System;
using TMPro;
using UnityEngine;

public class CanvaController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MoneyText;
    [SerializeField] TextMeshProUGUI WavesCountText;
    [SerializeField] TextMeshProUGUI NextWaves;
    [SerializeField] TextMeshProUGUI WavesSurvived;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI CoinsText;
    WaveManager _waveManager;
    MoneyManager _moneyManager;
    ScoreManager _scoreManager;


    void Start()
    {
        _waveManager = WaveManager.GetInstance();
        _moneyManager = MoneyManager.GetInstance();
        _scoreManager = ScoreManager.Instance;
        MoneyText.text = _moneyManager.GetMoneyCount().ToString();
        WavesCountText.text = _waveManager.WaveCount.ToString();
        NextWaves.text = _waveManager.WaitTime.ToString();
        EventMaster.GetInstance().CreateNewEvent("ReloadText");
        EventMaster.GetInstance().CreateNewEvent("GameOver");
        EventMaster.GetInstance().GetEvent("ReloadText").AddListener(Reload);
        EventMaster.GetInstance().GetEvent("GameOver").AddListener(Activate);
    }

    void Update()
    {
        MoneyText.text = _moneyManager.GetMoneyCount().ToString();
    }

    public void Activate()
    {
        WavesSurvived.transform.parent.transform.parent.gameObject.SetActive(true);
    }

    public void Reload()
    {
        WavesSurvived.text = _waveManager.WaveCount.ToString();
        ScoreText.text = _scoreManager.Score.ToString();
        CoinsText.text = _scoreManager.Money.ToString();
    }
}