using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

//THE TURRET WON'T ATTACK, REFACTOR ALL THESE STATS INTI AND STUFF!!!

public class TestTurret : MonoBehaviour {

    [Header("Turret Settings")]
    //public AbilitySO[] abilitySOArray;

    [Header("Base Stats")]
    [SerializeField] private TurretStatsSO turretStatsSO;

    private float currentHP, currentMP, currentAttackRange, currentFireRate;
    private float currentMaxHP, currentMaxMP, currentMaxAttackRange, currentMaxFireRate;

    [Header("References")]
    private IProjectile iprojectile;

    [SerializeField] private Transform turretPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LevelSO[] levelSOArray;
    [SerializeField] private LevelSystem levelSystem;

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

    private void Start() {
        InitStats();
        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }

    private void LevelSystem_OnLevelUp(object sender, LevelSystem.OnLevelUpEventArgs e) {
        Upgrade(e.level);
    }

    private void InitStats() {
        currentAttackRange = currentMaxAttackRange = turretStatsSO.baseAttackRange;
        currentFireRate = currentMaxFireRate = turretStatsSO.baseFireRate;
        currentMaxMP = turretStatsSO.baseMaxMP;
        currentMP = 0f;
        currentHP = currentMaxHP = turretStatsSO.baseMaxHP;
        GetComponent<CircleCollider2D>().radius = currentMaxAttackRange;
    }

    [SerializeField] private TurretIconUI turretIconUI;

    private float fireCooldown = 0f;
    private GameObject currentTarget;
    private HashSet<GameObject> subscribedEnemies = new HashSet<GameObject>();
    private HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();

    private void Update() {
        RotateTurretPivotTowardsTarget();
        HandleAttackCooldown();
    }

    private void HandleAttackCooldown() {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f) {
            AttackClosestEnemy();
            fireCooldown = 1f / currentMaxFireRate;
        }
    }

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

    private void AttackClosestEnemy() {
        currentTarget = FindClosestEnemy();
        if (currentTarget == null) return;

        Enemy enemyScript = currentTarget.GetComponent<Enemy>();
        if (enemyScript != null && !subscribedEnemies.Contains(currentTarget)) {
            enemyScript.OnEnemyDestroyed += EnemyScript_OnEnemyDestroyed;
            subscribedEnemies.Add(currentTarget);
        }
        iprojectile = turretStatsSO.projectilePrefab.GetComponent<IProjectile>();
        iprojectile.SpawnProjectile(turretStatsSO.projectilePrefab, firePoint, currentTarget.transform);
    }

    private void EnemyScript_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        //Gain MP
        currentMP += e.mpGain;
        if (currentMP >= currentMaxMP) {
            currentMP = currentMaxMP;
        }
        //Gain EXP
        levelSystem.AddEXP(e.expGain);
    }

    private GameObject FindClosestEnemy() {
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesInRange) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= currentAttackRange) {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}