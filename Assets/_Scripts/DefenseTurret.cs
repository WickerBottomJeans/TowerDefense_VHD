using System;
using UnityEngine;
using UnityEngine.Rendering;

public class DefenseTurret : _BaseTurret {
    private GameObject currentSword;

    private void Update() {
        if (enemiesInRange.Count > 0) {
            AttackEnemy(); //this would release a sword with its first target, it would find other target later until it get destroy
        } else if (enemiesInRange.Count == 0 & currentSword != null) {
            DestroySword();
        }
    }

    private void Start() {
        iprojectile = turretStatsSO.projectilePrefab.GetComponent<IProjectile>();
        InitStats();
        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }

    private void DestroySword() {
        if (currentSword != null) {
            Destroy(currentSword.gameObject);
        }
    }

    public override void AttackEnemy() {
        //Attack closest enemy
        currentTarget = FindEnemy();
        if (currentTarget == null) {
            DestroySword();
            return;
        }

        Enemy enemyScript = currentTarget.GetComponent<Enemy>();
        if (enemyScript != null && !subscribedEnemies.Contains(currentTarget)) {
            enemyScript.OnEnemyDestroyed += EnemyScript_OnEnemyDestroyed;
            subscribedEnemies.Add(currentTarget);
        }
        if (currentSword == null) {
            iprojectile = turretStatsSO.projectilePrefab.GetComponent<IProjectile>();
            currentSword = iprojectile.SpawnProjectile(turretStatsSO.projectilePrefab, firePoint, currentTarget.transform);
        } else {
            currentSword.GetComponent<SwordSpell>().SetTarget(currentTarget.transform);
        }
    }

    public void EnemyScript_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        //Gain MP
        currentMP += e.mpGain;
        if (currentMP >= currentMaxMP) {
            currentMP = currentMaxMP;
        }
        //Gain EXP
        levelSystem.AddEXP(e.expGain);

        //Give new target to the sword
        currentSword.GetComponent<SwordSpell>().SetTarget(FindEnemy().transform);
        FireOnHPChanged();
    }

    public override GameObject FindEnemy() {
        //Find closest enemy
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesInRange) {
            if (enemy == null) {
                Debug.Log("Enemy = null???");
                enemiesInRange.Remove(enemy);
                continue;
            }
            float distanceToEnemy;
            if (currentSword == null) {
                //error will pop if distanceToEnemy > currentAttackRange
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

    public void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}