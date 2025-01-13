using UnityEngine;
using System.Collections.Generic;

public class EarthHammer : MonoBehaviour, ISpecialAbility {
    public float PushForce = 10f; // The force with which enemies are pushed away
    public float PushRadius = 1f; // The radius of effect for the push
    public float mpCost = 50f;
    public float MPCost => mpCost;

    // List of enemies to push (you can add your own logic for detecting enemies)
    private List<GameObject> enemiesInRange = new List<GameObject>();

    public float cooldown = 5f;
    public bool RequiresAiming => true; // Requires targeting location
    public float CoolDown => cooldown; // No cooldown for this spell

    public void Activate(Vector2 targetLocation, _BaseTurret turret) {
        // Detect enemies in range
        Debug.Log("EAT ASSSSSS");
        DetectEnemiesInRange(targetLocation);
        transform.position = targetLocation;
        // Push each enemy only once
        for (int i = 0; i < enemiesInRange.Count; i++) {
            GameObject enemy = enemiesInRange[i];
            Vector2 direction = (enemy.transform.position - (Vector3)targetLocation).normalized;
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.AddForce(direction * PushForce, ForceMode2D.Impulse);
            }
        }
    }

    // This method detects enemies in range of the target location
    private void DetectEnemiesInRange(Vector2 targetLocation) {
        enemiesInRange.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetLocation, PushRadius);
        for (int i = 0; i < colliders.Length; i++) {
            Collider2D collider = colliders[i];
            if (collider.CompareTag("Enemy")) {
                enemiesInRange.Add(collider.gameObject);
            }
        }
    }
}