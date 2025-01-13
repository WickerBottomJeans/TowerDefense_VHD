using UnityEngine;

[CreateAssetMenu(fileName = "TurretStats", menuName = "ScriptableObjects/TurretStats", order = 1)]
public class TurretStatsSO : ScriptableObject {
    public float turretCost;
    public float baseAttackRange = 5f;
    public float baseFireRate = 1f;
    public float baseMaxMP = 100f;
    public float baseMaxHP = 200f;
    public float attackRangeMultiplier = 0.1f;
    public float fireRateMultiplier = 0.1f;
    public float maxMPMultiplier = 0.1f;
    public float maxHPMultiplier = 0.1f;
    public GameObject projectilePrefab;
    public GameObject specialAbilityGameObject;
    public GameObject secondSpecialAbilityGameObject;

    public _BaseTurret turretPrefab;
    public Sprite ghostSprite;
}