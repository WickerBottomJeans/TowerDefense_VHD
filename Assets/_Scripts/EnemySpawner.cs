using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;      
    public Transform spawnPoint;        
    public Transform[] waypoints;       
    public float spawnTimerMax = 5f;    

    private float spawnTimer;           

    private void Update() {
        // Countdown timer for spawning
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f) {
            SpawnEnemy();
            spawnTimer = spawnTimerMax; // Reset the timer
        }
    }

    private void SpawnEnemy() {
        // Instantiate the enemy at the spawn point
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Assign waypoints to the enemy
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null) {
            enemyScript.waypoints = waypoints;
        }
        
    }
}
