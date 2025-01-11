using System;
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

    [Header("References")]
    public Slider healthBar; // Thanh máu bên trên
    private Transform[] waypoints; // Path waypoints for the enemies to follow
    private Transform targetTower; // Trụ mục tiêu

    private int currentWaypointIndex = 0; // Index of the next waypoint
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

        FindTurret();

        // Thêm Collider nếu chưa có
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    private void FindTurret()
    {
        // Tìm trụ mục tiêu dựa trên tag
        GameObject tower = GameObject.FindGameObjectWithTag("Turret");
        if (tower != null)
        {
            targetTower = tower.transform;
            Debug.Log("New tower found, heading to attack.");
        }
        else
        {
            targetTower = null; // Không có trụ trong cảnh
            Debug.Log("No tower found in the scene.");
        }
    }

    //private void Update()
    //{

    //    // Kiểm tra nếu đã đến trụ
    //    if (targetTower != null && Vector3.Distance(transform.position, targetTower.position) <= 2.5f)
    //    {
    //        AttackTower();
    //    }
    //    else if (waypoints != null && waypoints.Length > 0)
    //    {
    //        MoveAlongWaypoints();
    //    }
    //}
    private void Update()
    {
        // Cập nhật trụ nếu không có
        if (targetTower == null)
        {
            FindTurret(); // Tìm trụ mới nếu có
        }

        // Kiểm tra nếu đã đến trụ
        if (targetTower != null && Vector3.Distance(transform.position, targetTower.position) <= 2.5f)
        {
            AttackTower();
        }
        else if (waypoints != null && waypoints.Length > 0)
        {
            MoveAlongWaypoints();
        }
        Debug.Log(targetTower == null);
    }


    //private void MoveAlongWaypoints()
    //{
    //    if (currentWaypointIndex < waypoints.Length)
    //    {
    //        Transform targetWaypoint = waypoints[currentWaypointIndex];
    //        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
    //        transform.position += direction * speed * Time.deltaTime;

    //        // Check if the enemy has reached the waypoint
    //        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
    //        {
    //            currentWaypointIndex++;

    //            if (currentWaypointIndex >= waypoints.Length && targetTower != null)
    //            {
    //                // Hướng đến trụ sau khi đi hết waypoint
    //                targetTower = GameObject.FindGameObjectWithTag("Turret")?.transform;
    //            }
    //        }
    //    }
    //}
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

                if (currentWaypointIndex >= waypoints.Length && targetTower == null)
                {
                    FindTurret(); // Tìm trụ nếu waypoint kết thúc
                }
            }
        }
    }


    //private void AttackTower()
    //{
    //    if (attackTimer <= 0f)
    //    {
    //        // Tấn công trụ
    //        _BaseTurret tower = targetTower.GetComponent<_BaseTurret>();
    //        if (tower != null)
    //        {
    //            tower.TakeDamage(attackDamage);
    //            Debug.Log($"Enemy attacked the tower for {attackDamage} damage.");
    //        }

    //        // Đặt lại bộ đếm thời gian
    //        attackTimer = attackCooldown;
    //    }

    //    attackTimer -= Time.deltaTime;
    //}
    private void AttackTower()
    {
        if (attackTimer <= 2.0f)
        {
            if (targetTower != null)
            {
                _BaseTurret tower = targetTower.GetComponent<_BaseTurret>();
                if (tower != null)
                {
                    // Kiểm tra nếu trụ đã bị phá hủy
                    FindTurret(); // Tìm trụ mới nếu có
                    tower.TakeDamage(attackDamage);
                    Debug.Log($"Enemy attacked the tower for {attackDamage} damage.");
                }
            }

            // Đặt lại bộ đếm thời gian
            attackTimer = attackCooldown;
        }

        attackTimer -= Time.deltaTime;
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
        Gizmos.DrawWireSphere(transform.position, 2.5f); // Vẽ phạm vi tấn công
    }
}
