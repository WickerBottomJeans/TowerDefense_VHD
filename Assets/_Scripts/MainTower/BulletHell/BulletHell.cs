using System;
using UnityEngine;

public class BulletHell : MonoBehaviour
{
    [SerializeField] private float damage = 200f;
    [SerializeField] private UbhBulletSimpleSprite2d bullet;

    private void Awake()
    {
        bullet = GetComponentInParent<UbhBulletSimpleSprite2d>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(GameTags.TAG_ENEMY)) {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
        }
    }
}
