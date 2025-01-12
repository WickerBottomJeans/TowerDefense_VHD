using System.Collections.Generic;
using UnityEngine;

public class EnemyBow : Enemy {

    [Header("Bow Settings")]
    public GameObject arrowPrefab; // Prefab của mũi tên

    public Transform arrowSpawnPoint; // Vị trí bắn mũi tên
    public float arrowSpeed = 10f; // Tốc độ mũi tên
    public float attackRange = 5f; // Phạm vi bắn

    protected override void AttackTower() {
        if (targetTurret != null) {
            // Kiểm tra khoảng cách trước khi bắn
            float distanceToTurret = Vector3.Distance(transform.position, targetTurret.transform.position);
            if (distanceToTurret <= attackRange) {
                // Gọi hàm bắn mũi tên
                ShootArrow(targetTurret.transform);

                // Phát âm thanh bắn
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null) {
                    audioSource.Play();
                }

                // Đặt lại thời gian hồi tấn công
                attackTimer = attackCooldown;
            }
        }
    }

    private void ShootArrow(Transform target) {
        // Tạo mũi tên
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        // Lấy hướng bắn
        Vector2 direction = (target.position - arrowSpawnPoint.position).normalized;

        // Thêm vận tốc cho mũi tên
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.linearVelocity = direction * arrowSpeed;
        }
    }
}