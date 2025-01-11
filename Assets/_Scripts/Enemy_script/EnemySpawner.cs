using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Properties")]
    public GameObject enemyPrefab; // Prefab của quái vật
    public Transform spawnPoint;   // Điểm spawn quái vật
    public float spawnInterval = 2f; // Thời gian giữa các lần spawn
    public int maxEnemies = 10;    // Số lượng quái tối đa

    private int currentEnemyCount = 0;

    void Start()
    {
        // Bắt đầu spawning quái vật theo chu kỳ
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemyCount >= maxEnemies)
        {
            Debug.Log("Maximum enemy limit reached.");
            return;
        }

        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            currentEnemyCount++;

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.OnEnemyDestroyed += HandleEnemyDeath;
            }
        }
        else
        {
            Debug.LogError("Enemy prefab or spawn point is not assigned.");
        }
    }

    void HandleEnemyDeath(object sender, Enemy.OnEnemyDestroyedEventArgs args)
    {
        currentEnemyCount--;
    }

    void OnDisable()
    {
        // Dừng spawning khi Spawner bị vô hiệu hóa
        CancelInvoke(nameof(SpawnEnemy));
    }
}
