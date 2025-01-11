using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class _BaseTurret : MonoBehaviour, IHasHPBar {

    #region Reference

    [SerializeField] public Transform firePoint;
    [SerializeField] public LevelSystem levelSystem;
    [SerializeField] public Transform turretPivot;
    [SerializeField] public TurretStatsSO turretStatsSO;
    public SpecialSkillButton specialSkillButton;

    #endregion Reference

    #region Variables

    protected bool specialSkillUnlocked = false;
    protected GameObject currentTarget;
    protected IProjectile iprojectile;
    public ISpecialAbility iSpecialAbility;
    [SerializeField] protected float currentHP, currentMP, currentAttackRange, currentFireRate;
    [SerializeField] protected float currentMaxHP, currentMaxMP, currentMaxAttackRange, currentMaxFireRate;
    protected HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();
    protected float fireCooldown = 0f;
    protected HashSet<GameObject> subscribedEnemies = new HashSet<GameObject>();

    public event EventHandler<IHasHPBar.OnHPChangedEventArgs> OnHPChanged;

    #endregion Variables

    #region LifeCycle

    protected void Update() {
        RotateTurretPivotTowardsTarget();
        HandleAttackCooldown();
    }

    protected void Start() {
        InitStats();
        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }

    #endregion LifeCycle

    #region VisualStuff

    protected void RotateTurretPivotTowardsTarget() {
        if (currentTarget == null || turretPivot == null) return;

        Vector2 direction = (currentTarget.transform.position - turretPivot.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretPivot.rotation = Quaternion.RotateTowards(
            turretPivot.rotation,
            Quaternion.Euler(0, 0, angle),
            360f * Time.deltaTime
        );
    }

    #endregion VisualStuff

    #region AttackStuff

    public abstract void AttackEnemy();

    public abstract GameObject FindEnemy();

    protected void EnemyScript_OnEnemyDestroyed(object sender, Enemy.OnEnemyDestroyedEventArgs e) {
        //Gain MP
        currentMP += e.mpGain;
        if (currentMP >= currentMaxMP) {
            currentMP = currentMaxMP;
        }
        //Gain EXP
        levelSystem.AddEXP(e.expGain);
        FireOnHPChanged();
    }

    protected void HandleAttackCooldown() {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f) {
            AttackEnemy();
            fireCooldown = 1f / currentMaxFireRate;
        }
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Add(other.gameObject);
        }
    }

    protected void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    #endregion AttackStuff

    #region DataStuff

    public void Upgrade(int level) {
        if (level == 0) { return; }
        currentMaxAttackRange = turretStatsSO.baseAttackRange + (level * (turretStatsSO.baseAttackRange * turretStatsSO.attackRangeMultiplier));
        currentMaxFireRate = turretStatsSO.baseFireRate + (level * (turretStatsSO.baseFireRate * turretStatsSO.fireRateMultiplier));
        currentMaxMP = turretStatsSO.baseMaxMP + (level * (turretStatsSO.baseMaxMP * turretStatsSO.maxMPMultiplier));
        currentMaxHP = turretStatsSO.baseMaxHP + (level * (turretStatsSO.baseMaxHP * turretStatsSO.maxHPMultiplier));

        // Min to ensure those current stats don't exceed the max
        currentHP = Mathf.Min(currentHP + (currentMaxHP - currentHP), currentMaxHP);
        //currentMP = Mathf.Min(currentMP + (currentMaxMP - currentMP), currentMaxMP);
        currentAttackRange = Mathf.Min(currentAttackRange + (currentMaxAttackRange - currentAttackRange), currentMaxAttackRange);
        currentFireRate = Mathf.Min(currentFireRate + (currentMaxFireRate - currentFireRate), currentMaxFireRate);

        FireOnHPChanged();
    }

    protected void InitStats() {
        currentAttackRange = currentMaxAttackRange = turretStatsSO.baseAttackRange;
        currentFireRate = currentMaxFireRate = turretStatsSO.baseFireRate;
        currentHP = currentMaxMP = turretStatsSO.baseMaxMP;

        currentMP = 0f;
        GetComponent<CircleCollider2D>().radius = currentMaxAttackRange;
        iprojectile = turretStatsSO.projectilePrefab.GetComponent<IProjectile>();
        iSpecialAbility = turretStatsSO.specialAbilityGameObject.GetComponent<ISpecialAbility>();
        specialSkillButton = transform.Find("SpecialSkillButtonCanvas/SpecialSkillButton")?.GetComponent<SpecialSkillButton>();
    }

    protected void LevelSystem_OnLevelUp(object sender, LevelSystem.OnLevelUpEventArgs e) {
        FireOnHPChanged();
        Upgrade(e.level);
        if (e.isMaxLevel) {
            specialSkillUnlocked = true;
            specialSkillButton.OnSpecialButtonClicked += SpecialSkillButton_OnSpecialButtonClicked;
        }
    }

    private void SpecialSkillButton_OnSpecialButtonClicked(object sender, SpecialSkillButton.OnSpecialButtonClickedEventArgs e) {
        CastSpecialSkill(e.targetLocation);
    }

    protected abstract void CastSpecialSkill(Vector2 targetLocation);

    #endregion DataStuff

    protected void TakeDamage(float damage) {
        if (currentHP > damage) {
            currentHP -= damage;
            FireOnHPChanged();
        } else {
            Die();
            OnHPChanged?.Invoke(this, new IHasHPBar.OnHPChangedEventArgs {
                HPNormalized = 0f
            });
        }
    }

    protected void Die() {
        Destroy(gameObject);
    }

    protected void FireOnHPChanged() {
        OnHPChanged?.Invoke(this, new IHasHPBar.OnHPChangedEventArgs {
            HPNormalized = currentHP / currentMaxHP,
            MPNormalized = currentMP / currentMaxMP,

            levelNormalized = (float)levelSystem.GetLevel() / levelSystem.GetMaxLevel()
        });
    }
}