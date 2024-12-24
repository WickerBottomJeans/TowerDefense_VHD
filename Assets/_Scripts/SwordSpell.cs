using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SwordSpell : MonoBehaviour, IProjectile {
    [SerializeField] private BaseTurret turret;
    private Rigidbody2D rb;
    private Transform target;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
    }

    public GameObject SpawnProjectile(GameObject projectilePrefab, Transform SpawnPoint, Transform target) {
        GameObject projectile = Instantiate(projectilePrefab, SpawnPoint.position, Quaternion.identity);
        projectile.GetComponent<SwordSpell>().SetTarget(target);
        return projectile;
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }
}