using System;
using UnityEngine;

public class UISpMainTower : MonoBehaviour
{
    public Action<bool> OnActiveShield;
    public Action OnUpgradeTower;
    public Action OnActiveSkill;
    private bool _activeShield;
    
    private CoinManager _coinManager;
    [SerializeField]
    private MainTower mainTower;
    
    [SerializeField]
    private int amountActiveShield = 30;
    [SerializeField]
    private int amountUpgradeTower = 50;
    [SerializeField]
    private int amountUsingSkinTower = 80;

    private void Start()
    {
        _coinManager = CoinManager.Instance;
    }

    public void ActiveShield()
    {
        if(_activeShield) return;
        if (_coinManager != null && _coinManager.TrySpendCoins(amountActiveShield))
        {        
            _activeShield = true;
            OnActiveShield?.Invoke(true);
        }
    }
    
    public void DisActiveShield()
    {
        _activeShield = false;
    }
    
    /*private void DisActiveShield()
    {
        OnActiveShield?.Invoke(false);
    }*/
    
    public void UpgradeTower()
    {
        if (_coinManager != null && _coinManager.TrySpendCoins(amountUpgradeTower))
        {        
            OnUpgradeTower?.Invoke();
        }
    }
    
    public void ActiveSkill()
    {
        if(mainTower.curMp >= 80)
        {
            mainTower.curMp = 0;
            OnActiveSkill?.Invoke();
        }
/*        if (_coinManager != null && _coinManager.TrySpendCoins(amountUsingSkinTower))
        {        
            OnActiveSkill?.Invoke();   
        }*/
    } 
}
