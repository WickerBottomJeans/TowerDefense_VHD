using System;
using UnityEngine;

public class BulletMainTower : MonoBehaviour, IProjectile {
    
    [SerializeField] private float speed = .5f;
    [SerializeField] private float chillTime = 1f;
    [SerializeField] private int damage = 200;
    [SerializeField] private Transform target;
    
    private void FixedUpdate() {
        if(target == null) {
            Destroy(gameObject);
            return;
        }
        transform.position = Vector3.MoveTowards
            (transform.position, target.position, speed * Time.deltaTime);
    }
    public GameObject SpawnProjectile(GameObject projectilePrefab, 
        Transform spawnPoint, Transform target) {
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        BulletMainTower projScript = projectile.GetComponent<BulletMainTower>();
        if (projScript != null) {
            projScript.SetTarget(target);
        }
        return projectile;
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(GameTags.TAG_ENEMY)) {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
