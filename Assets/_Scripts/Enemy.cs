using System;
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

    [Header("References")]
    public Slider healthBar; // Thanh máu bên trên
    private Transform[] waypoints; // Path waypoints for the enemies to follow

    private int currentWaypointIndex = 0; // Index of the next waypoint
    private bool isDead = false;
    private bool isAttackingTower = false;
    private float attackTimer = 0f; // Bộ đếm thời gian cho tấn công

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

        // Thêm Collider nếu chưa có
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        // Tìm trụ mục tiêu dựa trên tag
        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        if (tower != null)
        {
            targetTower = tower.transform;
        }
        else
        {
            Debug.LogError("Tower not found in the scene. Ensure it has the 'Tower' tag.");
        }
    }

    private void Update()
    {
        if (!isDead && waypoints != null && waypoints.Length > 0)
        {
            MoveAlongWaypoints();
        }
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

                if (currentWaypointIndex >= waypoints.Length)
                {
                    ReachDestination();
                }
            }
        }
    }

    private void ReachDestination()
    {
        Debug.Log("Enemy reached the destination!");
        DesTroySelf();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
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
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
