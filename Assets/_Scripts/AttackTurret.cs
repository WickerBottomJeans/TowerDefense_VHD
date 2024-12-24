using System;
using UnityEngine;

public class AttackTurret : BaseTurret {
    private GameObject currentSword;

    private void Update() {
        if (enemiesInRange.Count > 0 && currentSword == null) {
            AttackEnemy(); //this would release a sword with its first target, it would find other target later until it get destroy
        } else if (enemiesInRange.Count == 0 & currentSword != null) {
            DestroySword();
        }
    }

    private void Start() {
        iprojectile = turretStatsSO.projectilePrefab.GetComponent<IProjectile>();
    }

    private void DestroySword() {
        throw new NotImplementedException();
    }

    public override void AttackEnemy() {
        //Attack closest enemy
        currentTarget = FindEnemy();
        if (currentTarget == null) return;

        Enemy enemyScript = currentTarget.GetComponent<Enemy>();
        if (enemyScript != null && !subscribedEnemies.Contains(currentTarget)) {
            enemyScript.OnEnemyDestroyed += EnemyScript_OnEnemyDestroyed;
            subscribedEnemies.Add(currentTarget);
        }
        iprojectile = turretStatsSO.projectilePrefab.GetComponent<IProjectile>();
        currentSword = iprojectile.SpawnProjectile(turretStatsSO.projectilePrefab, firePoint, currentTarget.transform);
    }

    public override GameObject FindEnemy() {
        //Find closest enemy
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesInRange) {
            float distanceToEnemy;
            if (currentSword == null) {
                distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            } else {
                distanceToEnemy = Vector3.Distance(currentSword.transform.position, enemy.transform.position);
            }
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= currentAttackRange) {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }
}