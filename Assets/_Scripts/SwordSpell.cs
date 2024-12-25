using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SwordSpell : MonoBehaviour, IProjectile {
    [SerializeField] private float speed = .5f;
    [SerializeField] private float chillTime = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float smoothFactor = 0.1f;
    [SerializeField] private BaseTurret turret;
    private Rigidbody2D rb;
    private Transform target;
    private bool isAttacking;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    //WE need a way to pump new target constantly when the currentTarget dies
    private void Update() {
        if (isAttacking && target != null) {
            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            //error here. we need to give next target for the sword when enemy dies
            Vector2 newVelocity = Vector2.Lerp(currentVelocity, directionToTarget * speed, smoothFactor);
            rb.linearVelocity = newVelocity;
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public GameObject SpawnProjectile(GameObject projectilePrefab, Transform SpawnPoint, Transform target) {
        GameObject projectile = Instantiate(projectilePrefab, SpawnPoint.position, Quaternion.identity);
        projectile.GetComponent<SwordSpell>().SetTarget(target);
        return projectile;
    }

    public void SetTarget(Transform target) {
        this.target = target;
        isAttacking = true;
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;
        }
    }

    public void FixedUpdate() {
    }

    private void Enemy_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        isAttacking = false;
    }
}