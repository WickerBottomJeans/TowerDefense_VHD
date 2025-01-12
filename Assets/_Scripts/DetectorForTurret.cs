using UnityEngine;
using System.Collections.Generic;

public class DetectorForTurret : MonoBehaviour {
    private _BaseTurret parentTurret;
    private HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();

    private void Start() {
        parentTurret = GetComponentInParent<_BaseTurret>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Add(other.gameObject);
            parentTurret?.OnEnemyEnterRange(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Remove(other.gameObject);
            parentTurret?.OnEnemyExitRange(other.gameObject);
        }
    }
}