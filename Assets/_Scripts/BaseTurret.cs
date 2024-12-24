using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseTurret : MonoBehaviour {

    #region Reference

    [SerializeField] private Transform firePoint;
    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private Transform turretPivot;
    [SerializeField] private TurretStatsSO turretStatsSO;

    #endregion Reference

    #region Variables

    private GameObject currentTarget;
    private IProjectile iprojectile;
    private float currentHP, currentMP, currentAttackRange, currentFireRate;
    private float currentMaxHP, currentMaxMP, currentMaxAttackRange, currentMaxFireRate;
    private HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();
    private float fireCooldown = 0f;
    private HashSet<GameObject> subscribedEnemies = new HashSet<GameObject>();

    #endregion Variables

    #region LifeCycle

    private void Update() {
        RotateTurretPivotTowardsTarget();
        HandleAttackCooldown();
    }

    private void Start() {
        InitStats();
        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }

    #endregion LifeCycle

    #region VisualStuff

    private void RotateTurretPivotTowardsTarget() {
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

    private void EnemyScript_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        //Gain MP
        currentMP += e.mpGain;
        if (currentMP >= currentMaxMP) {
            currentMP = currentMaxMP;
        }
        //Gain EXP
        levelSystem.AddEXP(e.expGain);
    }

    private void HandleAttackCooldown() {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f) {
            AttackEnemy();
            fireCooldown = 1f / currentMaxFireRate;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger entered by: " + other.gameObject.name);
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Add(other.gameObject);
            Debug.Log("Added to enemiesInRange: " + other.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
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

    private void InitStats() {
        currentAttackRange = currentMaxAttackRange = turretStatsSO.baseAttackRange;
        currentFireRate = currentMaxFireRate = turretStatsSO.baseFireRate;
        currentMaxMP = turretStatsSO.baseMaxMP;
        currentMP = 0f;
        currentHP = currentMaxHP = turretStatsSO.baseMaxHP;
        GetComponent<CircleCollider2D>().radius = currentMaxAttackRange;
    }

    private void LevelSystem_OnLevelUp(object sender, LevelSystem.OnLevelUpEventArgs e) {
        Upgrade(e.level);
    }

    #endregion DataStuff
}