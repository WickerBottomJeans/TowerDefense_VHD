using System;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUIMainTower : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;
    //[SerializeField] private Image mpBar;
    [SerializeField] private Image levelHeart;
    [SerializeField]
    private MainTower mainTower;

    private void Update()
    {
        mpBar.value = mainTower.curMp;
    }

    private void Start() {
        //mpBar.fillAmount = 0f;
        levelHeart.fillAmount = 0f;
        mpBar.maxValue = 100;
        mpBar.value = 0;
    }
    
    public void InitData(float maxHP) {
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;
    }
    
    public void UpdateHPBar(float currentHP) {
        hpBar.value = currentHP;
    }

    public void UpdateMPBar(float currentHP)
    {
        mpBar.value = currentHP;
    }
}
