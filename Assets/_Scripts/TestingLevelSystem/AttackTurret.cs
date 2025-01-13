using UnityEngine;

public class AttackTurret : _BaseTurret {

    #region AttackStuff

    public override void AttackEnemy() {
        currentTarget = FindEnemy();
        if (currentTarget == null) return;

        Enemy enemyScript = currentTarget.GetComponent<Enemy>();
        if (enemyScript != null && !subscribedEnemies.Contains(currentTarget)) {
            enemyScript.OnEnemyDestroyed += EnemyScript_OnEnemyDestroyed;
            subscribedEnemies.Add(currentTarget);
        }

        if (iprojectile != null) {
            iprojectile.SpawnProjectile(turretStatsSO.projectilePrefab, firePoint, currentTarget.transform);
        }
    }

    public override GameObject FindEnemy() {
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

    #endregion AttackStuff
}