using System.Collections;
using UnityEngine;

public class ThunderStrike : MonoBehaviour {
    public float damage = 300f;

    public void Initialize(Vector2 targetLocation) {
        // Set position
        transform.position = new Vector3(targetLocation.x, targetLocation.y, 0);

        // Start strike logic
        StartCoroutine(Strike());
    }

    private IEnumerator Strike() {
        // Visual or delay logic (e.g., lightning animation)
        yield return new WaitForSeconds(0.5f);

        // Enable the collider to detect enemies
        GetComponent<CircleCollider2D>().enabled = true;

        // Wait a short duration to allow the collider to detect
        yield return new WaitForSeconds(0.1f);

        // Destroy the strike object after
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Check if the object is an enemy and apply damage
        if (collision.CompareTag("Enemy") && collision.TryGetComponent(out Enemy enemyScript)) {
            enemyScript.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected() {
        // For debugging: draw the strike range in the editor
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, collider.radius);
        }
    }
}