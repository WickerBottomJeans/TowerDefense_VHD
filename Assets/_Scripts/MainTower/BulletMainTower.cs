using System;
using UnityEngine;

public class BulletMainTower : MonoBehaviour, IProjectile
{
    [SerializeField] private float speed = .5f;
    [SerializeField] private float chillTime = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speedMultiplier;
    private Transform target; // The target the projectile is moving towards
    private Rigidbody2D rb;

    private float chillTimer = 0f;
    private bool isChasingTarget = false;
    private bool isDoneChasing = false;
    private Vector2 initialDirection; // Initial direction of throw
    [SerializeField] private float smoothFactor = 0.1f; // Smoothness of transition (smaller = smoother)

    public enum State
    {
        Spawn,
        Idle,
        Destroy
    }

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs
    {
        public State state;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        FireOnStateChanged(State.Spawn);
    }

    private void FireOnStateChanged(State state)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });
    }

    private void Update()
    {
        if (chillTimer > 0f)
        {
            chillTimer -= Time.deltaTime; // Wait for chill phase to end
        }
    }

    public float forceAmount = 2f;
    private bool forceApplied = false;
    public float torque = 5f;

    private void FixedUpdate()
    {
        if (chillTimer > 0f && target != null && forceApplied == false)
        {
            // Chilling phase: Move in the initial direction
            //Vector2 directionToTarget = (target.position - transform.position).normalized;
            //initialDirection = -directionToTarget;
            //rb.linearVelocity = initialDirection * speed;
            float torque = 5f; // Rotational force
            /*rb.AddTorque(torque, ForceMode2D.Impulse);
            Vector2 upwardForce = Vector2.up * forceAmount;
            rb.AddForce(upwardForce, ForceMode2D.Force);*/
            forceApplied = true;
        }
        else if (isChasingTarget && target != null)
        {
            // Chasing phase: Smoothly transition towards the target

            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            Vector2 newVelocity = Vector2.Lerp(currentVelocity, directionToTarget * speed, smoothFactor);
            rb.linearVelocity = newVelocity;
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (isDoneChasing)
        {
            // After chasing is complete: Stop movement and disable physics
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            //rb.bodyType = RigidbodyType2D.Kinematic;
            GetComponent<CircleCollider2D>().enabled = false;
            transform.rotation = Quaternion.identity;
        }
        else if (chillTimer <= 0f && !isChasingTarget)
        {
            // Transition from chilling to chasing
            isChasingTarget = true;
            FireOnStateChanged(State.Idle);
            speed *= speedMultiplier;
        }
    }

    public GameObject SpawnProjectile(GameObject projectilePrefab, Transform spawnPoint, Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        BulletMainTower projScript = projectile.GetComponent<BulletMainTower>();
        if (projScript != null)
        {
            projScript.SetTarget(target); // Set the target and let the damage be handled by the WaterSpell itself
        }
        return projectile;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        chillTimer = chillTime;
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;
        }
    }

    private void Enemy_OnEnemyDestroyed(object sender, EventArgs e)
    {
        FireOnStateChanged(State.Destroy);

        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            //Stop the projectile from bouncing off and keep hitting enemy again
            isChasingTarget = false;
            isDoneChasing = true;

            // Destroy the projectile after hitting the target
            FireOnStateChanged(State.Destroy);
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Start the chill phase as soon as the projectile is enabled (launched)
        chillTimer = chillTime;
    }
}
