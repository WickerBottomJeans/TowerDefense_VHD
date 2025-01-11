using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float speed = 2f;
    public float mpGain = 5f; // quái chết trả về
    public float expGain = 5f; // quái chết trả về
    public int coinGain = 10; // quái chết trả về
    public float attackDamage = 10f; // Sát thương khi tấn công trụ
    public float attackCooldown = 2f; // Thời gian hồi giữa các đòn tấn công

    [Header("Detection Settings")]
    public float detectRange = 1f;
    public float attackRange = 0.5f;

    [Header("References")]
    public Slider healthBar; // Thanh máu bên trên
    private Transform[] waypoints; // Path waypoints for the enemies to follow

    private List<GameObject> turretInRange = new List<GameObject>();
    private GameObject targetTurret;
    private int currentWaypointIndex = 0; // Index of the next waypoint
    private float attackTimer = 0f; // Bộ đếm thời gian cho tấn công

    private CircleCollider2D circleCollider2D;

    public event EventHandler<OnEnemyDestroyedEventArgs> OnEnemyDestroyed; //

    public class OnEnemyDestroyedEventArgs : EventArgs
    {
        public float mpGain;
        public float expGain;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Lấy danh sách waypoint từ WaypointManager
        WaypointManager waypointManager = UnityEngine.Object.FindAnyObjectByType<WaypointManager>();
        if (waypointManager != null)
        {
            waypoints = waypointManager.GetWaypoints();
        }
        else
        {
            Debug.LogError("WaypointManager not found in the scene.");
        }

        circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = true;
        circleCollider2D.radius = detectRange;

        // Thêm Collider nếu chưa có
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        // Check for turrets in range
        if (turretInRange.Count > 0)
        {
            targetTurret = FindTurret();
            if (targetTurret != null)
            {
                float distanceToTurret = Vector3.Distance(transform.position, targetTurret.transform.position);
                // Move toward turret if not within attack range
                if (distanceToTurret > attackRange)
                {
                    MoveToTarget(targetTurret.transform.position);
                }
                // Attack if within range and cooldown is ready
                else if (attackTimer <= 0)
                {
                    AttackTower();
                }
            }
        }
        // If no turrets are detected or turret destroyed, follow waypoints
        else if (waypoints != null && waypoints.Length > 0)
        {
            MoveAlongWaypoints();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Turret"))
        {
            turretInRange.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Turret"))
        {
            turretInRange.Remove(other.gameObject);
        }
    }

    private GameObject FindTurret()
    {
        GameObject closestTurret = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject turret in turretInRange)
        {
            float distanceToTurret = Vector3.Distance(transform.position, turret.transform.position);
            if (distanceToTurret < shortestDistance)
            {
                shortestDistance = distanceToTurret;
                closestTurret = turret;
            }
        }
        return closestTurret;
    }

    private void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void MoveAlongWaypoints()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Check if the enemy has reached the waypoint
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
    }

    private void AttackTower()
    {
        if (targetTurret != null)
        {
            _BaseTurret tower = targetTurret.GetComponent<_BaseTurret>();
            if (tower != null)
            {
                tower.TakeDamage(attackDamage);
                Debug.Log($"Enemy attacked the tower for {attackDamage} damage.");

                // If the turret is destroyed, clear it from the target and continue to the next waypoint
                //if (tower.IsDestroyed())
                //{
                //    turretInRange.Remove(targetTurret);
                //    targetTurret = null;
                //}
            }
        }

        attackTimer = attackCooldown; // Đặt lại thời gian hồi
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DesTroySelf();
    }

    private void DesTroySelf()
    {
        OnEnemyDestroyed?.Invoke(this, new OnEnemyDestroyedEventArgs
        {
            mpGain = mpGain,
            expGain = expGain,
        });

        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.AddCoin(coinGain);
        }

        Destroy(gameObject);
    }

    public float getMPGain()
    {
        return mpGain;
    }

    public float getEXPGain()
    {
        return expGain;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange); // Vẽ phạm vi phát hiện
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Vẽ phạm vi tấn công
    }
}
