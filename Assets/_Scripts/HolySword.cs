using UnityEngine;

public class HolySword : MonoBehaviour, ISpecialAbility {
    public float range = 2f;
    public float cooldown = 5f;
    public float CoolDown => cooldown;
    public float mpCost = 50f;
    public float MPCost => mpCost;
    public bool RequiresAiming => false;

    public void Activate(Vector2 targetLocation, _BaseTurret turret) {
        transform.position = targetLocation;        // Find all colliders in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetLocation, range);

        // Iterate through all colliders and check for enemies
        foreach (Collider2D collider in colliders) {
            GameObject enemy = collider.gameObject;

            if (enemy.CompareTag("Enemy")) {
                // Call the ChangeTargetToTower method on the enemy if it has the required component
                var enemyScript = enemy.GetComponent<Enemy>(); // Replace "Enemy" with the actual script name
                if (enemyScript != null) {
                    enemyScript.ChangeTargetToTower(turret.gameObject); // Pass the tower (this object) as the argument
                }
            }
        }
    }
}