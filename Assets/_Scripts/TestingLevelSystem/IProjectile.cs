using UnityEngine;

public interface IProjectile {

    public GameObject SpawnProjectile(GameObject projectilePrefab, Transform SpawnPoint, Transform target);

    public void SetTarget(Transform target);
}