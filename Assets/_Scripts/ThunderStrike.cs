using System.Collections;
using UnityEngine;

public class ThunderStrike : MonoBehaviour {
    public float damage = 300f;
    public float range = 2f;

    public void Initialize(Vector2 targetLocation) {
        // Set position
        transform.position = new Vector3(targetLocation.x, targetLocation.y, 0);

        // Start strike logic
        StartCoroutine(Strike());
    }

    private IEnumerator Strike() {
        // Visual or delay logic (e.g., lightning animation)
        yield return new WaitForSeconds(0.5f);

        // Apply damage to all enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D enemy in hitEnemies) {
            if (enemy.CompareTag("Enemy") && enemy.TryGetComponent(out Enemy enemyScript)) {
                enemyScript.TakeDamage(damage);
            }
        }

        // Destroy the strike object after
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        // For debugging: draw the strike range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}