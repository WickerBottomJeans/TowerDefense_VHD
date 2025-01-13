using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SwordSpell : MonoBehaviour, IProjectile {
    [SerializeField] private float speed = .5f;
    [SerializeField] private float chillTime = 1f;
    [SerializeField] private float damage = 10;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float smoothFactor = 0.1f;
    [SerializeField] private _BaseTurret turret;
    [SerializeField] private Transform visual;
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

    //We need someway to turn off isOnTopTarget when the target get out of turret range
    // Sword seem to don't switch target when it get out of turret range
    private void Update() {
        if (isAttacking && target != null) {
            if (isOnTopTarget) {
                // Stick the sword to the enemy
                //MAGIC NUMBER!!!!!
                //REMEMBER TO REMOVE THIS MAGIC NUMBER AFTER EXPERIMENTING

                transform.position = target.position;
                //decide where u want to hold the sword
                visual.position = target.position + new Vector3(0.3f, 0, 0);
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
        if (target == null) {
            Debug.Log("Target == null????????");
        }
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