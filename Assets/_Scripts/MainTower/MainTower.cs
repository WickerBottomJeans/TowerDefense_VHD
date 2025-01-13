using System;
using Unity.VisualScripting;
using UnityEngine;

public class MainTower: BaseTower
{

    private MainTowerAttack _mainTowerAttack;
    private MainTowerHealing _mainTowerHealing;
    private UISpMainTower _uiSpMainTower;
    private HPBarUIMainTower _hpBarUIMainTower;
    [SerializeField]
    private ParticleSystem fxDeath;
    [SerializeField]
    private ParticleSystem fxUpgrade;
    
    //shield
    [SerializeField]
    private GameObject shield;
    private bool _activeShield;
    
    private int _takeTowerDisShield = 0;
    private int _totalTakeDisShield = 5;

    public float curMp = 0f;

    private void Awake()
    {
        _mainTowerAttack = GetComponent<MainTowerAttack>();
        _mainTowerHealing = GetComponentInChildren<MainTowerHealing>();
        _uiSpMainTower = GetComponentInChildren<UISpMainTower>();
        _hpBarUIMainTower = GetComponentInChildren<HPBarUIMainTower>();
    }

    public void AddMP()
    {
        if(curMp <= 100)
        {
            curMp++;
        }
    }

    protected override void Start()
    {
        base.Start();
        InitDataTower();
        _mainTowerHealing.OnTakeDamage += TakeDamage;
        _uiSpMainTower.OnActiveShield += ActiveShield;
        _uiSpMainTower.OnUpgradeTower += Upgrade;
        _uiSpMainTower.OnActiveSkill += ActiveSkill;
    }

    private void ActiveSkill()
    {
        _mainTowerAttack.ActiveSkill(levelTower);
    }

    private void LoadAgainData()
    {
        InitStats();
        InitDataTower();
    }

    private void InitDataTower()
    {
        _mainTowerAttack.InitData(mainTowerSO.GetTowerData(levelTower));
        _mainTowerHealing.InitData
            (currentHP, currentMaxHP, currentMP, currentMaxMP);
        _hpBarUIMainTower.InitData
            (currentMaxHP);
    }

    protected override void InitStats()
    {
        base.InitStats();
    }

    private void ActiveShield(bool active)
    {
        _activeShield = active;
        shield.SetActive(active);
    }

    protected override void TakeDamage(float damage)
    {
        if (_activeShield)
        {
            _takeTowerDisShield++;
            if (_takeTowerDisShield >= _totalTakeDisShield)
            {
                _takeTowerDisShield = 0;
                ActiveShield(false);
                _uiSpMainTower.DisActiveShield();
            }
            return;
        }
        base.TakeDamage(damage);
        _hpBarUIMainTower.UpdateHPBar(currentHP);
    }

    public override void Upgrade()
    {
        base.Upgrade();
        SpawnFxUpgrade();
    }

    protected override void Die()
    {
        base.Die();
        //SpawnFxDeath();
        Debug.Log("Die");
        GameManager.Instance.SetState(GameManager.State.Lose);
    }

    private void SpawnFxDeath()
    {
        Instantiate(fxDeath, transform.position, Quaternion.identity);
    }

    private void SpawnFxUpgrade()
    {
        fxUpgrade.Play();
        levelTower++;
        LoadAgainData();
    }
    
    private void OnDestroy()
    {
        _mainTowerHealing.OnTakeDamage -= TakeDamage;
        _uiSpMainTower.OnActiveShield -= ActiveShield;
        _uiSpMainTower.OnUpgradeTower -= Upgrade;
        _uiSpMainTower.OnActiveSkill -= ActiveSkill;
    }
}