using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour {
    public static CoinManager Instance { get; private set; }
    public TextMeshProUGUI text;
    public int coinBalance = 200;

    public event Action<int> OnCoinBalanceChange;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        text.SetText(coinBalance.ToString());
        OnCoinBalanceChange += CoinManager_OnCoinBalanceChange;
    }

    private void CoinManager_OnCoinBalanceChange(int obj) {
        text.SetText(coinBalance.ToString());
    }

    public void AddCoin(int amount) {
        coinBalance += amount;

        OnCoinBalanceChange?.Invoke(coinBalance);
    }

    public bool TrySpendCoins(int cost) {
        if (coinBalance >= cost) {
            coinBalance -= cost;
            OnCoinBalanceChange?.Invoke(coinBalance);
            return true;
        } else {
            return false;
        }
    }

    public void ResetCoinBalance() {
        coinBalance = 200;
        OnCoinBalanceChange?.Invoke(coinBalance);
    }
}