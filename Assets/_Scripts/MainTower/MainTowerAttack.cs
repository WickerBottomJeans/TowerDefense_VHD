using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTowerAttack : MonoBehaviour
{
    private TowerData towerData;
    private IProjectile iprojectile;
    private float  currentAttackRange, currentFireRate;
    private float currentMaxAttackRange, currentMaxFireRate;
    [SerializeField]
    private MainTower mainTower;
    
    private HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();
    private HashSet<GameObject> subscribedEnemies = new HashSet<GameObject>();

    [SerializeField]
    private Transform firePoint;
    private float fireCooldown = 0f;
    private GameObject currentTarget;
    private Coroutine _coroutineSkill;
    
    [SerializeField] private List<UbhShotCtrl> shotCtrls;

    private void Start()
    {
        ResetObjShot();
    }

    private void ResetObjShot()
    {
        foreach (var shotCtrl in shotCtrls)
        {
            shotCtrl.gameObject.SetActive(false);
        }
    }
    
    public void ActiveSkill(int indexSkill)
    {
        StopCoroutineSkill();
        foreach (var shotCtrl in shotCtrls)
        {
            shotCtrl.StopShotRoutine();
        }
        _coroutineSkill = StartCoroutine(DelayStopSkill(indexSkill));
    }
    
    private void StopCoroutineSkill()
    {
        if (_coroutineSkill != null)
        {
            StopCoroutine(_coroutineSkill);
        }
    }

    private IEnumerator DelayStopSkill(int indexSkill)
    {
        yield return null;
        shotCtrls[indexSkill].gameObject.SetActive(true);
        shotCtrls[indexSkill].StartShotRoutine();
        yield return new WaitForSeconds(1f);
        shotCtrls[indexSkill].StopShotRoutine();
    }
    
    private void FixedUpdate()
    {
        HandleAttackCooldown();
    }

    public void InitData(TowerData mainTowerSo)
    {
        this.towerData = mainTowerSo;
        this.iprojectile = this.towerData.projectilePrefab.GetComponent<IProjectile>();
        currentAttackRange = currentMaxAttackRange = mainTowerSo.baseAttackRange;
        currentFireRate = currentMaxFireRate = mainTowerSo.baseFireRate;
        GetComponent<CircleCollider2D>().radius = currentMaxAttackRange;
    }

    private void HandleAttackCooldown() {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f) {
            AttackEnemy();
            fireCooldown = 1f / currentMaxFireRate;
        }
    }

    private void AttackEnemy() {
        currentTarget = FindEnemy();
        if (currentTarget == null) return;

        Enemy enemyScript = currentTarget.GetComponent<Enemy>();
        if (enemyScript != null && !subscribedEnemies.Contains(currentTarget)) {
            enemyScript.OnEnemyDestroyed += EnemyScript_OnEnemyDestroyed;
            subscribedEnemies.Add(currentTarget);
        }

        iprojectile = towerData.projectilePrefab.GetComponent<IProjectile>();
        if (iprojectile != null) {
            iprojectile.SpawnProjectile(towerData.projectilePrefab, 
                firePoint, currentTarget.transform);
            mainTower.AddMP();
        }
    }
    
    protected void EnemyScript_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        
    }

    private GameObject FindEnemy() {
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
    
    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(GameTags.TAG_ENEMY)) {
            enemiesInRange.Add(other.gameObject);
        }
    }

    protected void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(GameTags.TAG_ENEMY)) {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private void OnDisable()
    {
        StopCoroutineSkill();
    }
}
