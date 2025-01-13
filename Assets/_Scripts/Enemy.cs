using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

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

    private DetectorForEnemy detectorForEnemy;

    public float attackRange = 0.5f;

    [Header("References")]
    public Slider healthBar; // Thanh máu bên trên

    protected Transform[] waypoints;

    protected List<GameObject> targetInRange = new List<GameObject>();
    protected GameObject targetTurret;
    protected int currentWaypointIndex = 0;
    protected float attackTimer = 0f;
    private CircleCollider2D circleCollider2D;

    public event EventHandler<OnEnemyDestroyedEventArgs> OnEnemyDestroyed;

    public class OnEnemyDestroyedEventArgs : EventArgs {
        public float mpGain;
        public float expGain;
    }

    // Thêm biến trạng thái "bị thôi miên"
    private bool isHypnotized = false;

    private float hypnotizedDuration = 5f; // Thời gian thôi miên
    private float hypnotizedTimer = 0f;

    //provoke stuff
    public bool isProvoked;

    public GameObject provokeTarget;

    public void OnTurretEnterRange(GameObject turret) {
        targetInRange.Add(turret);
    }

    public void OnTurretExitRange(GameObject turret) {
        targetInRange.Remove(turret);
    }

    protected virtual void Start() {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        WaypointManager waypointManager = UnityEngine.Object.FindAnyObjectByType<WaypointManager>();
        if (waypointManager != null) {
            waypoints = waypointManager.GetWaypoints();
        } else {
            Debug.LogError("WaypointManager not found in the scene.");
        }

        Transform detectorTransform = transform.Find("DetectorForEnemy");
        if (detectorTransform != null) {
            CircleCollider2D detectorCollider = detectorTransform.GetComponent<CircleCollider2D>();
            if (detectorCollider != null) {
                detectorCollider.radius = detectRange;
            } else {
                Debug.Log("Hi");
            }
        }
    }

    protected virtual void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            ApplyHypnotize();
        }

        attackTimer -= Time.deltaTime;

        // Kiểm tra trạng thái "bị thôi miên"
        if (isHypnotized) {
            hypnotizedTimer -= Time.deltaTime;
            if (hypnotizedTimer <= 0) {
                isHypnotized = false;
                gameObject.tag = "Enemy"; // Đổi lại tag khi hết thôi miên
                Debug.Log("Enemy is no longer hypnotized.");
            }
        }

        if (isProvoked) {
            targetTurret = provokeTarget;
            if (targetTurret != null) {
                float distanceToTurret = Vector3.Distance(transform.position, targetTurret.transform.position);
                if (distanceToTurret > attackRange) {
                    MoveToTarget(targetTurret.transform.position);
                } else if (attackTimer <= 0) {
                    AttackTower();
                }
            } else {
                Debug.Log("Target turret == null");
            }
            return;
        }
        if (isHypnotized) {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null) {
                float distanceToEnemy = Vector3.Distance(transform.position, closestEnemy.transform.position);
                if (distanceToEnemy > attackRange) {
                    MoveToTarget(closestEnemy.transform.position);
                } else if (attackTimer <= 0) {
                    AttackEnemy(closestEnemy);
                }
            }
        } else {
            if (targetInRange.Count > 0) {
                targetTurret = FindTarget();
                if (targetTurret != null) {
                    float distanceToTurret = Vector3.Distance(transform.position, targetTurret.transform.position);
                    if (distanceToTurret > attackRange) {
                        MoveToTarget(targetTurret.transform.position);
                    } else if (attackTimer <= 0) {
                        AttackTower();
                    }
                }
            } else if (waypoints != null && waypoints.Length > 0) {
                MoveAlongWaypoints();
            }
        }
    }

    private GameObject FindTarget() {
        GameObject closestTurret = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject turret in targetInRange) {
            float distanceToTurret = Vector3.Distance(transform.position, turret.transform.position);
            if (distanceToTurret < shortestDistance) {
                shortestDistance = distanceToTurret;
                closestTurret = turret;
            }
        }
        return closestTurret;
    }

    private GameObject FindClosestEnemy() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies) {
            if (enemy == this.gameObject) continue; // Bỏ qua chính nó

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    protected virtual void MoveToTarget(Vector3 targetPosition) {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Cập nhật hướng mặt của enemy
        if (direction.x != 0) {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(direction.x);
            transform.localScale = localScale;
        }
    }

    protected virtual void MoveAlongWaypoints() {
        if (currentWaypointIndex < waypoints.Length) {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Cập nhật hướng mặt của enemy
            if (direction.x != 0) {
                Vector3 localScale = transform.localScale;
                localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(direction.x);
                transform.localScale = localScale;
            }

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f) {
                currentWaypointIndex++;
            }
        }
    }

    protected virtual void AttackTower() {
        if (targetTurret != null) {
            _BaseTurret tower = targetTurret.GetComponent<_BaseTurret>();
            if (tower != null) {
                tower.TakeDamage(attackDamage);

                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null && !audioSource.isPlaying) {
                    audioSource.Play();
                }
            }
        }

        attackTimer = attackCooldown;
    }

    protected virtual void AttackEnemy(GameObject enemy) {
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null) {
            enemyScript.TakeDamage(attackDamage);
            Debug.Log($"Hypnotized enemy attacked another enemy for {attackDamage} damage.");
        }

        attackTimer = attackCooldown;
    }

    public virtual void TakeDamage(float damage) {
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0) {
            Die();
        }
    }

    protected virtual void Die() {
        DesTroySelf();
    }

    protected virtual void DesTroySelf() {
        OnEnemyDestroyed?.Invoke(this, new OnEnemyDestroyedEventArgs {
            mpGain = mpGain,
            expGain = expGain,
        });

        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null) {
            coinManager.AddCoin(coinGain);
        }

        Destroy(gameObject);
    }

    public float getMPGain() {
        return mpGain;
    }

    public float getEXPGain() {
        return expGain;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // Thêm phương thức để kích hoạt trạng thái "bị thôi miên"

    public event EventHandler OnAppliedHypnotize;

    public void ApplyHypnotize() {
        isHypnotized = true;
        hypnotizedTimer = hypnotizedDuration;
        gameObject.tag = "SpelledEnemy";

        //fire event to turret to notice hypnotization
        OnAppliedHypnotize?.Invoke(this, EventArgs.Empty);

        // Change the color to red using SpriteRenderer
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.color = Color.red;
        }
    }

    // Thêm phương thức để thay đổi mục tiêu tấn công sang trụ
    public void ChangeTargetToTower(GameObject tower) {
        provokeTarget = tower;
        isProvoked = true;
        isHypnotized = false; // Dừng trạng thái thôi miên nếu có
        Debug.Log("Enemy changed target to tower: " + tower.name);
    }
}