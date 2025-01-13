using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour, IHasHPBar {
    
    public Transform turretPivot;
    public MainTowerSO mainTowerSO;
    private GameObject currentTarget;
    public float currentHP, currentMP;
    public float currentMaxHP, currentMaxMP;

    public int levelTower = 0;

    public event EventHandler<IHasHPBar.OnHPChangedEventArgs> OnHPChanged;
    
    protected virtual void Start() {
        InitStats();
    }
    
    protected void Update() {
        RotateTurretPivotTowardsTarget();
    }

    private void RotateTurretPivotTowardsTarget()
    {
        if (currentTarget == null || turretPivot == null) return;

        Vector2 direction = (currentTarget.transform.position - turretPivot.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretPivot.rotation = Quaternion.RotateTowards(
            turretPivot.rotation,
            Quaternion.Euler(0, 0, angle),
            360f * Time.deltaTime
        );
    }

    protected virtual void InitStats() {
        currentHP = currentMaxHP = mainTowerSO.GetTowerData(levelTower).baseMaxHP;
        currentMP = currentMaxMP = mainTowerSO.GetTowerData(levelTower).baseMaxMP;
    }
    
    protected virtual void TakeDamage(float damage) {
        if (currentHP > damage) {
            currentHP -= damage;
            //FireOnHPChanged();
        } else {
            Die();
            OnHPChanged?.Invoke(this, new IHasHPBar.OnHPChangedEventArgs {
                HPNormalized = 0f
            });
        }
    }

    protected virtual void Die() {
        Destroy(gameObject);
    }

    /*protected void FireOnHPChanged() {
        OnHPChanged?.Invoke(this, new IHasHPBar.OnHPChangedEventArgs {
            HPNormalized = currentHP / currentMaxHP,
            MPNormalized = currentMP / currentMaxMP,
        });
    }*/
    
    public virtual void Upgrade() {
        /*if (level == 0) { return; }
        currentMaxAttackRange = turretStatsSO.baseAttackRange + (level * (turretStatsSO.baseAttackRange * turretStatsSO.attackRangeMultiplier));
        currentMaxFireRate = turretStatsSO.baseFireRate + (level * (turretStatsSO.baseFireRate * turretStatsSO.fireRateMultiplier));
        currentMaxMP = turretStatsSO.baseMaxMP + (level * (turretStatsSO.baseMaxMP * turretStatsSO.maxMPMultiplier));
        currentMaxHP = turretStatsSO.baseMaxHP + (level * (turretStatsSO.baseMaxHP * turretStatsSO.maxHPMultiplier));

        currentHP = Mathf.Min(currentHP + (currentMaxHP - currentHP), currentMaxHP);
        currentAttackRange = Mathf.Min(currentAttackRange + (currentMaxAttackRange - currentAttackRange), currentMaxAttackRange);
        currentFireRate = Mathf.Min(currentFireRate + (currentMaxFireRate - currentFireRate), currentMaxFireRate);

        specialSkillPrefab = turretStatsSO.specialSkillPrefab;

        FireOnHPChanged();*/
    }
}
