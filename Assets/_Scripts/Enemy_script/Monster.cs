using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public delegate void DeathEventHandler(Monster monster);
    public event DeathEventHandler OnDeath;

    [Header("Monster Properties")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public int rewardGold = 100;

    [Header("UI Elements")]
    public Slider healthBar;

    private float currentHealth;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isDead = false;

    //void Start()
    //{
    //    currentHealth = maxHealth;
    //    healthBar.maxValue = maxHealth;
    //    healthBar.value = currentHealth;

    //    // Lấy danh sách waypoint từ WaypointManager
    //    WaypointManager waypointManager = Object.FindAnyObjectByType<WaypointManager>();
    //    if (waypointManager != null)
    //    {
    //        waypoints = waypointManager.GetWaypoints();
    //    }
    //    else
    //    {
    //        Debug.LogError("WaypointManager not found in the scene.");
    //    }
    //}
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Thêm Collider nếu chưa có
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        // Lấy danh sách waypoint từ WaypointManager
        WaypointManager waypointManager = Object.FindAnyObjectByType<WaypointManager>();
        if (waypointManager != null)
        {
            waypoints = waypointManager.GetWaypoints();
        }
        else
        {
            Debug.LogError("WaypointManager not found in the scene.");
        }
    }


    //void Update()
    //{
    //    if (!isDead)
    //    {
    //        MoveAlongWaypoints();
    //    }
    //}

    void Update()
    {
        if (!isDead)
        {
            MoveAlongWaypoints();

            // Kiểm tra khi nhấn phím Space
            if (Input.GetKeyDown(KeyCode.Space)) // Kiểm tra phím Space
            {
                // Gây sát thương cho quái vật
                TakeDamage(50f); // Trừ 20 máu mỗi lần bấm Space
            }
        }
    }




    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;  // Trừ máu khi nhận sát thương
        healthBar.value = currentHealth;

        // Kiểm tra nếu máu <= 0, gọi Die() để quái vật chết và bị hủy
        if (currentHealth <= 0 && !isDead)
        {
            Die(); // Quái vật chết
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true; // Đánh dấu quái vật là đã chết

        // Gọi sự kiện OnDeath khi quái vật chết
        OnDeath?.Invoke(this);

        //GameManager.Instance.AddGold(rewardGold); // Thêm vàng cho người chơi khi quái vật chết
        Destroy(gameObject); // Hủy quái vật khỏi game

    }



    private void MoveAlongWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0 || currentWaypointIndex >= waypoints.Length) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                ReachDestination();
            }
        }
    }

    private void ReachDestination()
    {
        Debug.Log("Monster reached the destination!");
        Destroy(gameObject);
    }

   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
