using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseTurret : MonoBehaviour {

    #region Reference

    [SerializeField] public Transform firePoint;
    [SerializeField] public LevelSystem levelSystem;
    [SerializeField] public Transform turretPivot;
    [SerializeField] public TurretStatsSO turretStatsSO;

    #endregion Reference

    #region Variables

    protected GameObject currentTarget;
    protected IProjectile iprojectile;
    protected float currentHP, currentMP, currentAttackRange, currentFireRate;
    protected float currentMaxHP, currentMaxMP, currentMaxAttackRange, currentMaxFireRate;
    protected HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();
    protected float fireCooldown = 0f;
    protected HashSet<GameObject> subscribedEnemies = new HashSet<GameObject>();

    #endregion Variables

    #region LifeCycle

    protected void Update() {
        RotateTurretPivotTowardsTarget();
        HandleAttackCooldown();
    }

    protected void Start() {
        InitStats();
        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }

    #endregion LifeCycle

    #region VisualStuff

    protected void RotateTurretPivotTowardsTarget() {
        if (currentTarget == null || turretPivot == null) return;

        Vector2 direction = (currentTarget.transform.position - turretPivot.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretPivot.rotation = Quaternion.RotateTowards(
            turretPivot.rotation,
            Quaternion.Euler(0, 0, angle),
            360f * Time.deltaTime
        );
    }

    #endregion VisualStuff

    #region AttackStuff

    public abstract void AttackEnemy();

    public abstract GameObject FindEnemy();

    protected void EnemyScript_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        //Gain MP
        currentMP += e.mpGain;
        if (currentMP >= currentMaxMP) {
            currentMP = currentMaxMP;
        }
        //Gain EXP
        levelSystem.AddEXP(e.expGain);
    }

    protected void HandleAttackCooldown() {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f) {
            AttackEnemy();
            fireCooldown = 1f / currentMaxFireRate;
        }
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger entered by: " + other.gameObject.name);
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Add(other.gameObject);
            Debug.Log("Added to enemiesInRange: " + other.gameObject.name);
        }
    }

    protected void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Remove(other.gameObject);
            Debug.Log("Enemy exited range: " + other.gameObject.name);
        }
    }

    #endregion AttackStuff

    #region DataStuff

    public void Upgrade(int level) {
        if (level == 0) { return; }
        currentMaxAttackRange = turretStatsSO.baseAttackRange + (level * (turretStatsSO.baseAttackRange * turretStatsSO.attackRangeMultiplier));
        currentMaxFireRate = turretStatsSO.baseFireRate + (level * (turretStatsSO.baseFireRate * turretStatsSO.fireRateMultiplier));
        currentMaxMP = turretStatsSO.baseMaxMP + (level * (turretStatsSO.baseMaxMP * turretStatsSO.maxMPMultiplier));
        currentMaxHP = turretStatsSO.baseMaxHP + (level * (turretStatsSO.baseMaxHP * turretStatsSO.maxHPMultiplier));

        // Min to ensure those current stats don't exceed the max
        currentHP = Mathf.Min(currentHP + (currentMaxHP - currentHP), currentMaxHP);
        currentMP = Mathf.Min(currentMP + (currentMaxMP - currentMP), currentMaxMP);
        currentAttackRange = Mathf.Min(currentAttackRange + (currentMaxAttackRange - currentAttackRange), currentMaxAttackRange);
        currentFireRate = Mathf.Min(currentFireRate + (currentMaxFireRate - currentFireRate), currentMaxFireRate);
    }

    protected void InitStats() {
        currentAttackRange = currentMaxAttackRange = turretStatsSO.baseAttackRange;
        currentFireRate = currentMaxFireRate = turretStatsSO.baseFireRate;
        currentMaxMP = turretStatsSO.baseMaxMP;
        currentMP = 0f;
        currentHP = currentMaxHP = turretStatsSO.baseMaxHP;
        GetComponent<CircleCollider2D>().radius = currentMaxAttackRange;
    }

    protected void LevelSystem_OnLevelUp(object sender, LevelSystem.OnLevelUpEventArgs e) {
        Upgrade(e.level);
    }

    #endregion DataStuff
}