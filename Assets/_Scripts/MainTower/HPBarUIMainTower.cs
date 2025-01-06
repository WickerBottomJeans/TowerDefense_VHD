using System;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUIMainTower : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image levelHeart;

    private void Start() {
        mpBar.fillAmount = 0f;
        levelHeart.fillAmount = 0f;
    }
    
    public void InitData(float maxHP) {
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;
    }
    
    public void UpdateHPBar(float currentHP) {
        hpBar.value = currentHP;
    }
}
