using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainTowerSO", menuName = "ScriptableObjects/MainTowerSO", order = 1)]
public class MainTowerSO: ScriptableObject
{
    public List<TowerData> TowerDatas = new();
    
    public TowerData GetTowerData(int levelTower)
    {
        return TowerDatas[levelTower];
    }
}

[Serializable]
public class TowerData
{
    public float baseAttackRange = 5f;
    public float baseFireRate = 1f;
    public float baseMaxMP = 100f;
    public float baseMaxHP = 100f;
    public float attackRangeMultiplier = 0.1f;
    public float fireRateMultiplier = 0.1f;
    public float maxMPMultiplier = 0.1f;
    public float maxHPMultiplier = 0.1f;
    public GameObject projectilePrefab;
    public GameObject specialSkillPrefab;
}
