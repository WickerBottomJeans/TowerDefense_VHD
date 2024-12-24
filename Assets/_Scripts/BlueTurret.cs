using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BlueTurret : MonoBehaviour {
    [Header("Turret Settings")]
    public float attackRange = 5f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    //public AbilitySO[] abilitySOArray;
    public int specialAttackManaCost = 10;

    [Header("References")]
    public Transform turretPivot;
    public Transform firePoint;
    public LevelSO[] levelSOArray;

    [Header("Upgrade Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] TurretIconUI turretIconUI;

    public float currentMana = 0f;
    public float maxMana = 100f;

    private float fireCooldown = 0f;
    private GameObject currentTarget;
    private HashSet<GameObject> subscribedEnemies = new HashSet<GameObject>();
    private List<GameObject> enemiesInRange = new List<GameObject>();


    //About upgrade bullshit
    public event EventHandler<OnUpgradableEventArgs> OnUpgradable;
    public class OnUpgradableEventArgs : EventArgs {
        public LevelSO levelSO;
    }

    private void Start() {
        // Automatically scale collider to attack range
        SphereCollider rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.radius = attackRange;
        turretIconUI.OnUpgradeButtonClicked += UpgradeIconTemplate_OnUpgradeButtonClicked;
    }

    private void UpgradeIconTemplate_OnUpgradeButtonClicked(object sender, EventArgs e) {
        UpgradeTurret();
    }

    private void Update() {
        RotateTurretPivotTowardsTarget();
        HandleAttackCooldown();
    }

    private void HandleAttackCooldown() {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f) {
            AttackEnemy();
            fireCooldown = 1f / fireRate;
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

    private void AttackEnemy() {
        currentTarget = FindClosestEnemy();
        if (currentTarget == null) return;

        Enemy enemyScript = currentTarget.GetComponent<Enemy>();
        if (enemyScript != null && !subscribedEnemies.Contains(currentTarget)) {
            enemyScript.OnEnemyDestroyed += EnemyScript_OnEnemyDestroyed;
            subscribedEnemies.Add(currentTarget);
        }

        //WaterSpell.SpawnProjectile(projectilePrefab, firePoint, currentTarget.transform);
    }

    private GameObject FindClosestEnemy() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= attackRange) {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }



    public void GainMana(float amount) {
        currentMana += amount;
        if (currentMana >= maxMana) {
            currentMana = maxMana;
        }
        //CheckUpgradable();
    }


    public void UpgradeTurret() {
        //LevelSO currentLevelSO = levelSOArray.FirstOrDefault(levelSO => levelSO.level == currentLevel);
        //currentMana -= currentLevelSO.levelUpgradeManaCost;
        //currentLevel++;
        //Debug.Log("Upgraded");
    }



    private void EnemyScript_OnEnemyDestroyed(object sender, EventArgs e) {
        GainMana(currentTarget.GetComponent<Enemy>().getMPGain());
    }

    private void CheckUpgradable() {
        //LevelSO currentLevelSO = levelSOArray.FirstOrDefault(levelSO => levelSO.level == currentLevel);
        //Debug.Log(currentLevelSO);
        //if (currentLevelSO != null && currentMana >= currentLevelSO.levelUpgradeManaCost) {
        //    OnUpgradable?.Invoke(this, new OnUpgradableEventArgs {
        //        levelSO = currentLevelSO
        //    });
        //}
        //EAT SHIT
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}