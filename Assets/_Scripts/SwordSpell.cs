using Unity.VisualScripting;
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
    private bool isOnTopTarget = false;

    //make it swing
    [SerializeField] private float swingAmplitude = 10f; // Maximum angle for swinging

    [SerializeField] private float swingFrequency = 5f;  // Speed of the swing
    private float swingTimer = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    //WE need a way to pump new target constantly when the currentTarget dies
    private void Update() {
        if (isAttacking && target != null) {
            if (isOnTopTarget) {
                // Stick the sword to the enemy
                transform.position = target.position;

                // Swinging effect while stuck
                swingTimer += Time.deltaTime * swingFrequency;
                float swingAngle = Mathf.Sin(swingTimer) * swingAmplitude;
                transform.rotation = Quaternion.Euler(0, 0, swingAngle);
            } else {
                // Continue moving towards the target
                Vector2 currentVelocity = rb.linearVelocity;
                Vector2 directionToTarget = (target.position - transform.position).normalized;
                Vector2 newVelocity = Vector2.Lerp(currentVelocity, directionToTarget * speed, smoothFactor);
                rb.linearVelocity = newVelocity;

                // Rotate towards the target
                Vector2 direction = (target.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.Euler(0, 0, angle),
                    360f * Time.deltaTime
                );
            }
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

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Debug.Log(enemy);
            isOnTopTarget = true;
            enemy.TakeDamage(damage);
        }
    }

    public void FixedUpdate() {
    }

    private void Enemy_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        isAttacking = false;
        //this to make it not teleport to next enemy
        isOnTopTarget = false;
    }
}