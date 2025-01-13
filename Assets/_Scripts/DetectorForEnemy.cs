using UnityEngine;
using System.Collections.Generic;

public class DetectorForEnemy : MonoBehaviour {
    private Enemy parentEnemy; // Reference to the parent Enemy script

    private void Start() {
        parentEnemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Turret") || other.CompareTag("SpelledEnemy")) { // Assuming turrets are tagged as "Turret"
            parentEnemy?.OnTurretEnterRange(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Turret") || other.CompareTag("SpelledEnemy")) {
            parentEnemy?.OnTurretExitRange(other.gameObject);
        }
    }
}