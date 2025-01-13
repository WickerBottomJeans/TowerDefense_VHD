using UnityEngine;

public class DarkSpell : MonoBehaviour, ISpecialAbility {
    public float range = 2f;
    public float cooldown = 5f;
    public GameObject spellRangeCirclePrefab;  // Reference to the circle prefab with sprite and animation
    public float mpCost = 50f;
    public float MPCost => mpCost;

    public float CoolDown => cooldown;
    public bool RequiresAiming => false;

    private GameObject spellRangeCircleInstance;

    private void Start() {
        // Instantiate the range circle and adjust its size
        if (spellRangeCirclePrefab != null) {
            spellRangeCircleInstance = Instantiate(spellRangeCirclePrefab, transform.position, Quaternion.identity);
            spellRangeCircleInstance.transform.localScale = new Vector3(range * 2f, range * 2f, 1f);  // Scale based on the diameter
        }
    }

    public void Activate(Vector2 targetLocation, _BaseTurret turret) {
        // Set the position of the spell and range circle to the target location
        transform.position = new Vector2(targetLocation.x, targetLocation.y);

        // Move the range circle to the target location
        if (spellRangeCircleInstance != null) {
            spellRangeCircleInstance.transform.position = new Vector3(targetLocation.x, targetLocation.y, spellRangeCircleInstance.transform.position.z);
        }

        // Find all enemies within the spell's range using a circle overlap
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(targetLocation, range);

        foreach (Collider2D enemyCollider in enemiesInRange) {
            if (enemyCollider.CompareTag("Enemy")) {
                // Get the Enemy component and call ApplyHypnotize() method
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null) {
                    enemy.ApplyHypnotize();
                    Debug.Log(enemy + " got spelled");
                }
            }
        }
    }
}