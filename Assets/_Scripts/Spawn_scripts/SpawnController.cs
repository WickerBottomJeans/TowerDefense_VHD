using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public enum SpawnMode
    {
        Controlled,
        Infinite,
        Timed
    }

    [SerializeField] private SpawnMode spawnMode = SpawnMode.Controlled;
    [SerializeField] private GameObject[] enemyPrefabs; // Danh sách quái
    [SerializeField] private GameObject[] bossPrefabs;  // Danh sách boss
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private int maxBosses = 1;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float bossSpawnInterval = 30f;

    [SerializeField] private float totalDuration; // Tổng thời gian cho chế độ TimedSpawn
    private float elapsedTime; // Thời gian đã trôi qua

    private int currentEnemyCount = 0;
    private int currentBossCount = 0;
    private float spawnTimer = 0f;
    private float bossSpawnTimer = 0f;

    void Start()
    {
        spawnTimer = spawnInterval;
        bossSpawnTimer = bossSpawnInterval;
        elapsedTime = 0f; // Đặt lại thời gian đã trôi qua khi bắt đầu
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime; // Cập nhật thời gian đã trôi qua

        switch (spawnMode)
        {
            case SpawnMode.Controlled:
                ControlledSpawn();
                break;
            case SpawnMode.Infinite:
                InfiniteSpawn();
                break;
            case SpawnMode.Timed:
                TimedSpawn();
                break;
        }
    }

    private void ControlledSpawn()
    {
        // Spawn quái
        if (currentEnemyCount < maxEnemies)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
            }
        }

        // Spawn boss
        if (currentBossCount < maxBosses)
        {
            bossSpawnTimer -= Time.deltaTime;
            if (bossSpawnTimer <= 0f)
            {
                SpawnBoss();
                bossSpawnTimer = bossSpawnInterval;
            }
        }
    }

    private void InfiniteSpawn()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }

        bossSpawnTimer -= Time.deltaTime;
        if (bossSpawnTimer <= 0f)
        {
            SpawnBoss();
            bossSpawnTimer = bossSpawnInterval;
        }
    }
    private void TimedSpawn()
    {
        // Kiểm tra nếu thời gian tổng đã vượt qua
        if (elapsedTime >= totalDuration)
        {
            Debug.Log("Timed Spawn Ended: Total duration reached.");
            return; // Ngừng spawn khi hết thời gian tổng
        }

        // Thực hiện logic spawn quái và boss như ControlledSpawn
        if (currentEnemyCount < maxEnemies)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
            }
        }

        if (currentBossCount < maxBosses)
        {
            bossSpawnTimer -= Time.deltaTime;
            if (bossSpawnTimer <= 0f)
            {
                SpawnBoss();
                bossSpawnTimer = bossSpawnInterval;
            }
        }
    }

    private void SpawnEnemy()
    {
        if (currentEnemyCount >= maxEnemies) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        currentEnemyCount++;
    }

    private void SpawnBoss()
    {
        if (currentBossCount >= maxBosses) return;

        if (bossPrefabs.Length == 0)
        {
            Debug.LogWarning("No boss prefabs assigned!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject bossPrefab = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

        currentBossCount++;
    }
}
