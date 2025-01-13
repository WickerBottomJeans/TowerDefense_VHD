using UnityEngine;

public class FunTurret : _BaseTurret {
    //second ability earth toss

    protected void Update() {
        RotateTurretPivotTowardsTarget();
        HandleAttackCooldown();
    }

    protected void Start() {
        InitStats();
        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }

    public override GameObject FindEnemy() {
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesInRange) {
            if (enemy.CompareTag("Enemy")) {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance && distanceToEnemy <= currentAttackRange) {
                    shortestDistance = distanceToEnemy;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }

    public override void AttackEnemy() {
        currentTarget = FindEnemy();

        if (currentTarget == null || !enemiesInRange.Contains(currentTarget)) {
            return;
        }

        Enemy enemyScript = currentTarget.GetComponent<Enemy>();
        if (enemyScript != null && !subscribedEnemies.Contains(currentTarget)) {
            enemyScript.OnEnemyDestroyed += EnemyScript_OnEnemyDestroyed;
            enemyScript.OnAppliedHypnotize += EnemyScript_OnAppliedHypnotize;

            subscribedEnemies.Add(currentTarget);
        }

        iprojectile = turretStatsSO.projectilePrefab.GetComponent<IProjectile>();
        iprojectile.SpawnProjectile(turretStatsSO.projectilePrefab, firePoint, currentTarget.transform);
    }
}