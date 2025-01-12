using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    protected float currentHealth; // Đổi từ private thành protected
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
    protected Transform[] waypoints; // Đổi từ private thành protected

    protected List<GameObject> turretInRange = new List<GameObject>(); // Đổi từ private thành protected
    protected GameObject targetTurret; // Đổi từ private thành protected
    protected int currentWaypointIndex = 0; // Đổi từ private thành protected
    protected float attackTimer = 0f; // Đổi từ private thành protected

    private CircleCollider2D circleCollider2D;

    public event EventHandler<OnEnemyDestroyedEventArgs> OnEnemyDestroyed;

    public class OnEnemyDestroyedEventArgs : EventArgs
    {
        public float mpGain;
        public float expGain;
    }

    protected virtual void Start() // Đổi private thành protected virtual
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

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

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    protected virtual void Update() // Đổi private thành protected virtual
    {
        attackTimer -= Time.deltaTime;

        if (turretInRange.Count > 0)
        {
            targetTurret = FindTurret();
            if (targetTurret != null)
            {
                float distanceToTurret = Vector3.Distance(transform.position, targetTurret.transform.position);
                if (distanceToTurret > attackRange)
                {
                    MoveToTarget(targetTurret.transform.position);
                }
                else if (attackTimer <= 0)
                {
                    AttackTower();
                }
            }
        }
        else if (waypoints != null && waypoints.Length > 0)
        {
            MoveAlongWaypoints();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Turret"))
        {
            turretInRange.Add(collision.gameObject);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other) 
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

    protected virtual void MoveToTarget(Vector3 targetPosition) // Đổi private thành protected
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    protected virtual void MoveAlongWaypoints() // Đổi private thành protected
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
    }

    protected virtual void AttackTower() 
    {
        if (targetTurret != null)
        {
            _BaseTurret tower = targetTurret.GetComponent<_BaseTurret>();
            if (tower != null)
            {
                tower.TakeDamage(attackDamage);
                Debug.Log($"Enemy attacked the tower for {attackDamage} damage.");

                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }

        attackTimer = attackCooldown;
    }

    public virtual void TakeDamage(float damage) // Đổi private thành public virtual
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die() // Đổi private thành protected virtual
    {
        DesTroySelf();
    }

    protected virtual void DesTroySelf() // Đổi private thành protected virtual
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
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
