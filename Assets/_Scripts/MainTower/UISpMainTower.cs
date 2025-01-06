using System;
using UnityEngine;

public class UISpMainTower : MonoBehaviour
{
    public Action<bool> OnActiveShield;
    public Action OnUpgradeTower;
    public Action OnActiveSkill;
    private bool _activeShield;

    public void ActiveShield()
    {
        if(_activeShield) return;
        _activeShield = true;
        OnActiveShield?.Invoke(true);
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
        OnUpgradeTower?.Invoke();
    }
    
    public void ActiveSkill()
    {
        OnActiveSkill?.Invoke();   
    } 
}
