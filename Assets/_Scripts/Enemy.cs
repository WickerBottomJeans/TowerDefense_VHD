using System;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Enemy Stats")]
    public float maxHealth = 100f;

    private float currentHealth;
    public float speed = 2f;
    public float mpGain = 5f; // quái chết trả về 
    public float expGain = 5f; //quái chết trả về 
    public int coinGain = 10; //quái chết trả về 

    [Header("References")]
    public Transform[] waypoints;       // Path waypoints for the enemies to follow

    private int currentWaypointIndex = 0; // Index of the next waypoint

    private void Start() {
        currentHealth = maxHealth;
    }

    public event EventHandler<OnEnemyDestroyedEventArgs> OnEnemyDestroyed; //

    public class OnEnemyDestroyedEventArgs : EventArgs {
        public float mpGain;
        public float expGain;
    }

    private void Update() {
        // Move along the path if waypoints are assigned
        if (waypoints != null && waypoints.Length > 0) {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath() {
        // Get the next waypoint
        if (currentWaypointIndex < waypoints.Length) {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;

            // Move towards the waypoint
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Check if the enemy has reached the waypoint
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f) {
                // Move to the next waypoint
                currentWaypointIndex++;

                // If all waypoints are passed, handle end of path (destroy or other logic)
                if (currentWaypointIndex >= waypoints.Length) {
                    // Reached the end, destroy the enemy or trigger something else
                    DesTroySelf();
                }
            }
        }
    }

    public void TakeDamage(float damage) {
        // Reduce health
        currentHealth -= damage;

        // Check if the enemy is dead
        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        DesTroySelf();
    }

    private void DesTroySelf() {
        OnEnemyDestroyed?.Invoke(this, new OnEnemyDestroyedEventArgs {
            mpGain = mpGain,
            expGain = expGain,
        });

        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null) {
            coinManager.AddCoin(coinGain); // Add the coinGain amount to the CoinManager
        }
        Destroy(gameObject);
    }

    // phải có
    public float getMPGain() {   
        return mpGain;
    }

    public float getEXPGain() {
        return expGain;
    }
}